using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Namaskara.Models;
using Namaskara.ViewModels;

namespace Namaskara.Controllers
{
    public class ShoppingCartController : Controller
    {
        NamaskaraDb ndb = new NamaskaraDb();
        // GET: ShoppingCart
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            //Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()

            };
            return View(viewModel);
        }


        public ActionResult AddToCart(int id, int count)
        {
            var addedItem = ndb.Items.Single(item => item.Id == id);

            var cart = ShoppingCart.GetCart(this.HttpContext);

            for (int i = 0; i < count; i++)
            {
                cart.AddToCart(addedItem);
            }
            var results = new ShoppingCartAddModel
            {
                Message = addedItem.Name + " - " + addedItem.Size + " " + count.ToString() + "x has been added to cart",
                
                
            };
            return Json(results);
        }

        [HttpPost]
        public ActionResult UpdateCartItem(int id, int count)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            CartItem cartItem = ndb.CartItems.Include("Item").Single(m => m.RecordId == id);
            if (count == 0) ndb.CartItems.Remove(cartItem);
            else
            {
                cartItem.Count = count;
                ndb.Entry(cartItem).State = System.Data.Entity.EntityState.Modified;
            }
            
            ndb.SaveChanges();

            cartItem.Item.Product = ndb.Products.Find(cartItem.Item.ProductId);
            decimal itemPrice = cartItem.Item.Product.IsOnSale ? Utilities.FindReducedPrice(cartItem.Item.RetailPrice, cartItem.Item.Product.DiscountPercentage) : cartItem.Item.RetailPrice;

            var results = new ShoppingCartRemoveViewModel
            {
                Message = count == 0 ? "Item has been removed from your shopping cart." : "Item has been updated.",
                CartTotal = String.Format("Rp {0:n}", cart.GetTotal()),
                ItemCount = count,
                CartCount = String.Format("Rp {0:n}", itemPrice * count),
                DeleteId = id
            };
            return Json(results);
        }

        [HttpPost]
        public ActionResult DeleteFromCart(int id)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            CartItem cartItem = ndb.CartItems.Single(m => m.RecordId == id);
            ndb.CartItems.Remove(cartItem);
            ndb.SaveChanges();

            var items = cart.GetCartItems();


            var results = new ShoppingCartRemoveViewModel
            {
                Message =  "Item has been removed from your shopping cart.",
                CartTotal = String.Format("Rp {0:n}", cart.GetTotal()),
                Summary = String.Format("{0} Items // Rp {1:n}", items.Count, cart.GetTotal()),
                IsEmpty = (items.Count == 0),
                DeleteId = cartItem.RecordId
            };
            return Json(results);
        }


        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()

            };

            ViewData["CartCount"] = cart.GetCount();
            
            return PartialView(viewModel);
        }
    }
}