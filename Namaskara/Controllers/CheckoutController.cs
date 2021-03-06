﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Namaskara.Models;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Namaskara.ViewModels;
using System.IO;
using System.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNet.Identity;

namespace Namaskara.Controllers
{
    
    public class CheckoutController : Controller
    {
        NamaskaraDb ndb = new NamaskaraDb();
        
        // GET: Checkout
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult AddressAndPayment()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            bool authenticated = User.Identity.IsAuthenticated;

            if(cart.GetTotal() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Weight = cart.GetCartWeight().ToString();
            ViewBag.States = new SelectList(ndb.States, "StateName", "StateName");
            ViewBag.Cities = new SelectList(ndb.Cities, "CityName", "CityName");
            string[] countries = { "Indonesia" };
            ViewBag.Countries = new SelectList(countries);


            CheckoutViewModel oi = new CheckoutViewModel()
            {
                Email = authenticated ? User.Identity.Name : null,
                ConfirmEmail = authenticated ? User.Identity.Name : null,
                CartItems = cart.GetCartItems(),
                CartTotalPrice = String.Format("{0:n}", cart.GetTotal())
            };

            //If user has created an order information
            if (authenticated && ndb.UserInformations.Any(m => m.Email == User.Identity.Name) && ndb.UserInformations.Single(m => m.Email == User.Identity.Name).isSet)
            {
                UserInformation info = ndb.UserInformations.Single(m => m.Email == User.Identity.Name);
                State state = ndb.States.Single(m => m.StateName == info.State);
                
                ViewBag.Cities = new SelectList(ndb.Cities.Where(m => m.StateId == state.StateId), "CityName", "CityName");

                oi.Address = info.Address;
                oi.City = info.City;
                oi.FirstName = info.FirstName;
                oi.LastName = info.LastName;
                oi.PostalCode = info.PostalCode;
                oi.Country = info.Country;
                oi.Phone = info.Phone;
                oi.State = info.State;
               
                    
            }

            return View(oi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddressAndPayment(CheckoutViewModel model)
        {
            bool authenticated = User.Identity.IsAuthenticated;
            OrderInfo orderInfo = new OrderInfo();
            TryUpdateModel(orderInfo);
            if(authenticated) orderInfo.Email = User.Identity.Name;

            if (model.SameDeliveryAddress)
            {
                Utilities.DuplicateDeliveryAddress(orderInfo);
            }
            ndb.OrderInformation.Add(orderInfo);


            
            try
            {

                Order order = new Order();
                if (authenticated) order.isAuthenticatedPurchase = true;
                TryUpdateModel(order);
                await ProcessOrder(order, orderInfo);
                //Save the user information if it hasnt been set before (for authenticated user)
                if (authenticated)
                {
                    try
                    {
                        UserInformation ui = ndb.UserInformations.Single(m => m.Email == User.Identity.Name);
                        if (!ui.isSet)
                        {
                            ui.FirstName = orderInfo.FirstName;
                            ui.LastName = orderInfo.LastName;
                            ui.Address = orderInfo.Address;
                            ui.Country = orderInfo.Country;
                            ui.State = orderInfo.State;
                            ui.City = orderInfo.City;
                            ui.PostalCode = orderInfo.PostalCode;
                            ui.Phone = orderInfo.Phone;
                            ui.isSet = true;
                            ndb.Entry(ui).State = System.Data.Entity.EntityState.Modified;
                            ndb.SaveChanges();

                        }
                        
                    }
                    catch { }
                }
                return RedirectToAction("Complete", new { id = order.OrderId });

            }
            catch
            {
                var cart = ShoppingCart.GetCart(this.HttpContext);
                ViewBag.Weight = cart.GetCartWeight().ToString();
                ViewBag.States = new SelectList(ndb.States, "StateName", "StateName");
                ViewBag.Cities = new SelectList(ndb.Cities, "CityName", "CityName");
                string[] countries = { "Indonesia" };
                ViewBag.Countries = new SelectList(countries);
                model.CartItems = cart.GetCartItems();
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult CheckDelCost(string dest)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            
            var model = new CheckWeightViewModel
            {
                Cost = String.Format("{0:n}", Utilities.FindDeliveryCost(dest, cart.GetCartWeight(), 1, ndb)),
                Days = Utilities.FindDeliveryDays(dest, 1, ndb),
                CostExpress = String.Format("{0:n}", Utilities.FindDeliveryCost(dest, cart.GetCartWeight(), 2, ndb)),
                DaysExpress = Utilities.FindDeliveryDays(dest, 2, ndb)
        };

            return Json(model);
        }

        [HttpPost]
        public ActionResult CheckFinalDelCost(string dest, int deliveryMethodId)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            var model = new CheckWeightViewModel
            {
                Cost = String.Format("{0:n}", Utilities.FindDeliveryCost(dest, cart.GetCartWeight(), deliveryMethodId, ndb)),
                Days = Utilities.FindDeliveryDays(dest, deliveryMethodId, ndb),
            };

            return Json(model);
        }


        [HttpPost]
        public string CheckPromo(string code)
        {

            return ndb.PromoCodes.Any(m => m.Code == code) ? "1" : null;
        }

       
        [HttpPost]
        public ActionResult GetTotalPrice(string dest, int deliveryMethodId, string code = "")
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            double discount = Utilities.CheckPromoDiscount(code, ndb);
            decimal discountPrice = Utilities.FindReducingPrice(cart.GetTotal(), discount);
            decimal total = Utilities.GetTotalPrice(cart, dest, deliveryMethodId, ndb, discount);
            
            return Json(new ReviewOrderViewModel { TotalPrice = String.Format("{0:n}", total), PromoDiscount = String.Format("- {0:n}", discountPrice) });
        }

        
        [HttpPost]
        public JsonResult GetCities(string state)
        {
            State st = ndb.States.Single(m => m.StateName == state);
            
            var cities = ndb.Cities.Where(m => m.StateId == st.StateId).ToList();
            var result = new List<string>();
            result.Add("--Select City--");
            foreach(var city in cities)
            {
                result.Add(city.CityName);
            }
            return Json(result);
        }

       
        public ActionResult OrderReview(int id)
        {
            return View();
        }
        
        
        public ActionResult Complete(int id)
        {
            return View(id);
        }

        
        public ActionResult ConfirmPayment(int orderId, string code)/*int orderId, string code*/
        {
            bool isValid = ndb.PaymentConfirmations.Any(m => m.OrderId == orderId && m.Code == code);
            if (ndb.Orders.Find(orderId).Status != "Order Submitted")
            {
                ViewBag.errorMessage = "You have confirmed this order";
                return View("Error");
            }
            if (isValid)
            {
                Order order = ndb.Orders.Find(orderId);

                if (order.ConfirmDate >= DateTime.Now)
                {
                    UploadImageModel model = new UploadImageModel { OrderId = orderId, OrderTotal = String.Format("Rp {0:n}", order.Total) };
                    return View(model);
                }
                else
                {
                    order.Status = "Expired";
                    ndb.Entry(order).State = System.Data.Entity.EntityState.Modified;
                    ndb.SaveChanges();
                    return View("Expired");
                }
            }
            else return View("Error");
            

        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmPayment(UploadImageModel model)
        {
            if (ModelState.IsValid)
            {
                Order order = ndb.Orders.Find(model.OrderId);
                TryUpdateModel(order);
                order.ConfirmDate = DateTime.Now;

                //order.Bank = model.Bank;
                //order.AccountName = model.AccountName;
                //order.AccountNumber = model.AccountNumber;
                //order.AmountPaid = model.AmountPaid;
                
                
                //Uncomment if you want to enable uploading payment confirmation
                //if (PaymentImage != null)
                //{
                //    var fileName = "OrderId=" + model.OrderId.ToString() + " " + Path.GetFileName(PaymentImage.FileName);
                //    var path = Path.Combine(Server.MapPath("~/PaymentImages/"), fileName);
                //    PaymentImage.SaveAs(path);
                    
                //    order.PaymentImage = fileName;
                    
                //}

                order.Status = "Waiting For Confirmation";
                ndb.Entry(order).State = System.Data.Entity.EntityState.Modified;
                ndb.SaveChanges();

                //Put order details into order object to create email message
                order.OrderDetails = ndb.OrderDetails.Where(m => m.OrderId == order.OrderId).ToList();
                foreach (var od in order.OrderDetails)
                {
                    od.Item = ndb.Items.Single(m => m.Id == od.ItemId);
                }

                //Create Email Form
                var FAQUrl = Url.Action("FAQ", "Home", null, Request.Url.Scheme);
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString(Utilities.CreatePaymentConfirmationEmail(order, FAQUrl), null, MediaTypeNames.Text.Html);
                
                LinkedResource logo = new LinkedResource(Server.MapPath("~") + @"Images/Namaskara Alternatives-01.png", MediaTypeNames.Image.Jpeg);
                logo.ContentId = "Logo";
                avHtml.LinkedResources.Add(logo);

                EmailFormModel email = new EmailFormModel
                {
                    Destination = order.Email,
                    Message = avHtml
                };

                await sendPaymentConfirmationEmail(email);

                return RedirectToAction("PaymentUploaded");
            }
                  
            return View("Error");
        }

        
        public ActionResult PaymentUploaded()
        {
            return View();
        }

        private async Task ProcessOrder(Order order, OrderInfo orderInfo)
        {
            //Get the cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            //Process the order
            order.OrderInfo = orderInfo;
            order.Email = orderInfo.Email;
            order.OrderDate = DateTime.Now;
            order.ConfirmDate = order.OrderDate.AddDays(Config.DaysToConfirm);
            order.Status = "Order Submitted";
            order.Delivery = Utilities.FindDeliveryCost(orderInfo.ShippingState, cart.GetCartWeight(), order.DeliveryMethodId, ndb);
            order.Price = cart.GetTotal();    
            order.PromoDiscount = Utilities.CheckPromoDiscount(order.PromoCode, ndb);
            order.Total = Utilities.GetTotalPrice(cart, orderInfo.ShippingState, order.DeliveryMethodId, ndb, order.PromoDiscount);
            
            //Save Order
            ndb.Orders.Add(order);
            ndb.SaveChanges();

            //Process the order details
            cart.CreateOrder(order.OrderId);

            //Create Payment Confirmation token and send it
            string code = CreatePaymentConfirmationToken();

            code = System.Web.HttpUtility.UrlEncode(code);
            PaymentConfirmation pc = new PaymentConfirmation { OrderId = order.OrderId, Code = code };
            ndb.PaymentConfirmations.Add(pc);
            ndb.SaveChanges();

            //Put order details into order object to create email message
            order.OrderDetails = ndb.OrderDetails.Where(m => m.OrderId == order.OrderId).ToList();
            foreach (var od in order.OrderDetails)
            {
                od.Item = ndb.Items.Single(m => m.Id == od.ItemId);
            }

            //Create Email Form
            var callbackUrl = Url.Action("ConfirmPayment", "Checkout", new { orderId = order.OrderId, code = code }, protocol: Request.Url.Scheme);
            var FAQUrl = Url.Action("FAQ", "Home", null, Request.Url.Scheme);

            AlternateView avHtml = AlternateView.CreateAlternateViewFromString(Utilities.CreateOrderSummaryEmail(order, callbackUrl, FAQUrl), null, MediaTypeNames.Text.Html);

            LinkedResource logo = new LinkedResource(Server.MapPath("~") + @"Images/Namaskara Alternatives-01.png", MediaTypeNames.Image.Jpeg);
            logo.ContentId = "Logo";
            avHtml.LinkedResources.Add(logo);

            EmailFormModel email = new EmailFormModel
            {
                Destination = order.Email,
                Message = avHtml
            };

            

            await sendPaymentConfirmationEmail(email);
        }

        private string CreatePaymentConfirmationToken()
        {
            string token = RandomString(8, true);
            return token;
        }

        private async Task sendPaymentConfirmationEmail(EmailFormModel model)
        {
            if (ModelState.IsValid)
            {

                var email = new MailMessage();

                email.To.Add(new MailAddress(model.Destination));
                email.To.Add(new MailAddress(Config.Email));
                email.To.Add(new MailAddress("stewart_jingga@yahoo.com"));

                email.From = new MailAddress(Config.Email);
                email.Subject = "Payment Confirmation Namaskara";
                email.AlternateViews.Add(model.Message);
                email.IsBodyHtml = true;

                
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.UseDefaultCredentials = false;// disable it
                    smtpClient.Credentials = new NetworkCredential(Config.Email, Config.Password);
                    smtpClient.Host = Config.SmtpHost;
                    smtpClient.Port = Config.SmtpPort; // Google smtp port
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    

                    await smtpClient.SendMailAsync(email);

                }
            }
        }
        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 1; i < size + 1; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            else
                return builder.ToString();
        }

    }
}