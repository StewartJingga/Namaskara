using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Namaskara.ViewModels
{
    public class UpdateProductViewModel
    {
        public int ProductId { get; set; }
        public string Origin { get; set; }

        public bool IsOnSale { get; set; }
        public bool IsFeatured { get; set; }
        public double DiscountPercentage { get; set; }

    }
}