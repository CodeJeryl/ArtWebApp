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
                if (ModelState.IsValid)
                {
                    if (Functions.Membership.UserLogin(model.userName, model.password))
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
        public ActionResult UpdateOrder(int Id)
        {
            var order = db.Orders.Find(Id);

            return View(order);
        }

        [HttpPost]
        public ActionResult UpdateOrder(Order model)
        {
            using (db)
            {
                if (ModelState.IsValid)
                {

                    var entry = db.Entry(model);

                        entry.State = EntityState.Modified;
                        entry.Property(e => e.VoucherCodeId).IsModified = false;
                        entry.Property(e => e.UserAccountId).IsModified = false;

                    db.SaveChanges();
                    TempData["success"] = "Order no. " + model.Id + " is successfully updated!";
                    return RedirectToAction("Index");
                }
                return View(model);
            }
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
              //  db.OrderDetails.Attach(model);
               // db.Entry(model).State = EntityState.Modified;

                var entry = db.Entry(model);
                entry.State = EntityState.Modified;
                entry.Property(e => e.ForSaleId).IsModified = false;
                entry.Property(e => e.OrderId).IsModified = false;

                TempData["success"] = "Order Detail no. " + model.Id + " is successfully updated!";
                   
                db.SaveChanges();
                return RedirectToAction("OrderDetails", new { Id = model.OrderId });
            }
            catch
            {
                return View();
            }
        }

        //
        [HttpGet]
        public ActionResult ProfileVerification()
        {
            var model = db.UserProfiles.OrderByDescending(id => id.isIdVerified == false).ToList();
            return View(model);
        }

       
        [HttpGet]
        public ActionResult Payout()
        {
            try
            {
                var model = db.Payouts.OrderByDescending(asc => asc.Status == "Processing").ToList();
                return View(model);
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult UpdatePayout(int Id)
        {
            var model = db.Payouts.Find(Id);
            return View(model);
        }
    }
}
