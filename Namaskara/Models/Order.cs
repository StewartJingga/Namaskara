using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Namaskara.Models
{
    public class Order
    {
        [Display(Name = "Order ID")]
        public int OrderId { get; set; }
        [ScaffoldColumn(false)]
        public int OrderInfoId { get; set; }
        public string Email { get; set; }
        [Display(Name = "Price (Rp)")]
        public decimal Price { get; set; }
        [Display(Name = "Delivery (Rp)")]
        public decimal Delivery { get; set; }
        [Display(Name = "Total (Rp)")]
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string PaymentImage { get; set; }
        public string Bank { get; set; }
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }
        [Display(Name = "Amount Paid (Rp)")]
        public decimal AmountPaid { get; set; }
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime ConfirmDate { get; set; }
        public OrderInfo OrderInfo { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

    }
}