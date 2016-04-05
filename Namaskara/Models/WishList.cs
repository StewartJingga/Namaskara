using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Namaskara.Models
{
    public class WishList
    {
        [Key]
        public int WishListId { get; set; }
        public string UserEmail { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public DateTime DateListed { get; set; }
    }
}