using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;
using Rotativa;
using Rotativa.Options;
using WebMatrix.WebData;
using ArtProject2016.Functions;

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
            var orders = db.OrderDetails.Where(art => art.ForSale.SellerId == WebSecurity.CurrentUserId && art.Order.Paid)
                                         .OrderByDescending(ord => ord.Order.OrderDate).ToList();

            var sellcon = new SellControls();

            ViewBag.Pending = "₱ " + sellcon.GetPending();
            ViewBag.Redeemable = "₱ "+ sellcon.GetRedeemable();
                
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
            model._ShippingCompany = db.ShippingCompanies.ToList();

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
                editShipping.ShippingCompanyId = model.SelectedShippingId;
                editShipping.TrackingNumber = model.Track;
                editShipping.Shipped = true;
                editShipping.OrderDetailStatus = "Shipped";

                var shipName = db.ShippingCompanies.Find(model.SelectedShippingId);
                var OrderTrackingDetails = new OrderTracking()
                                               {
                                                   DateTime = DateTime.Now,
                                                   StatusType = "Shipped",
                                                   Description = "Order has been shipped with " + shipName.Name +
                                                                 " For more details, please visit the shipping company website and enter"
                                                                 + " the tracking number [" + model.Track + "].",
                                                   OrderDetailId = model.OrderDetails.Id
                                               };

                db.OrderTrackings.Add(OrderTrackingDetails);
                db.SaveChanges();
                TempData["success"] = "Successfully Added Shipping Details for Order # "+ model.OrderDetails.Id;
                return RedirectToAction("Index");

            }

          //  model._ShippingCompany = db.ShippingCompanies.ToList();
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
        public ActionResult BuyerDetails(int Id)
        {
            var buyerDets = db.OrderDetails.Find(Id);

          // return View(buyerDets);
            return new ViewAsPdf(buyerDets)
            {
                FileName = "Order #:" + buyerDets.OrderId.ToString() + " Details.pdf",
                PageSize = Size.Letter,
                IsBackgroundDisabled = false,
                IsGrayScale = false,
                IsJavaScriptDisabled = true,
                CustomSwitches = "--viewport-size 1000x1000"
            };
        }


        //
        // GET: print buyer details
        public ActionResult ViewBuyerDetails()
        {
            return View();
        }

        //public ActionResult PrintInvoice(int invoiceId)
        //{
           
        //    return new ActionAsPdf(
        //                   "Invoice",
        //                   new { invoiceId = invoiceId }) { FileName = "Invoice.pdf" };
        //}

      
      

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
