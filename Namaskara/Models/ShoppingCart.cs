using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Namaskara.Models
{
    public class ShoppingCart
    {
        NamaskaraDb ndb = new NamaskaraDb();
        
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        public void AddToCart(Item item)
        {
            var cartItem = ndb.CartItems.SingleOrDefault(c => c.CartId == ShoppingCartId && c.ItemId == item.Id);

            if (cartItem == null)
            {
                // Create new item if item has not been created
                cartItem = new CartItem
                {
                    ItemId = item.Id,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                ndb.CartItems.Add(cartItem);
            }
            else
            {
                //If item exists in the cart, increase the count
                cartItem.Count++;
            }
            ndb.SaveChanges();
        }

        public int RemoveFromCart(int id)
        {
            //Get the cart
            var cartItem = ndb.CartItems.Single(cart => cart.CartId == ShoppingCartId && cart.RecordId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    ndb.CartItems.Remove(cartItem);
                }
                ndb.SaveChanges();
            }
            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = ndb.CartItems.Where(cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                ndb.CartItems.Remove(cartItem);
            }

            ndb.SaveChanges();
        }

        public List<CartItem> GetCartItems()
        {
            List<CartItem> cartItems = ndb.CartItems.Where(c => c.CartId == ShoppingCartId).ToList();
            foreach (CartItem cart in cartItems)
            {
                cart.Item = ndb.Items.Include("Product").Single(m => m.Id == cart.ItemId);
            }
            return cartItems;
        }
        
        public int GetCount()
        {
            var cartItems = ndb.CartItems.Where(c => c.CartId == ShoppingCartId);

            int? count = 0;
            foreach (var cartItem in cartItems)
            {
                count += cartItem.Count;
            }
            return count?? 0;
        }
        
        public decimal GetTotal()
        {

            List<CartItem> cartItems = ndb.CartItems.Where(m => m.CartId == ShoppingCartId).ToList();
            decimal? total = 0;
            decimal price;
            foreach(var cartItem in cartItems)
            {
                cartItem.Item = ndb.Items.Include("Product").Single(m => m.Id == cartItem.ItemId);

                price = cartItem.Item.Product.IsOnSale ? Utilities.FindReducedPrice(cartItem.Item.RetailPrice, cartItem.Item.Product.DiscountPercentage) : cartItem.Item.RetailPrice;

                total += price * cartItem.Count;
            }
            

            return total ?? decimal.Zero;
        }
        
        public decimal CreateOrder(int orderId)
        {
            
            decimal orderTotal = 0;

            var cartItems = GetCartItems();
            

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ItemId = item.ItemId,
                    OrderId = orderId,
                    UnitPrice = item.Item.Product.IsOnSale ? Utilities.FindReducedPrice(item.Item.RetailPrice, item.Item.Product.DiscountPercentage) : item.Item.RetailPrice,
                    Quantity = item.Count
                };

                orderTotal += (item.Count * orderDetail.UnitPrice);
                orderDetail.Item = null;
                ndb.OrderDetails.Add(orderDetail);
            }
            
            
            //ndb.Entry(order).State = System.Data.Entity.EntityState.Modified;

            ndb.SaveChanges();
            EmptyCart();


            return orderTotal;
        }
        
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] = context.User.Identity.Name;
                }
                else
                {
                    Guid tempCartId = Guid.NewGuid();
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();
        }
        
        public void MigrateCart(string username)
        {
            var shoppingCart = ndb.CartItems.Where(c => c.CartId == ShoppingCartId);

            foreach (CartItem item in shoppingCart)
            {
                item.CartId = username;
            }

            ndb.SaveChanges();
        } 
    }
}