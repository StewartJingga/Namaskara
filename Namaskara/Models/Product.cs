using System;
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

        public string ImageUrl { get; set; }

        [ScaffoldColumn(false)]
        public int CategoryId { get; set; }

        [ScaffoldColumn(false)]
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public List<Item> Items { get; set; }

    }
}