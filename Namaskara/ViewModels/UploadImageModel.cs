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
        
        public int OrderId { get; set; }
        [Required]
        public HttpPostedFileBase PaymentImage { get; set; }
        public string Bank { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }

    }
}