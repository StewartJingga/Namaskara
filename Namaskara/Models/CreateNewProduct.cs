using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Namaskara.Models
{
    public class CreateNewProduct
    {
        //new Item { Name = "Alfalfa Seed Natural", Size = "150 g", RetailPrice = 61750M, IsAvailable = true, Product = Products.Single(m => m.Name == "Alfalfa Seed Natural") },
        //new Product { Name =  "Alfalfa Seed Natural", Category = Categories.Single(c => c.Name == "Seeds & Grains"), ImageUrl = "", Origin = ""},
        public string Name { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public string Origin { get; set; }
        public int Sizes { get; set; }

    }
}