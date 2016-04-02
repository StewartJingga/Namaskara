using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Namaskara.Models
{
    public class City
    {
        [Key]
        public string CityName { get; set; }
        public int StateId { get; set; }
        public State State { get; set; }
    }
}