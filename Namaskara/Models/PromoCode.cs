using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Namaskara.Models
{
    public class PromoCode
    {
        [Key]
        public string Code { get; set; }
        public double Discount { get; set; }
    }
}