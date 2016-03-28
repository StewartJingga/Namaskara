using Namaskara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Namaskara.ViewModels;
using System.Diagnostics;

namespace Namaskara.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StoreManagerController : Controller
    {
        NamaskaraDb ndb = new NamaskaraDb();
        // GET: StoreManager
        public ActionResult CategoryIndex()
        {
            List<Category> categories = ndb.Categories.ToList();
            return View(categories);
        }

        public ActionResult ProductIndex(int id = 0)
        {
            List<Product> products;
            if(id == 0)
            {
                products = ndb.Products.ToList();
                
            }
            else
            {
                products = ndb.Products.Where(m => m.CategoryId == id).ToList();  
            }
            return View(products);


        }
        public ActionResult Details(int id)
        {
            return View(ndb.Items.Where(m => m.ProductId == id).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(IList<UpdateItemViewModel> model)
        {
            if (ModelState.IsValid)
            {
                Debug.Print(model[0].Id.ToString());
                Debug.Print(model[0].Name);
                Debug.Print(model[0].Size);
                Debug.Print(model[0].RetailPrice.ToString());
                Debug.Print(model[0].IsAvailable.ToString());
                for (int i = 0; i < model.Count(); i++)
                {
                    Item ci = ndb.Items.Find(model[i].Id);
                    if (ci == null) return View(model);
                    ci.IsAvailable = model[i].IsAvailable;
                    ndb.Entry(ci).State = System.Data.Entity.EntityState.Modified;
                    
                }
                ndb.SaveChanges();

                return RedirectToAction("ProductIndex");
            }
            return View(model);
        }




        public ActionResult Edit(int id)
        {
            Product product = ndb.Products.Find(id);
            
            return View(product);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model)
        {

            if (ModelState.IsValid)
            {
                Product prod = ndb.Products.Find(model.ProductId);
                prod.Origin = model.Origin;
                ndb.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                ndb.SaveChanges();
                return RedirectToAction("ProductIndex");
            }
            else return View(model);  
        }

        public ActionResult Delete(int id)
        {
            return View(ndb.Products.Find(id));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ndb.Products.Remove(ndb.Products.Find(id));
            ndb.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult OrderIndex()
        {
            
            return View(ndb.Orders.ToList());
        }

        public ActionResult WCOrder()
        {

            return View(ndb.Orders.Where(m=>m.Status == "Waiting For Confirmation").ToList());
        }

        public ActionResult EditOrder(int id)
        {
            List<string> status = new List<string>
            {
                "Order Submitted",
                "Waiting For Confirmation",
                "Confirmed",
                "Delivered",
                "Completed"
            };
            ViewBag.Status = new SelectList(status);
            return View(ndb.Orders.Find(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                ndb.Entry(order).State = System.Data.Entity.EntityState.Modified;
                ndb.SaveChanges();

                return RedirectToAction("OrderIndex");
            }
            List<string> status = new List<string>
            {
                "Order Submitted",
                "Waiting For Confirmation",
                "Confirmed",
                "Delivered",
                "Completed"
            };
            ViewBag.Status = new SelectList(status);
            return View(order);
        }


    }
}