using Namaskara.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Namaskara.ViewModels
{
    public class UploadImageModel
    {
        [Display(Name = "Order ID")]
        public int OrderId { get; set; }
        public HttpPostedFileBase PaymentImage { get; set; }
        public string Bank { get; set; }
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }
        [Display(Name = "Amount Paid (Rp)")]
        [RegularExpression("[0-9]+", ErrorMessage = "Please enter number only")]
        public decimal AmountPaid { get; set; }
        public string OrderTotal { get; set; }


    }
}