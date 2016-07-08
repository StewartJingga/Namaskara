using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Namaskara.Models;
using System.Collections.Generic;
using Namaskara.ViewModels;
using System.Diagnostics;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Namaskara.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext context = new ApplicationDbContext();
        NamaskaraDb ndb = new NamaskaraDb();

        public AccountController()
        {

        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //[AllowAnonymous]
        //public async Task<ActionResult> SeedAdmin()
        //{
        //    IdentityRole role = new IdentityRole { Name = "Admin" };
        //    context.Roles.Add(role);
        //    IdentityRole role2 = new IdentityRole { Name = "Member" };
        //    context.Roles.Add(role2);
        //    context.SaveChanges();

        //    string email = "stewart_jingga@yahoo.com";
        //    var passwordHash = new PasswordHasher();
            
        //    string password = passwordHash.HashPassword("smithsog00d");
        //    var user = new ApplicationUser { UserName = email, Email = email };

        //    var result = await UserManager.CreateAsync(user, password);
        //    if (result.Succeeded)
        //    {
        //        //Adding user to Role
        //        await UserManager.AddToRoleAsync(user.Id, "Admin");

        //        UserInformation ui = new UserInformation { Email = user.UserName };
        //        ndb.UserInformations.Add(ui);
        //        ndb.SaveChanges();
        //    }

        //    return RedirectToAction("Index", "Home");

        //}


        //Adding User to Role
        [Authorize(Roles = "Admin")]
        public ActionResult AddUserToRole()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult AddUserToRole(AddUserToRole model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByEmail(model.Email);
                UserManager.AddToRole(user.Id, model.Role);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult AccountNavigation()
        {
            return PartialView();
        }

        public ActionResult Index()
        {

            return View();
        }
        public ActionResult UserInformation()
        {
            

            string userEmail = User.Identity.GetUserName();
            try
            {
                UserInformation userInfo = ndb.UserInformations.Single(m => m.Email == userEmail);
                UserAccountViewModel model = new UserAccountViewModel();

                model.Address = userInfo.Address;
                model.City = userInfo.City;
                model.Country = userInfo.Country;
                model.Email = userEmail;
                model.FirstName = userInfo.FirstName;
                model.LastName = userInfo.LastName;
                model.Phone = userInfo.Phone;
                model.PostalCode = userInfo.PostalCode;
                model.State = userInfo.State;

                return View(model);
            }
            catch { }
            
 
            return View();
        }

        public ActionResult EditInformation()
        {
            //var states = ndb.States.Select(s => new SelectListItem { Text = s.StateName, Value = s.StateName , Selected = (s.StateName == "Jawa Timur") }).ToList();
            //ViewBag.States = states;
            ViewBag.States = new SelectList(ndb.States, "StateName", "StateName");
            ViewBag.Cities = new SelectList(ndb.Cities, "CityName", "CityName");
            string[] countries = { "Indonesia" };
            ViewBag.Countries = new SelectList(countries);

            string userEmail = User.Identity.GetUserName();
            try
            {
                UserInformation userInfo = ndb.UserInformations.Single(m => m.Email == userEmail);
                UserAccountViewModel model = new UserAccountViewModel();

                model.Address = userInfo.Address;
                model.Email = userEmail;
                model.FirstName = userInfo.FirstName;
                model.LastName = userInfo.LastName;
                model.Phone = userInfo.Phone;
                model.PostalCode = userInfo.PostalCode;
                Debug.Print(userInfo.State);
                model.State = userInfo.State;
                model.City = userInfo.City;
                model.Country = userInfo.Country;
                

                return View(model);
            }catch { }
            return View();
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInformation(UserAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userEmail = User.Identity.GetUserName();
                bool infoExists = ndb.UserInformations.Any(m => m.Email == userEmail);
                UserInformation userInfo = infoExists ? ndb.UserInformations.Single(m => m.Email == userEmail) : new UserInformation();

                userInfo.Address = model.Address;
                userInfo.City = model.City;
                userInfo.Country = model.Country;
                userInfo.FirstName = model.FirstName;
                userInfo.LastName = model.LastName;
                userInfo.Phone = model.Phone;
                userInfo.PostalCode = model.PostalCode;
                userInfo.State = model.State;
                userInfo.Email = userEmail;
                userInfo.isSet = true;

                if (infoExists) ndb.Entry(userInfo).State = System.Data.Entity.EntityState.Modified;
                else ndb.UserInformations.Add(userInfo);

                ndb.SaveChanges();

                return RedirectToAction("UserInformation");
            }
            return View(model);
        }

        public ActionResult OrderHistory()
        {
            string userEmail = User.Identity.GetUserName();
            List<Order> orders = ndb.Orders.Where(m=>m.Email == userEmail && m.isAuthenticatedPurchase == true).ToList();
            return View(orders);
        }
    
        public ActionResult OrderDetails(int id)
        {
            
            List<OrderDetail> orderDetails = ndb.OrderDetails.Where(m => m.OrderId == id).ToList();
            foreach(var od in orderDetails)
            {
                od.Item = ndb.Items.Include("Product").Single(m=> m.Id == od.ItemId);
            }
            ViewData["OrderNumber"] = id;
            return PartialView(orderDetails);
        }

     
        public ActionResult ShippingDetails(int id)
        {
            ViewData["OrderNumber"] = id;
            return PartialView(ndb.Orders.Include("OrderInfo").Single(m => m.OrderId == id));
        }

        public ActionResult Wishlist()
        {
            List<WishList> model = ndb.Wishlists.Where(x => x.UserEmail == User.Identity.Name).ToList();

            foreach (var wl in model)
            {
                wl.Item = ndb.Items.Include("Product").Single(m => m.Id == wl.ItemId);
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public string AddToWishlist(int id)
        {
            if (!User.Identity.IsAuthenticated) return Config.WishlistFailed;
            else
            {
                try
                {

                    List<Item> items = ndb.Items.Where(m => m.ProductId == id).OrderByDescending(m => m.Id).ToList();
                    int itemid = items[0].Id;
                    bool wlexists = ndb.Wishlists.Any(m => m.ItemId == itemid && m.UserEmail == User.Identity.Name);

                    if (wlexists)
                    {
                        return Config.WishlistExists;
                    }

                    ndb.Wishlists.Add(new WishList
                    {
                        ItemId = items[0].Id,
                        UserEmail = User.Identity.Name,
                        DateListed = DateTime.Now,
                        Item = null
                    });

                    ndb.SaveChanges();

                    return Config.WishlistSuccess;
                }
                catch
                {
                    return "Error inputting wishlist";
                }

            }
        }

        [HttpPost]
        public ActionResult RemoveFromWishlist(int id)
        {
            WishList wl = ndb.Wishlists.Find(id);

            string itemName = ndb.Items.Single(m => m.Id == wl.ItemId).Name;

            ndb.Wishlists.Remove(wl);
            ndb.SaveChanges();

            var results = new ShoppingCartRemoveViewModel
            {
                Message = Server.HtmlEncode(itemName) + " has been removed from your Wishlist.",
                DeleteId = id
            };

            return Json(results);
        }

        //Migrating shopping cart to logged in user
        private void MigrateShoppingCart(string UserName)
        {
            //Associate shopping cart items with logged-in user
            var cart = ShoppingCart.GetCart(this.HttpContext);
            cart.MigrateCart(UserName);
            Session[ShoppingCart.CartSessionKey] = UserName;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Uncomment this to enable confirmation before logging in
            //var user = await UserManager.FindByNameAsync(model.Email);
            //if(user != null)
            //{
            //    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
            //    {
            //        ViewBag.errorMessage = "You must have a confirmed email to log on.";
            //        return View("Error");
            //    }
            //}

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    //Migrate shopping cart
                    MigrateShoppingCart(model.Email);
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //Adding user to Role
                    await UserManager.AddToRoleAsync(user.Id, "Member");

                    UserInformation ui = new UserInformation { Email = user.UserName };
                    ndb.UserInformations.Add(ui);
                    ndb.SaveChanges();
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    MigrateShoppingCart(model.Email);

                    //Uncomment this to enable email confirmation
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    await UserManager.ConfirmEmailAsync(user.Id, code);
                    //code = System.Web.HttpUtility.UrlEncode(code);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account",
                    //    new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id,
                    //    "Confirm your account", "Please confirm your account by clicking <a href=\""
                    //    + callbackUrl + "\">here</a>");

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771

                    //ViewBag.Message = "Check your email and confirm account, you must be confirmed " +
                    //    "before you can log in.";

                    //return View("Info");
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                UserInformation userInfo = new UserInformation() { Email = userId};
                ndb.UserInformations.Add(userInfo);
                ndb.SaveChanges();

                return View("ConfirmEmail");
            }
            return View("Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //code = System.Web.HttpUtility.UrlEncode(code);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                var FAQUrl = Url.Action("FAQ", "Home", null, Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", Utilities.CreateResetPasswordEmail(callbackUrl, FAQUrl));
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}