using Namaskara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Namaskara.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(FormCollection model)
        {

            if (ModelState.IsValid)
            {

                var email = new MailMessage();
                email.To.Add(new MailAddress(Config.Email));
                email.From = new MailAddress(Config.Email);
                email.Subject = model["Subject"];
                email.Body = Utilities.CreateEnquiryEmail(model["Name"], model["Email"], model["Message"]);
                email.IsBodyHtml = true;

                using (var client = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = Config.Email,
                        Password = Config.Password
                    };
                    client.UseDefaultCredentials = false;// disable it
                    client.Credentials = credential;
                    client.Host = Config.SmtpHost;
                    client.Port = Config.SmtpPort;
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    


                    client.Send(email);

                    ViewBag.Message = "Message has been successfully sent.";

                }
            }
            else
            {
                ViewBag.Message = "Failed to send message";
            }

            

            return View();
        }

        public ActionResult TermsAndConditions()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult HowToOrder()
        {
            return View();
        }

        public ActionResult ExchangePolicy()
        {
            return View();
        }
    }
}