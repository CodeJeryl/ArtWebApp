using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Models;
using WebMatrix.WebData;

namespace ArtProject2016.Controllers
{
    public class SellController : Controller
    {
        //
        // GET: /Sale/
        ArtContext db = new ArtContext();
        public ActionResult Index()
        {
            var orders = db.OrderDetails.Where(art => art.ForSale.SellerId == WebSecurity.CurrentUserId && art.OrderDetailStatus != "Delivered").ToList();
            
            return View(orders);
        }

        //
        // GET: /Sale/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Sale/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Sale/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Sale/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Sale/Edit/5
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
        // GET: /Sale/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Sale/Delete/5
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
