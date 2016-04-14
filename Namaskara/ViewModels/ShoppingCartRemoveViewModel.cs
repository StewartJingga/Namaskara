using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Namaskara.Models;

namespace Namaskara.ViewModels
{
    public class ShoppingCartRemoveViewModel
    {
        public string Message { get; set; }
        public string CartTotal { get; set; }
        public string CartCount { get; set; }
        public int ItemCount { get; set; }
        public int DeleteId { get; set; }
        public string Summary { get; set; }
        public bool IsEmpty { get; set; }

    }
}