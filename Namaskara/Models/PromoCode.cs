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
        public int Id { get; set; }
        public string Code { get; set; }
        [RegularExpression("[0-9]+", ErrorMessage = "Enter the number only")]
        [Range(0, 100, ErrorMessage = "Enter number from 0 to 100")]
        public double Discount { get; set; }
    }
}