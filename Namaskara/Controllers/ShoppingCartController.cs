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
        public ActionResult RemoveFromCart(int id)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            CartItem cartItem = ndb.CartItems.Single(m => m.RecordId == id);
            string itemName = ndb.Items.Single(m => m.Id == cartItem.ItemId).DisplayName;

            int itemCount = cart.RemoveFromCart(id);

            var results = new ShoppingCartRemoveViewModel
            {
                Message = Server.HtmlEncode(itemName) + " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }

        
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            ViewData["CartCount"] = cart.GetCount();
            return PartialView("CartSummary");
        }
    }
}