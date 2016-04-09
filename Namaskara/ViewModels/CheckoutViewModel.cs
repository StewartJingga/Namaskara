using Namaskara.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Namaskara.ViewModels
{
    public class CheckoutViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Confirm Email")]
        [Compare("Email", ErrorMessage = "The email and confirmation email do not match.")]
        public string ConfirmEmail { get; set; }
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Please choose a city")]
        public string City { get; set; }
        
        [Required(ErrorMessage = "Please choose a state")]
        public string State { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Required(ErrorMessage = "Please pick a country")]
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
        
        [Required(ErrorMessage = "Please choose a city")]
        [Display(Name = "City")]
        public string ShippingCity { get; set; }
        
        [Required(ErrorMessage = "Please choose a state")]
        [Display(Name = "State")]
        public string ShippingState { get; set; }
        [Display(Name = "Postal Code")]
        public string ShippingPostalCode { get; set; }
        [Required(ErrorMessage = "Please choose a city")]
        [Display(Name = "Country")]
        public string ShippingCountry { get; set; }
        [Display(Name = "Same Delivery Address")]
        public bool SameDeliveryAddress { get; set; }
        public List<CartItem> CartItems { get; set; }
        public string CartTotalPrice { get; set; }
        public string PromoCode { get; set; }
        public bool PromoActivated { get; set; }

    }
}