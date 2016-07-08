using Namaskara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Namaskara.ViewModels
{
    public class CreateProductViewModel
    {
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Origin { get; set; }
        public string Description { get; set; }
        public PackagingEnum Packaging { get; set; }
    }
}