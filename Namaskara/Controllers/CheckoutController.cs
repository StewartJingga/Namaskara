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

namespace Namaskara.Controllers
{
    [Authorize]
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
                CartItems = cart.GetCartItems(),
                CartTotalPrice = String.Format("Rp {0:n}", cart.GetTotal())
            };

            //If user has created an order information
            if (ndb.UserInformations.Any(m => m.Email == User.Identity.Name))
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
        public ActionResult AddressAndPayment(CheckoutViewModel model)
        {

            OrderInfo orderInfo = new OrderInfo();
            TryUpdateModel(orderInfo);
            orderInfo.Email = User.Identity.Name;
            if (model.SameDeliveryAddress)
            {
                Utilities.DuplicateDeliveryAddress(orderInfo);
            }
            ndb.OrderInformation.Add(orderInfo);
            ndb.SaveChanges();

            try
            {
                Order order = new Order();
                TryUpdateModel(order);
                ProcessOrder(order, orderInfo);
                return RedirectToAction("Complete", new { id = order.OrderId });

            }
            catch
            {
                return View("Error");
            }
        }

        [AllowAnonymous]
        public ActionResult AnonymousAddressAndPayment()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            if (cart.GetTotal() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            
            ViewBag.Weight = cart.GetCartWeight().ToString();
            ViewBag.States = new SelectList(ndb.States, "StateName", "StateName");
            ViewBag.ShippingState = new SelectList(ndb.States, "StateName", "StateName");
            ViewBag.Cities = new SelectList(ndb.Cities, "CityName", "CityName");
            string[] countries = { "Indonesia" };
            ViewBag.Countries = new SelectList(countries);

            var model = new CheckoutViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotalPrice = String.Format("Rp {0:n}", cart.GetTotal())

            };
            return View(model);
            
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AnonymousAddressAndPayment(CheckoutViewModel model)
        {
            OrderInfo orderInfo = new OrderInfo();
            TryUpdateModel(orderInfo);
            if (model.SameDeliveryAddress)
            {
                Utilities.DuplicateDeliveryAddress(orderInfo);
            }
            ndb.OrderInformation.Add(orderInfo);
            ndb.SaveChanges();

            try
            {
                Order order = new Order();
                TryUpdateModel(order);
                ProcessOrder(order, orderInfo);
                return RedirectToAction("Complete", new { id = order.OrderId });
            }
            catch
            {
                return View(orderInfo);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckDelCost(string dest)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            var model = new CheckWeightViewModel { Cost = String.Format("Rp {0:n}", Utilities.FindDeliveryCost(dest, cart.GetCartWeight(), ndb)) };
            return Json(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public string CheckPromo(string code)
        {

            return ndb.PromoCodes.Any(m => m.Code == code) ? "1" : null;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetTotalPrice(string dest, string code = "")
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            double discount = Utilities.CheckPromoDiscount(code, ndb);
            decimal discountPrice = Utilities.FindReducingPrice(cart.GetTotal(), discount);
            decimal total = Utilities.GetTotalPrice(cart, dest, ndb, discount);
            
            return Json(new ReviewOrderViewModel { TotalPrice = String.Format("Rp {0:n}", total), PromoDiscount = String.Format("- Rp {0:n}", discountPrice) });
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        public ActionResult OrderReview(int id)
        {
            return View();
        }
        
        [AllowAnonymous]
        public ActionResult Complete(int id)
        {
            return View(id);
        }

        [AllowAnonymous]
        public ActionResult ConfirmPayment(int orderId, string code)
        {
            bool isValid = ndb.PaymentConfirmations.Any(m => m.OrderId == orderId && m.Code == code);
            if(ndb.Orders.Find(orderId).Status != "Order Submitted")
            {
                return View("Error");
            }
            if (isValid)
            {
                Order order = ndb.Orders.Find(orderId);
                
                if (order.ConfirmDate >= DateTime.Now)
                {
                    UploadImageModel model = new UploadImageModel { OrderId = orderId };
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

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmPayment(UploadImageModel model, HttpPostedFileBase PaymentImage)
        {
            if (ModelState.IsValid)
            {
                Order order = ndb.Orders.Find(model.OrderId);
                order.Bank = model.Bank;
                order.AccountName = model.AccountName;
                order.AccountNumber = model.AccountNumber;
                try
                {
                    order.AmountPaid = Decimal.Parse(model.AmountPaid);
                }
                catch { return View("Error"); }
                

                if (PaymentImage != null)
                {
                    var fileName = "OrderId=" + model.OrderId.ToString() + " " + Path.GetFileName(PaymentImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/PaymentImages/"), fileName);
                    PaymentImage.SaveAs(path);
                    
                    order.PaymentImage = fileName;
                    
                }
                order.Status = "Waiting For Confirmation";
                ndb.Entry(order).State = System.Data.Entity.EntityState.Modified;
                ndb.SaveChanges();
                return RedirectToAction("PaymentUploaded");
            }
                  
            return View("Error");
        }

        [AllowAnonymous]
        public ActionResult PaymentUploaded()
        {
            return View();
        }

        private void ProcessOrder(Order order, OrderInfo orderInfo)
        {
            //Get the cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            //Process the order
            order.OrderInfoId = orderInfo.OrderInfoId;
            order.Email = orderInfo.Email;
            order.OrderDate = DateTime.Now;
            order.ConfirmDate = order.OrderDate.AddDays(Config.DaysToConfirm);
            order.Status = "Order Submitted";
            order.Delivery = Utilities.FindDeliveryCost(orderInfo.ShippingState, cart.GetCartWeight(), ndb);
            order.Price = cart.GetTotal();    
            order.PromoDiscount = Utilities.CheckPromoDiscount(order.PromoCode, ndb);
            order.Total = Utilities.GetTotalPrice(cart, orderInfo.ShippingState, ndb, order.PromoDiscount);
            
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

            //Create Email Form
            var callbackUrl = Url.Action("ConfirmPayment", "Checkout", new { orderId = order.OrderId, code = code }, protocol: Request.Url.Scheme);
            EmailFormModel email = new EmailFormModel
            {
                Destination = order.Email,
                Message = "Your order number is: " + order.OrderId + "<br>Please confirm your payment by clicking <a href=\""
                        + callbackUrl + "\">here</a>"
            };

            sendPaymentConfirmationEmail(email);
        }

        private string CreatePaymentConfirmationToken()
        {
            string token = RandomString(8, true);
            return token;
        }

        private void sendPaymentConfirmationEmail(EmailFormModel model)
        {
            if (ModelState.IsValid)
            {

                var email = new MailMessage();
                email.To.Add(new MailAddress(model.Destination));
                email.From = new MailAddress("stewart_jingga@yahoo.com");
                email.Subject = "Payment Confirmation Namaskara";
                email.Body = model.Message;
                email.IsBodyHtml = true;

                using (var client = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "stewart_jingga@yahoo.com",
                        Password = "smithsog00d"
                    };
                    client.Credentials = credential;
                    client.Host = "smtp.mail.yahoo.com";
                    client.Port = 587;
                    client.EnableSsl = true;

                   
                    client.Send(email);


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