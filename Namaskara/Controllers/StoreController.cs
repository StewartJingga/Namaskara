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

        public ActionResult ProductIndex(int id, string cat)
        {
            List<Product> products = ndb.Products.Where(m => m.CategoryId == id).ToList();
            ViewData["CatName"] = cat;
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
    }
}