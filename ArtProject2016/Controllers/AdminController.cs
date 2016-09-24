using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Functions;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;
using WebMatrix.WebData;

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
        public ActionResult UpdateOrderDetails(int Id)
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
        public ActionResult UpdateOrderDetails(OrderDetail model)
        {
            try
            {
                //  db.OrderDetails.Attach(model);
                // db.Entry(model).State = EntityState.Modified;
                //  Class1 sd = new Class1();

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
        public ActionResult VerifyID(int Id)
        {
            var model = db.UserProfiles.Find(Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult VerifyID(UserProfile model)
        {

            if (ModelState.IsValid)
            {
                using (db)
                {
                    var entry = db.Entry(model);
                    entry.State = EntityState.Modified;

              //      entry.Property(up => up.UserAccountId).IsModified = false;
                    TempData["success"] = "Profile successfully updated of userID: " + model.UserAccountId;

                    db.SaveChanges();

                    return RedirectToAction("ProfileVerification");
                }
            }

            return View(model);

        }


        [HttpGet]
        public ActionResult Payout()
        {
            try
            {
                var model = db.Payouts.OrderByDescending(asc => asc.Status == "Payout Processing").ToList();
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

        [HttpPost]
        public ActionResult UpdatePayout(Payout model)
        {
            if (ModelState.IsValid)
            {
                using (db)
                {
                    var entry = db.Entry(model);
                    entry.State = EntityState.Modified;

                    entry.Property(up => up.UserAccountId).IsModified = false;
                    TempData["success"] = "Payout Request updated of userID: " + model.UserAccountId;

                    db.SaveChanges();
                    return RedirectToAction("Payout");
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Messages()
        {
            var model = db.Messages.OrderByDescending(des => des.DateTime);
            return View(model);
        }

        [HttpGet]
        public ActionResult ReplyMessage(int id)
        {
            AdminReplyMessage ViewModel = new AdminReplyMessage();

            var message = db.Messages.Find(id);

            ViewModel.Message = message;
            if(message == null)
            { 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            return View(ViewModel);

        }

        [HttpPost]
        public ActionResult ReplyMessage(AdminReplyMessage viewModel)
        {
            var updateMessage = db.Messages.Find(viewModel.Message.Id);
          if(viewModel.MessageReply.BodyReply !=null)
          {
                var Reply = new MessageReply()
                                {
                                    AdminAccountId = WebSecurity.CurrentUserId,
                                    BodyReply = viewModel.MessageReply.BodyReply,
                                    DateTime = DateTime.Now,
                                    MessageId = viewModel.Message.Id
                                };
               
                updateMessage.Responded = true;

                db.MessageReplies.Add(Reply);
                db.SaveChanges();

              TempData["success"] = "Reply sent!";
                return RedirectToAction("Messages");
          }
            ModelState.AddModelError("BodyReply","Dont leave your Reply Textbox blank!");
            viewModel.Message = updateMessage;
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult RepliedMessage(int id)
        {
            var Model = db.MessageReplies.Where(rep => rep.MessageId == id);
            return View(Model.ToList());
        }
    }
}
