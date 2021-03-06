﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Namaskara.Models
{
    [Bind(Exclude = "Id")]
    public class Item
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [ScaffoldColumn(false)]
        public int ProductId { get; set; }
        [ScaffoldColumn(false)]
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        [Required]
        [DisplayName("Item Name")]
        [ScaffoldColumn(false)]
        public string Name { get; set; }
        
        public string Size { get; set; }
        [DisplayName("Item Name")]
        public string DisplayName
        {
            get
            {
                return Name + " - " + Size;
            }
        }
        

        public bool IsAvailable { get; set; }
        [DisplayName("Retail Price")]
        public decimal RetailPrice { get; set; }
        public int Weight
        {
            get
            {
                string weight = Regex.Replace(Size, @"[^0-9]", "");
                int weightInInt = (this.Unit == "kg" || this.Unit == "ltr") ? Int32.Parse(weight) * 1000 : Int32.Parse(weight);

                return weightInInt + Config.PackagingInGrams[this.Product.Packaging];
            }
        }
        public string Unit
        {
            get
            {
                return Regex.Replace(Size, @"[^a-z]", "");
            }
        }

        






    }
}