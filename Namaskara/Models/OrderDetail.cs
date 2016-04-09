using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Namaskara.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        [Display(Name = "Price per Unit")]
        public decimal UnitPrice { get; set; }
        public Item Item { get; set; }
        public Order Order { get; set; }

    }
}