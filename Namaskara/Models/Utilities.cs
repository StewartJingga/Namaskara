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
    }
}