using Namaskara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Namaskara.Controllers
{
    public class StoreController : Controller
    {
        NamaskaraDb ndb = new NamaskaraDb();
        // GET: Store
        
        public ActionResult Index()
        {
            return View();

            
        }

        public ActionResult ProductIndex(int id=1)
        {
            List<Product> products = ndb.Products.Where(m => m.CategoryId == id).ToList();
            foreach(var product in products)
            {
                product.Items = ndb.Items.Where(m => m.ProductId == product.ProductId).OrderByDescending(m => m.Id).ToList();
                product.Category = ndb.Categories.Find(product.CategoryId);
            }
            ViewData["CatName"] = products[0].Category.Name;
            return View(products);
        }

        public ActionResult Details(int id)
        {
            Product product = ndb.Products.Find(id);
            product.Items = ndb.Items.Where(m => m.ProductId == id).ToList();
            return View(product);
        }

        [ChildActionOnly]
        public ActionResult CategoryList()
        {
            List<Category> categories = ndb.Categories.ToList();
            return PartialView(categories);
        }

        public ActionResult DetailsPartial(int id)
        {
            Product product = ndb.Products.Find(id);
            product.Items = ndb.Items.Where(m => m.ProductId == id).OrderByDescending(m => m.Id).ToList();
            return PartialView(product);
        }

        [HttpPost]
        public string UpdatePrice(int id)
        {
            Item item = ndb.Items.Include("Product").Single(m => m.Id == id);
            if (item.Product.IsOnSale)
            {
                return "<span class='old-price'>" + @String.Format("Rp {0:n}", item.RetailPrice) + "</span> <span class='glyphicon glyphicon-arrow-right'></span>" +
                       "<span class='new-price'>" + @String.Format("Rp {0:n}", Utilities.FindReducedPrice(item.RetailPrice, item.Product.DiscountPercentage)) + "</span>";
            }
            else
            {
                return @String.Format("Rp {0:n}", item.RetailPrice);
            }
        }

        [HttpPost]
        public string AddToWishlist(int id)
        {
            if (!User.Identity.IsAuthenticated) return Config.WishlistFailed;
            else
            {
                try
                {
                    
                    List<Item> items = ndb.Items.Where(m => m.ProductId == id).OrderByDescending(m => m.Id).ToList();
                    ndb.Wishlists.Add(new WishList
                    {
                        ItemId = items[0].Id,
                        UserEmail = User.Identity.Name,
                        DateListed = DateTime.Now,
                        Item = null
                    });

                    ndb.SaveChanges();

                    return Config.WishlistSuccess;
                }
                catch
                {
                    return "Error inputting wishlist";
                }
                
            }
        }

        [HttpPost]
        public ActionResult SearchBox(string searchString)
        {
            List<Product> result;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                result = ndb.Products.Where(m => m.Name.Contains(searchString)).ToList();
            }
            else result = null;

            return Json(result);
        }
    }
}