using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Namaskara.Models
{
    public class OrderInfo
    {
        [ScaffoldColumn(false)]
        [Key]
        public int OrderInfoId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        
        [Display(Name = "Confirm Email")]
        [Compare("Email", ErrorMessage = "The email and confirmation email do not match.")]
        public string ConfirmEmail { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        [Display(Name = "First Name")]
        public string ShippingFirstName { get; set; }
        [Display(Name = "Last Name")]
        public string ShippingLastName { get; set; }
        [Display(Name = "Phone")]
        public string ShippingPhone { get; set; }
        [Display(Name = "Address")]
        public string ShippingAddress { get; set; }
        [Display(Name = "City")]
        public string ShippingCity { get; set; }
        [Display(Name = "State")]
        public string ShippingState { get; set; }
        [Display(Name = "Postal Code")]
        public string ShippingPostalCode { get; set; }
        [Display(Name = "Country")]
        public string ShippingCountry { get; set; }

    }
}