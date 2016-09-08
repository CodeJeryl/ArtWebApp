using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;

namespace ArtProject2016.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        ArtContext db = new ArtContext();
        //
        // GET: /Admin/
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        
        //
        // POST: /Admin/Create
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(loginViewModel model)
        {
            try
            {
              if(ModelState.IsValid)
              {
                  if(Functions.Membership.UserLogin(model.userName,model.password))
                  {
                      return RedirectToAction("Index");
                  }
                  TempData["error"] = "Incorrect Username/Password.";
              }
                return View(model);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Index()
        {
            var orders = db.Orders.OrderBy(p => p.OrderDate).ThenBy(p => p.Paid != true);

            return View(orders);
        }

       
        public ActionResult OrderDetails(int Id)
        {
            var details = db.OrderDetails.Where(det => det.OrderId == Id).OrderBy(de => de.UnitPrice).ToList();
            return View(details);
        }



        [HttpGet]
        public ActionResult EditOrderDetails(int Id)
        {
            try
            {
                var detail = db.OrderDetails.Find(Id);

                return View(detail);
            }
            catch
            {
                return View();
            }
        }

        //
        // POST: /Admin/Edit/5
        [HttpPost]
        public ActionResult EditOrderDetails(OrderDetail model)
        {
            try
            {
                db.OrderDetails.Attach(model);
                db.Entry(model).State = EntityState.Modified;

                var entry = db.Entry(model);
                entry.Property(e => e.ForSaleId).IsModified = false;
                entry.Property(e => e.OrderId).IsModified = false;
              
                db.SaveChanges();
                return RedirectToAction("OrderDetails", new { Id = model.OrderId});
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Admin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
