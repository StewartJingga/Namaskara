using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Namaskara.ViewModels
{
    public class UpdateItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal RetailPrice { get; set; }
        public bool IsAvailable { get; set; }
    }
}