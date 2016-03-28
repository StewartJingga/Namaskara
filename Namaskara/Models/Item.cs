using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

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
       
       
        
       
        

    }
}