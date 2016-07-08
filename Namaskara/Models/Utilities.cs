using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Namaskara.Models
{
    public class Utilities
    {
        public static decimal FindReducedPrice(decimal price, double discount)
        {
            return Convert.ToDecimal(1 - discount / 100) * price;
        }
        public static decimal FindReducingPrice(decimal price, double discount)
        {
            return Convert.ToDecimal(discount / 100) * price;
        }


        public static decimal FindDeliveryCost(string state, int weight, int deliveryMethodId, NamaskaraDb ndb)//weight in gram
        {
            int weightInKilo = (int)Math.Ceiling((double)weight/1000);
            decimal pricePerKg = 0;
            switch (deliveryMethodId)
            {
                case (1):
                    pricePerKg = ndb.States.Single(m => m.StateName == state).PricePerKg;
                    break;
                case (2):
                    pricePerKg = ndb.States.Single(m => m.StateName == state).PricePerKgExpress;
                    break;
            }

            int cost = weightInKilo * (int)pricePerKg;            

            return Convert.ToDecimal(cost);
        }

        public static string FindDeliveryDays(string state, int deliveryMethodId, NamaskaraDb ndb)
        {
            try
            {
                string days = "";
                switch (deliveryMethodId)
                {
                    case (1):
                        days = ndb.States.Single(m => m.StateName == state).DeliveryDuration;
                        break;
                    case (2):
                        days = ndb.States.Single(m => m.StateName == state).DeliveryDurationExpress;
                        break;
                }
               
                return days;
            }
            catch { return null; }
        }

      

        public static void DuplicateDeliveryAddress(OrderInfo orderInfo)
        {
            orderInfo.ShippingFirstName = orderInfo.FirstName;
            orderInfo.ShippingLastName = orderInfo.LastName;
            orderInfo.ShippingPhone = orderInfo.Phone;
            orderInfo.ShippingAddress = orderInfo.Address;
            orderInfo.ShippingCity = orderInfo.City;
            orderInfo.ShippingCountry = orderInfo.Country;
            orderInfo.ShippingPostalCode = orderInfo.PostalCode;
            orderInfo.ShippingState = orderInfo.State;
        }

        public static decimal GetTotalPrice(ShoppingCart cart, string state, int deliveryMethodId, NamaskaraDb ndb, double promoDiscount = 0)
        {
            
            decimal deliveryCost = FindDeliveryCost(state, cart.GetCartWeight(), deliveryMethodId, ndb);
            decimal cartTotal = FindReducedPrice(cart.GetTotal(), promoDiscount);
            return deliveryCost + cartTotal;
        }

        public static double CheckPromoDiscount(string code, NamaskaraDb ndb)
        {
            if (ndb.PromoCodes.Any(m => m.Code == code))
            {
                return ndb.PromoCodes.Single(m => m.Code == code).Discount;
            }
            else return 0;
            
        }

        public static string CheckEnquiry(decimal price)
        {
            return price == 0 ? "Please Enquire" : String.Format("Rp {0:n}", price);
        }

        public static string CreateOrderSummaryEmail(Order order, string callbackUrl, string faqUrl)
        {
            string body = "<style>#table tr{border-bottom: 1px solid black;}#table td{padding: 6px;}.footer {font-size:11px;}</style>";

            body += "<h2>Namaste.</h2><h4>Thank You for your order. Your Order Number is: #" + order.OrderId;
            body += "<p>Here is a summary of your order:</p><br>";
            body += "<div><table id='table'><thead><tr><td>Product Name</td><td>Unit Price</td><td>Qty</td><td style=\"text-align:right;\">Total</td></tr></thead><tbody>";
            foreach (var item in order.OrderDetails)
            {
                body += "<tr><td>" + item.Item.DisplayName + "</td><td>" + String.Format("Rp. {0:n}", item.UnitPrice) + "</td><td>" + item.Quantity + "</td>";
                body += "<td style=\"text-align:right;\">" + String.Format("Rp. {0:n}", item.UnitPrice * item.Quantity) + "</td></tr>";
            }

            body += "<tr><td colspan=\"3\">Subtotal:</td><td style=\"text-align:right\">" + String.Format("Rp. {0:n}", order.Price) + "</td></tr>";
            body += "<tr><td colspan=\"3\">Delivery Cost:</td><td style=\"text-align:right\">" + String.Format("Rp. {0:n}", order.Delivery) + "</td></tr>";
            body += "<tr><td colspan=\"3\">Promo Discount:</td><td style=\"text-align:right\">" + String.Format("Rp. {0:n}", FindReducingPrice(order.Price, order.PromoDiscount)) + "</td></tr>";
            body += "<tr><td colspan=\"3\">Total:</td><td style=\"text-align:right;\">" + String.Format("Rp. {0:n}", order.Total) + "</td></tr>";
            body += "</tbody></table></div><br>";

            body += "<p>In order to process your order, please complete the payment within " + (Config.DaysToConfirm * 24) + " hours.</p>";
            body += "<p>Click <a href=\"" + callbackUrl + "\">here</a> to confirm your payment</p><br>";

            body += "<img src=\"cid:Logo\" width=400 height=150 style='margin: 0 auto;'><br>";

            body += "<p class='footer'>Log into your Namaskara Account to view the status of this order.</p>";
            body += "<p class='footer'>Please do not reply to this message. Please visit our <a href='" + faqUrl + "'>FAQ</a> for general inquiries.</p>";
            body += "<p class='footer'>If you need further assistance. please contact us at " + Config.WhatsAppNumber + " (9AM - 7PM).</p>";
            

            return body;
        }

        public static string CreatePaymentConfirmationEmail(Order order, string faqUrl)
        {
            string body = "<style>#table tr{border-bottom: 1px solid black;}#table td{padding: 6px;}.footer {font-size:11px;}</style>";

            body += "<h2>Namaste.</h2><h4>Thank You for confirming your payment. Your order is now being processed and will be dispatched to you shortly.";
            body += "<p>Here is a summary of your order:</p><br>";
            body += "<div><table id='table'><thead><tr><td>Product Name</td><td>Unit Price</td><td>Qty</td><td style=\"text-align:right;\">Total</td></tr></thead><tbody>";
            foreach (var item in order.OrderDetails)
            {
                body += "<tr><td>" + item.Item.DisplayName + "</td><td>" + String.Format("Rp. {0:n}", item.UnitPrice) + "</td><td>" + item.Quantity + "</td>";
                body += "<td style=\"text-align:right;\">" + String.Format("Rp. {0:n}", item.UnitPrice * item.Quantity) + "</td></tr>";
            }

            body += "<tr><td colspan=\"3\">Subtotal:</td><td style=\"text-align:right\">" + String.Format("Rp. {0:n}", order.Price) + "</td></tr>";
            body += "<tr><td colspan=\"3\">Delivery Cost:</td><td style=\"text-align:right\">" + String.Format("Rp. {0:n}", order.Delivery) + "</td></tr>";
            body += "<tr><td colspan=\"3\">Promo Discount:</td><td style=\"text-align:right\">" + String.Format("Rp. {0:n}", FindReducingPrice(order.Price, order.PromoDiscount)) + "</td></tr>";
            body += "<tr><td colspan=\"3\">Total:</td><td style=\"text-align:right;\">" + String.Format("Rp. {0:n}", order.Total) + "</td></tr>";
            body += "</tbody></table></div><br>";

            body += "<img src=\"cid:Logo\" width=400 height=150 style='margin: 0 auto;'><br>";

            body += "<p class='footer'>Log into your Namaskara Account to view the status of this order.</p>";
            body += "<p class='footer'>Please do not reply to this message. Please visit our <a href='" + faqUrl + "'>FAQ</a> for general inquiries.</p>";
            body += "<p class='footer'>If you need further assistance. please contact us at " + Config.WhatsAppNumber + " (9AM - 7PM).</p>";


            return body;
        }

        public static string CreateResetPasswordEmail(string callbackUrl, string faqUrl)
        {
            string body = "<style>#table tr{border-bottom: 1px solid black;}#table td{padding: 6px;}.footer {font-size:11px;}</style>";

            body += "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>";

            body += "<img src=\"cid:Logo\" width=400 height=150 style='margin: 0 auto;'><br>";

            body += "<p class='footer'>Log into your Namaskara Account to view the status of this order.</p>";
            body += "<p class='footer'>Please do not reply to this message. Please visit our <a href='" + faqUrl + "'>FAQ</a> for general inquiries.</p>";
            body += "<p class='footer'>If you need further assistance. please contact us at " + Config.WhatsAppNumber + " (9AM - 7PM).</p>";


            return body;
        }

        public static string CreateEnquiryEmail(string name, string email, string message)
        {
            string body = "From : " + name;
            body += "<br>Email : " + email;
            body += "<br><br>" + message;

            return body;
        }

    }
}