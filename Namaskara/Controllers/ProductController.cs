using Namaskara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Namaskara.Controllers
{
    
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        NamaskaraDb ndb = new NamaskaraDb();

        public ActionResult CategoryIndex()
        {
            return View(ndb.Categories.ToList());
        }
        // GET: Product
        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(Category model)
        {
            if (ModelState.IsValid)
            {
                Category cat = new Category();
                TryUpdateModel(cat);
                ndb.Categories.Add(cat);
                ndb.SaveChanges();

                return RedirectToAction("CategoryIndex");
            }
            else return View();
            
            
        }

        public ActionResult EditCategory(int id)
        {
            return View(ndb.Categories.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(Category model)
        {
            if (ModelState.IsValid)
            {
                ndb.Entry(model).State = System.Data.Entity.EntityState.Modified;
                ndb.SaveChanges();

                return RedirectToAction("CategoryIndex");
            }
            else return View(model);

        }

        public ActionResult ProductIndex()
        {
            return View(ndb.Products.ToList());
        }

        public ActionResult ProductDetails(int id)
        {
            Product model = ndb.Products.Find(id);
            model.Items = ndb.Items.Where(m => m.ProductId == id).ToList();
            return View(model);
        }

        public ActionResult CreateProduct()
        {
            
            ViewBag.Categories = new SelectList(ndb.Categories, "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(Product model)
        {
            
            if (ModelState.IsValid)
            {
                bool exist = ndb.Products.Any(m => m.Name == model.Name);
                if (exist)
                {
                    ViewBag.Categories = new SelectList(ndb.Categories, "CategoryId", "Name");
                    return View();
                }

                Product prod = new Product();
                TryUpdateModel(prod);
                prod.IsAvailable = false;
                prod.Category = null;
                ndb.Products.Add(prod);
                ndb.SaveChanges();

                return RedirectToAction("ProductIndex");
            }
            else
            {
                ViewBag.Categories = new SelectList(ndb.Categories, "CategoryId", "Name");
                return View();
            }
                

        }

        public ActionResult EditProduct(int id)
        {
            return View(ndb.Products.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product model)
        {
            if (ModelState.IsValid)
            {
                ndb.Entry(model).State = System.Data.Entity.EntityState.Modified;
                ndb.SaveChanges();

                return RedirectToAction("ProductIndex");

            }
            else return View(model);
        }

        public ActionResult CreateItem(int id)
        {
            Product prod = ndb.Products.Find(id);
            
            return View(prod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateItem(CreateItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                Item item = new Item();
                item.Size = model.Number + " " + model.Unit;
                item.Name = model.Name;
                item.ProductId = model.ProductId;
                item.RetailPrice = model.RetailPrice;
                item.Product = null;
                ndb.Items.Add(item);
                ndb.SaveChanges();

                return RedirectToAction("ProductDetails", new { id = item.ProductId});
            }

            else
            {
                ViewBag.Unit = new SelectList(Config.UnitList);
                return View(model);
            }
                 
        }


    }
}