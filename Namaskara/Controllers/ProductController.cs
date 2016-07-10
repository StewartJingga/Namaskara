using Namaskara.Models;
using Namaskara.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
            return View(ndb.Products.OrderBy(m => m.Name).ToList());
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
        public ActionResult CreateProduct(CreateProductViewModel model, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid) //&& )
            {
                bool exist = ndb.Products.Any(m => m.Name == model.Name);

                if(exist)
                {
                    ViewBag.Categories = new SelectList(ndb.Categories, "CategoryId", "Name");
                    ViewBag.Error = "Product with the same name is already existed. Please choose different name.";
                    return View(model);
                }

                Product prod = new Product();
                TryUpdateModel(prod);
                prod.IsAvailable = false;
                prod.Category = null;

                if (Image != null && Image.ContentLength > 0)
                {
                    string imageName = model.Name + "-img";
                    var path = Path.Combine(Server.MapPath("~/Images"), imageName + ".jpg");
                    Image.SaveAs(path);
                    prod.ImageUrl = imageName;
                }

                ndb.Products.Add(prod);
                ndb.SaveChanges();

                return RedirectToAction("ProductIndex");
            }
            else
            {
                ViewBag.Categories = new SelectList(ndb.Categories, "CategoryId", "Name");
                ViewBag.Error = "Create Product Failed";
                return View(model);
            }
                

        }

        public ActionResult EditProduct(int id)
        {
            ViewBag.Categories = new SelectList(ndb.Categories, "CategoryId", "Name");
            return View(ndb.Products.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(CreateProductViewModel model, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                Product product = ndb.Products.Find(model.ProductId);

                bool exist = ndb.Products.Any(m => m.Name == model.Name);

                if (exist && product.Name != model.Name)
                {
                    ViewBag.Categories = new SelectList(ndb.Categories, "CategoryId", "Name");
                    ViewBag.Error = "Product with the same name is already existed. Please choose different name.";

                    Product prod = new Product();
                    TryUpdateModel(prod);
                    return View(prod);
                }

                
                product.Name = model.Name;
                product.Origin = model.Origin;
                product.Description = model.Description;
                product.CategoryId = model.CategoryId;
                product.Packaging = model.Packaging;


                if (Image != null && Image.ContentLength > 0)
                {
                    //Delete the old image
                    DeleteImage(product.ImageUrl);

                    string imageName = model.Name + "-img";
                    var path = Path.Combine(Server.MapPath("~/Images"), imageName + ".jpg");
                    Image.SaveAs(path);
                    
                    //Add the new ImageURL
                    product.ImageUrl = imageName;
                }

                ndb.Entry(product).State = System.Data.Entity.EntityState.Modified;
                ndb.SaveChanges();

                return RedirectToAction("ProductIndex");

            }
            else {
                ViewBag.Categories = new SelectList(ndb.Categories, "CategoryId", "Name");
                Product prod = new Product();
                TryUpdateModel(prod);
                return View(prod);
            }
        }

        public ActionResult DeleteProduct(int id)
        {
            Product prod = ndb.Products.Find(id);
            List<Item> items = ndb.Items.Where(m => m.ProductId == id).ToList();
            foreach(var item in items)
            {
                ndb.Items.Remove(item);
            }

            //Remove the Product Image
            DeleteImage(prod.ImageUrl);
            
            ndb.Products.Remove(prod);
            ndb.SaveChanges();

            return RedirectToAction("ProductIndex");
        }

        public void DeleteImage(string imgUrl)
        {
            string imgPath = Request.MapPath("~/Images/" + imgUrl);
            if (System.IO.File.Exists(imgPath))
            {
                System.IO.File.Delete(imgPath);
            }
        }

        public ActionResult CreateItem(int id)
        {
            Product prod = ndb.Products.Find(id);
            ViewBag.Unit = new SelectList(Config.UnitList);
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

        public ActionResult DeleteItem(int id)
        {
            ndb.Items.Remove(ndb.Items.Find(id));
            ndb.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }


    }
}