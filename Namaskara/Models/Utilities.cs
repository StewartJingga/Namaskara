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


        public static decimal FindDeliveryCost(string state, int weight, NamaskaraDb ndb)//weight in gram
        {
            int weightInKilo = (int)Math.Ceiling((double)weight/1000);

            int cost = weightInKilo * (int)ndb.States.Single(m => m.StateName == state).PricePerKg;            

            return Convert.ToDecimal(cost);
        }

        public static void DuplicateDeliveryAddress(OrderInfo orderInfo)
        {
            orderInfo.ShippingAddress = orderInfo.Address;
            orderInfo.ShippingCity = orderInfo.City;
            orderInfo.ShippingCountry = orderInfo.Country;
            orderInfo.ShippingPostalCode = orderInfo.PostalCode;
            orderInfo.ShippingState = orderInfo.State;
        }

        public static decimal GetTotalPrice(ShoppingCart cart, string state, NamaskaraDb ndb, double promoDiscount = 0)
        {
            
            decimal deliveryCost = FindDeliveryCost(state, cart.GetCartWeight(), ndb);
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