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
                email.To.Add(new MailAddress("stewart_jingga@yahoo.com"));
                email.From = new MailAddress("stewart_jingga@yahoo.com");
                email.Subject = "Yow";
                email.Body = Utilities.CreateEnquiryEmail(model["Name"], model["Email"], model["Message"]);
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

            ViewBag.Message = "Email has been successfully sent.";

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