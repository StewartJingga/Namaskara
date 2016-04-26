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

        public static string CreateOrderSummaryEmail(Order order, string callbackUrl)
        {
            string body = "<h2>Thank You for purchasing at our store!</h2><h4>Your order has been submitted.</h4><p>Please review your order</p><br />";
            body += "<p>Please confirm your payment by clicking <a href=\"" + callbackUrl + "\">here</a></p>";
            
            body += "<h3>Your Order number is " + order.OrderId + "</h3>";
            body += "<div><table><thead><tr><td>Product Name</td><td>Unit Price</td><td>Qty</td><td>Total</td></tr></thead><tbody>";
            foreach (var item in order.OrderDetails)
            {
                body += "<tr><td>" + item.Item.DisplayName + "</td><td>" + item.UnitPrice + "</td><td>" + item.Quantity + "</td>";
                body += "<td>" + item.UnitPrice * item.Quantity + "</td></tr>";
            }

            body += "<tr><td colspan=\"3\">Subtotal:</td><td style=\"text-align:right\">" + order.Price + "</td></tr>";
            body += "<tr><td colspan=\"3\">Delivery Cost:</td><td style=\"text-align:right\">" + String.Format("{0}.00", order.Delivery) + "</td></tr>";
            body += "<tr><td colspan=\"3\">Promo Discount:</td><td style=\"text-align:right\">" + FindReducingPrice(order.Price, order.PromoDiscount) + "</td></tr>";
            body += "<tr><td colspan=\"3\">Total:</td><td style=\"text-align:right\">" + order.Total + "</td></tr>";
            body += "</tbody></table></div>";

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