using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Models;

namespace ArtProject2016.Controllers
{
    public class testController : Controller
    {
        private ArtContext db = new ArtContext();

        // GET: /test/
        public ActionResult Index()
        {
            var forsales = db.ForSales.Include(f => f.BuyerAccount).Include(f => f.Category).Include(f => f.SellerAccount);
            return View(forsales.ToList());
        }

        // GET: /test/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForSale forsale = db.ForSales.Find(id);
            if (forsale == null)
            {
                return HttpNotFound();
            }
            return View(forsale);
        }

        // GET: /test/Create
        public ActionResult Create()
        {
            ViewBag.BuyerId = new SelectList(db.UserAccounts, "Id", "userName");
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "name");
            ViewBag.SellerId = new SelectList(db.UserAccounts, "Id", "userName");
            return View();
        }

        // POST: /test/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Title,mediumUsed,wSize,hSize,Price,Framed,yearCreated,artDescription,datePosted,dateUpdated,ForPosting,Sold,BuyerId,SellerId,CategoryID")] ForSale forsale)
        {
            if (ModelState.IsValid)
            {
                db.ForSales.Add(forsale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BuyerId = new SelectList(db.UserAccounts, "Id", "userName", forsale.BuyerId);
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "name", forsale.CategoryID);
            ViewBag.SellerId = new SelectList(db.UserAccounts, "Id", "userName", forsale.SellerId);
            return View(forsale);
        }

        // GET: /test/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForSale forsale = db.ForSales.Find(id);
            if (forsale == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerId = new SelectList(db.UserAccounts, "Id", "userName", forsale.BuyerId);
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "name", forsale.CategoryID);
            ViewBag.SellerId = new SelectList(db.UserAccounts, "Id", "userName", forsale.SellerId);
            return View(forsale);
        }

        // POST: /test/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Title,mediumUsed,wSize,hSize,Price,Framed,yearCreated,artDescription,datePosted,dateUpdated,ForPosting,Sold,BuyerId,SellerId,CategoryID")] ForSale forsale)
        {
            if (ModelState.IsValid)
            {
                db.Entry(forsale).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BuyerId = new SelectList(db.UserAccounts, "Id", "userName", forsale.BuyerId);
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "name", forsale.CategoryID);
            ViewBag.SellerId = new SelectList(db.UserAccounts, "Id", "userName", forsale.SellerId);
            return View(forsale);
        }

        // GET: /test/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForSale forsale = db.ForSales.Find(id);
            if (forsale == null)
            {
                return HttpNotFound();
            }
            return View(forsale);
        }

        // POST: /test/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ForSale forsale = db.ForSales.Find(id);
            db.ForSales.Remove(forsale);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
