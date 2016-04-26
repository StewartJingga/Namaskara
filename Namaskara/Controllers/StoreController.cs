using Namaskara.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            List<Product> products = ndb.Products.Where(m => m.IsFeatured == true).ToList();
            foreach (var product in products)
            {
                product.Items = ndb.Items.Where(m => m.ProductId == product.ProductId).OrderByDescending(m => m.Id).ToList();
                product.Category = ndb.Categories.Find(product.CategoryId);
            }
            return View();

        }

        public ActionResult ProductIndex(int id=0, string src = null)
        {
            List<Product> products;
            //If link brings you here
            if(src == null && string.IsNullOrWhiteSpace(src))
            {
                switch (id)
                {
                    case (0):
                        products = ndb.Products.Where(m => m.IsFeatured == true).ToList();
                        break;
                    default:
                        products = ndb.Products.Where(m => m.CategoryId == id).ToList();
                        break;
                }

                foreach (var product in products)
                {
                    product.Items = ndb.Items.Where(m => m.ProductId == product.ProductId && m.IsAvailable == true).OrderByDescending(m => m.Id).ToList();
                    product.Category = ndb.Categories.Find(product.CategoryId);
                }
                ViewData["CatName"] = id == 0 ? "Featured Products" : products[0].Category.Name;
            }
            else//If search brings you here
            {
                try
                {
                    products = ndb.Products.Where(m => m.Name.Contains(src)).ToList();
                    foreach (var product in products)
                    {
                        product.Items = ndb.Items.Where(m => m.ProductId == product.ProductId && m.IsAvailable == true).OrderByDescending(m => m.Id).ToList();
                        product.Category = ndb.Categories.Find(product.CategoryId);
                    }
                    ViewData["CatName"] = "Search Results for \""+ src +"\"";
                }
                catch
                {
                    products = null;
                    ViewData["CatName"] = "Can't find products";
                }
            }
            if (products != null) products = products.OrderBy(m => m.Name).ToList();
            return View(products);
        }

        public ActionResult Details(int id)
        {
            Product product = ndb.Products.Find(id);
            product.Items = ndb.Items.Where(m => m.ProductId == id && m.IsAvailable == true).ToList();
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
            product.Items = ndb.Items.Where(m => m.ProductId == id && m.IsAvailable == true).OrderByDescending(m => m.Id).ToList();
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
        public ActionResult SearchBox(string searchString)
        {
            List<Product> result;
            try
            {
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    result = ndb.Products.Where(m => m.Name.Contains(searchString)).Take(5).ToList();
                    
                    Debug.Print(result.Count.ToString());

                }
                else result = null;
            }
            catch
            {
                result = null;
            }

            return Json(result);
        }
    }
}