using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;
using WebMatrix.WebData;

namespace ArtProject2016.Controllers
{
    [Authorize]
    public class SellController : Controller
    {
        //
        // GET: /Sale/
        ArtContext db = new ArtContext();
        public ActionResult Index()
        {
            var orders = db.OrderDetails.Where(art => art.ForSale.SellerId == WebSecurity.CurrentUserId && art.OrderDetailStatus != "Delivered" && art.Order.Paid).ToList();
            
                return View(orders);
            
        }


        //
        // GET: /Sale/Details/5
        public ActionResult Shipment(int id)
        {
            var ship = db.OrderDetails.Single(ss => ss.Id == id && ss.ForSale.SellerId == WebSecurity.CurrentUserId);
          //  var forsale = db.ForSales.Single(es => es. =)
                //Single(ss => ss.Id == id && ss.ForSale.SellerId == WebSecurity.CurrentUserId);
           
           
           ShipmentViewModel model = new ShipmentViewModel();
            model.OrderDetails = ship;

            if(ship == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Shipment(ShipmentViewModel model)
        {
            var editShipping = db.OrderDetails.Find(model.OrderDetails.Id);

            if (editShipping == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if(ModelState.IsValid)
            {
                editShipping.ShippingCompany = model.TrackCompany;
                editShipping.TrackingNumber = model.Track;
                editShipping.Shipped = true;
                editShipping.OrderDetailStatus = "Shipped";
                

                var OrderTrackingDetails = new OrderTracking()
                                               {
                                                   DateTime = DateTime.Now,
                                                   StatusType = "Shipped",
                                                   Description = "Order has been shipped with " + model.TrackCompany +
                                                                 " For more details, please visit the shipping company website and enter"
                                                                 + " the tracking number [" + model.Track + "].",
                                                   OrderDetailId = model.OrderDetails.Id
                                               };

                db.OrderTrackings.Add(OrderTrackingDetails);
                db.SaveChanges();
                TempData["success"] = "Successfully Added Shipping Details for Order # "+ model.OrderDetails.Id;
                return RedirectToAction("Index");

            }

            
            return View(model);
         }

        //
        // GET: /Sale/Create
        public ActionResult Return(int id)
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
