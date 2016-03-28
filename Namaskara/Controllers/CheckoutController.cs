using System;
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
            //If user has created an order information
            if(ndb.OrderInformation.Any(m => m.Email == User.Identity.Name))
            {
                return View(ndb.OrderInformation.Single(m => m.Email == User.Identity.Name));
            }
            else return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddressAndPayment(FormCollection values)
        {

            OrderInfo orderInfo = new OrderInfo();
            TryUpdateModel(orderInfo);
            orderInfo.Email = User.Identity.Name;
            Order order = new Order();

            if (!ndb.OrderInformation.Any(m => m.Email == User.Identity.Name))
            {
                ndb.OrderInformation.Add(orderInfo);
                ndb.SaveChanges();
            }
            
            try
            {
                order.OrderInfoId = orderInfo.OrderInfoId;
                order.Email = orderInfo.Email;
                order.OrderDate = DateTime.Now;
                order.Status = "Order Submitted";
                
                ProcessOrder(order);

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
            return View();
            
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AnonymousAddressAndPayment(FormCollection values)
        {
            OrderInfo orderInfo = new OrderInfo();
            TryUpdateModel(orderInfo);
            Order order = new Order();
  
            try
            {
                order.OrderInfoId = orderInfo.OrderInfoId;
                order.Email = orderInfo.Email;
                order.OrderDate = DateTime.Now;
                order.Status = "Order Submitted";

                ProcessOrder(order);

                return RedirectToAction("Complete", new { id = order.OrderId });
            }
            catch
            {
                return View(orderInfo);
            }
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
                TimeSpan difference = DateTime.Now - order.OrderDate;
                if (difference.TotalHours <= 24)
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

                if (PaymentImage.ContentLength > 0)
                {
                    var fileName = "OrderId=" + model.OrderId.ToString() + " " + Path.GetFileName(PaymentImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/PaymentImages/"), fileName);
                    PaymentImage.SaveAs(path);
                    
                    order.PaymentImage = fileName;
                    order.Status = "Waiting For Confirmation";
                }
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
        private void ProcessOrder(Order order)
        {
            

            //Save Order
            ndb.Orders.Add(order);
            ndb.SaveChanges();

            //Process the order
            var cart = ShoppingCart.GetCart(this.HttpContext);
            cart.CreateOrder(order);

            //Create Payment Confirmation token and send it
            string code = CreatePaymentConfirmationToken();

            code = System.Web.HttpUtility.UrlEncode(code);
            PaymentConfirmation pc = new PaymentConfirmation { OrderId = order.OrderId, Code = code };

            ndb.PaymentConfirmations.Add(pc);
            ndb.SaveChanges();

            var callbackUrl = Url.Action("ConfirmPayment", "Checkout", new { orderId = order.OrderId, code = code }, protocol: Request.Url.Scheme);
            EmailFormModel email = new EmailFormModel
            {
                Destination = order.Email,
                Message = "Please confirm your payment by clicking <a href=\""
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