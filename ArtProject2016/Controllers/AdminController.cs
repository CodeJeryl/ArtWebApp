using System;
using System.Collections.Generic;
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

       
        public ActionResult OrderDetails(int id)
        {
            var details = db.OrderDetails.Where(det => det.OrderId == id).OrderBy(de => de.UnitPrice).ToList();
            return View(details);
        }

        //
        // POST: /Admin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
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
