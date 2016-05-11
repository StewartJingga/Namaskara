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

        public StoreManagerController()
        {
            MakeExpired();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

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
        public ActionResult Details(int id, string message = "")
        {
            ViewBag.Message = message;

            Product product = ndb.Products.Find(id);
            product.Items = ndb.Items.Where(m => m.ProductId == id).ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(UpdateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                Product product = ndb.Products.Find(model.ProductId);
                TryUpdateModel(product);
                if (model.DiscountPercentage.GetType() != typeof(double))
                {
                    product.DiscountPercentage = Convert.ToDouble(model.DiscountPercentage);
                }
                

                product.Items = null;
                ndb.Entry(product).State = System.Data.Entity.EntityState.Modified;
                ndb.SaveChanges();

                return RedirectToAction("Details", new { id = model.ProductId, message = "Product succesfully updated" });
            }
            

            return View();
        }

        public ActionResult EditItem(int id)
        {
            return View(ndb.Items.Where(m => m.ProductId == id).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditItem(IList<UpdateItemViewModel> model)
        {
            if (ModelState.IsValid)
            {
                Debug.Print(model[0].Id.ToString());
                Debug.Print(model[0].Name);
                Debug.Print(model[0].Size);
                Debug.Print(model[0].RetailPrice.ToString());
                Debug.Print(model[0].IsAvailable.ToString());
                Item ci = null;
                for (int i = 0; i < model.Count(); i++)
                {
                    ci = ndb.Items.Find(model[i].Id);
                    if (ci == null) return View(model);
                    ci.RetailPrice = model[i].RetailPrice;
                    ci.IsAvailable = model[i].IsAvailable;
                    ndb.Entry(ci).State = System.Data.Entity.EntityState.Modified;
                    
                }
                ndb.SaveChanges();

                return RedirectToAction("Details", new {id = ci.ProductId, message = "Item has been successfully updated" });
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
                prod.Description = model.Description;
                ndb.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                ndb.SaveChanges();
                return RedirectToAction("Details", new { id = prod.ProductId });
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

        public ActionResult OrderIndex(string sort)
        {
            ViewBag.OrderSort = String.IsNullOrEmpty(sort) ? "order_asc": "";
            List<Order> orders;
            switch (sort)
            {
                case "order_asc":
                    orders = ndb.Orders.OrderBy(m => m.OrderId).ToList();
                    break;
                default:
                    orders = ndb.Orders.OrderByDescending(m => m.OrderId).ToList();
                    break;
                
            }
            return View(orders);
        }

        public ActionResult WCOrders(string sort, string status)
        {
            ViewBag.OrderSort = String.IsNullOrEmpty(sort) ? "order_asc" : "";
            status = String.IsNullOrEmpty(status) ? "Waiting For Confirmation" : status;
            ViewBag.OrderFilter = status;
            List<Order> orders;

            switch (sort)
            {
                case "order_asc":
                    orders = ndb.Orders.Where(m => m.Status == status).OrderBy(m => m.OrderId).ToList();
                    break;
                default:
                    orders = ndb.Orders.Where(m => m.Status == status).OrderByDescending(m => m.OrderId).ToList();
                    break;

            }
            return View(orders);
        }

        public ActionResult ConfirmedOrders(string sort)
        {
            ViewBag.OrderSort = String.IsNullOrEmpty(sort) ? "order_asc" : "";
            
            List<Order> orders;

            switch (sort)
            {
                case "order_asc":
                    orders = ndb.Orders.Where(m => m.Status == "Confirmed").OrderBy(m => m.OrderId).ToList();
                    break;
                default:
                    orders = ndb.Orders.Where(m => m.Status == "Confirmed").OrderByDescending(m => m.OrderId).ToList();
                    break;

            }
            foreach(var order in orders)
            {
                order.OrderInfo = ndb.OrderInformation.Find(order.OrderInfoId);
            }
            return View(orders);
        }


        public ActionResult ConfirmOrPostponePayment(int id, string status)
        {
            Order order = ndb.Orders.Find(id);
            order.Status = status;
            order.CompleteDate = DateTime.Now;
            ndb.Entry(order).State = System.Data.Entity.EntityState.Modified;
            ndb.SaveChanges();

            return RedirectToAction("WCOrders");

        }

        public ActionResult OrderDetails(int id)
        {
            return View(ndb.Orders.Include("OrderInfo").Single(m => m.OrderId == id));
        }

        public ActionResult OrderItems(int id)
        {
            List<OrderDetail> orderDetails = ndb.OrderDetails.Where(m => m.OrderId == id).ToList();
            foreach (var od in orderDetails)
            {
                od.Item = ndb.Items.Include("Product").Single(m => m.Id == od.ItemId);
            }
            ViewData["OrderNumber"] = id;
            Order order = ndb.Orders.Find(id);
            ViewData["OrderTotal"] = order.Total;
            ViewData["OrderDelivery"] = order.Delivery;
            ViewData["OrderPrice"] = order.Price;
            ViewData["OrderPromo"] = Utilities.FindReducingPrice(order.Price, order.PromoDiscount);

            return PartialView(orderDetails);
        }

        public ActionResult EditOrder(int id)
        {           
            Order order = ndb.Orders.Find(id);
            ViewBag.Status = new SelectList(Config.StatusList, order.Status);
            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                Order changes = ndb.Orders.Find(order.OrderId);
                TryUpdateModel(changes);
                ndb.Entry(changes).State = System.Data.Entity.EntityState.Modified;
                ndb.SaveChanges();

                return RedirectToAction("OrderIndex");
            }
            
            ViewBag.Status = new SelectList(Config.StatusList);
            return View(order);
        }

        private ActionResult MakeExpired()
        {
            List<Order> orders = ndb.Orders.Where(m => m.Status == "Order Submitted").ToList();
            foreach(Order order in orders)
            {
                
                TimeSpan difference = DateTime.Now - order.ConfirmDate;
                if (difference.TotalHours >= 1)
                {
                    order.Status = "Expired";
                    ndb.Entry(order).State = System.Data.Entity.EntityState.Modified;
                }
            }
            
            ndb.SaveChanges();
            return RedirectToAction("OrderIndex");
        }

        public ActionResult ExtendTime(int id)
        {
            Order order = ndb.Orders.Find(id);
            order.Status = "Waiting For Confirmation";
            order.ConfirmDate = DateTime.Now.AddDays(1);

            ndb.Entry(order).State = System.Data.Entity.EntityState.Modified;
            ndb.SaveChanges();

            return RedirectToAction("OrderDetails", new { id = id });
        }

        public ActionResult PromoIndex()
        {
            return View(ndb.PromoCodes);
        }
        public ActionResult CreatePromo()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePromo(PromoCode model)
        {
            PromoCode promo = new PromoCode();
            TryUpdateModel(promo);
            try
            {
                ndb.PromoCodes.Add(promo);
                ndb.SaveChanges();
                return RedirectToAction("PromoIndex");
            }
            catch
            {
                return RedirectToAction("PromoIndex");
            }

        }
        public ActionResult DeletePromo(string code)
        {
            ndb.PromoCodes.Remove(ndb.PromoCodes.Find(code));

            return RedirectToAction("PromoIndex");
        }

        [ChildActionOnly]
        public ActionResult OrderNavigation()
        {
            return PartialView();
        }



    }
}