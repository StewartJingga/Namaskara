﻿using System;
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
                cart.Item = ndb.Items.Find(cart.ItemId);
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
            decimal? total = (from cartItems in ndb.CartItems
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count * cartItems.Item.RetailPrice).Sum();

            return total ?? decimal.Zero;
        }
        
        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = GetCartItems();

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ItemId = item.ItemId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Item.RetailPrice,
                    Quantity = item.Count
                };

                orderTotal += (item.Count * item.Item.RetailPrice);
                ndb.OrderDetails.Add(orderDetail);
            }

            order.Total = orderTotal;
            ndb.Entry(order).State = System.Data.Entity.EntityState.Modified;

            ndb.SaveChanges();

            EmptyCart();

            return order.OrderId;
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