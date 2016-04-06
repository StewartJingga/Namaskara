using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Namaskara.Models
{
    public class NamaskaraDb : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderInfo> OrderInformation { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PaymentConfirmation> PaymentConfirmations { get; set; }
        public DbSet<UserInformation> UserInformations { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }

        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }

        public DbSet<WishList> Wishlists { get; set; }
        

    }
}