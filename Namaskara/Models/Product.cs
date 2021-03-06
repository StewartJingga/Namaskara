﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Namaskara.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Origin { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public PackagingEnum Packaging { get; set; }
        public bool IsOnSale { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsVegan { get; set; }
        public bool IsLactoseFree { get; set; }
        public bool IsNutFree { get; set; }

        [Range(0, 100.00, ErrorMessage ="Discount must be between 0 to 100")]
        public double DiscountPercentage { get; set; }
        [ScaffoldColumn(false)]
        public int CategoryId { get; set; }

        [ScaffoldColumn(false)]
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public List<Item> Items { get; set; }

        public Product()
        {
            this.IsAvailable = true;
            this.Packaging = PackagingEnum.Nothing;
        }

    }
}