using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Namaskara.Models
{
    public class State
    {
        [Key]
        public int StateId { get; set; }
        public string StateName { get; set; }
        public decimal PricePerKg { get; set; }
        public string DeliveryDuration { get; set; }
        public decimal PricePerKgExpress { get; set; }
        public string DeliveryDurationExpress { get; set; }
    }
}