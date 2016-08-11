using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Migrations;
using ArtProject2016.Models;

namespace ArtProject2016.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Gallery()
        {

            return View();
        }
        
        // GET: /Home/Details/5
        public ActionResult Details()
        {
            return View();
        }

        //
        // GET: /Home/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Home/Create
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
        // GET: /Home/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Home/Edit/5
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
        // GET: /Home/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Home/Delete/5
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
