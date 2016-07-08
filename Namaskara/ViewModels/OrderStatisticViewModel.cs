using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Namaskara.ViewModels
{
    public class OrderStatisticViewModel
    {
        [Display(Name = "Best Selling Item")]
        public string BestSellingItem { get; set; }
        [Display(Name = "Total Number of Purchase")]
        public int TotalNumberOfPurchase { get; set; }
        [Display(Name = "Total Item Purchase (Rp)")]
        public decimal TotalItemPurchase { get; set; }
        [Display(Name = "Total Delivery Cost (Rp)")]
        public decimal TotalDeliveryCost { get; set; }
        [Display(Name = "Total Cost (Rp)")]
        public decimal TotalCost { get; set; }
        

    }
}