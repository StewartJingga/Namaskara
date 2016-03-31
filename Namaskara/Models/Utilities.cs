using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Namaskara.Models
{
    public class Utilities
    {
        public static decimal FindReducedPrice(decimal price, double discount)
        {
            return Convert.ToDecimal(1 - discount / 100) * price;
        }

        public static decimal FindDeliveryCost(string city, int weight)//weight in gram
        {
            int weightInKilo = (int)Math.Ceiling((double)weight/1000);
            
            int deliveryCost;

            switch (city)
            {
                case ("Surabaya"):
                    deliveryCost = 17000 * weightInKilo;
                    break;
                case ("Medan"):
                    deliveryCost = 26000 * weightInKilo;
                    break;
                case ("Jakarta"):
                    deliveryCost = 10000 * weightInKilo;
                    break;
                case ("Makassar"):
                    deliveryCost = 31000 * weightInKilo;
                    break;
                
                default:
                    deliveryCost = 10000 * weightInKilo;
                    break;
            }

            return Convert.ToDecimal(deliveryCost);
        }

        public static void DuplicateDeliveryAddress(OrderInfo orderInfo)
        {
            orderInfo.ShippingAddress = orderInfo.Address;
            orderInfo.ShippingCity = orderInfo.ShippingCity;
            orderInfo.ShippingCountry = orderInfo.ShippingCountry;
            orderInfo.ShippingPostalCode = orderInfo.ShippingPostalCode;
            orderInfo.ShippingState = orderInfo.State;
        }

        public static decimal GetTotalPrice(ShoppingCart cart, string city, double promoDiscount = 0)
        {
            
            decimal deliveryCost = FindDeliveryCost(city, cart.GetCartWeight());
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

        
    }
}