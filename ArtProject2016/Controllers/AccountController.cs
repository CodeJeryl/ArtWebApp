using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ArtProject2016.Functions;
using ArtProject2016.Models;
using Rotativa;
using WebMatrix.WebData;
using ArtProject2016.ViewModel;

namespace ArtProject2016.Controllers
{
    [Authorize] //(Roles = "Artist")
    public class AccountController : Controller
    {
        //
        // GET: /Artist/
        ArtContext context = new ArtContext();


        public ActionResult Index()
        {
            return RedirectToAction("UpdateProfile");
        }

        //
        // GET: /Artist/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }



        public ActionResult UpdateProfile()
        {
            UserAccount user = new UserAccount();
            user = context.UserAccounts.First(ua => ua.Id == WebSecurity.CurrentUserId);

            UserProfile artist = new UserProfile();
            artist = context.UserProfiles.First(ar => ar.UserAccountId == WebSecurity.CurrentUserId);

            UpdateArtistViewModel viewModel = new UpdateArtistViewModel();
            viewModel.firstName = user.firstName;
            viewModel.lastName = user.lastName;

            viewModel.nickName = user.nickName;
            viewModel.userType = user.userType;
            viewModel.UserProfile = artist;

            ViewBag.ReadOnly = artist.isIdVerified;
            TempData["readonly"] = artist.isIdVerified;
            //a => a.UserAccountId == WebSecurity.CurrentUserId).First();
            return View(viewModel);

        }


        [HttpPost]
        public ActionResult UpdateProfile(UpdateArtistViewModel model)
        {
            try
            {
                var userToUpdate = context.UserAccounts.Find(WebSecurity.CurrentUserId);
                //dapat kasi state.modified ginamit na code ko haha sorry! haba tuloy code
                if (Functions.Membership.UniqueNickname(model.nickName) || userToUpdate.nickName.ToUpper() == model.nickName.ToUpper())
                {
                    if (ModelState.IsValid)
                    {
                        var artistToUpdate =
                           context.UserProfiles.Single(a => a.UserAccountId == WebSecurity.CurrentUserId);

                        ViewBag.ReadOnly = artistToUpdate.isIdVerified;

                        userToUpdate.firstName = model.firstName.ToUpper();
                        userToUpdate.lastName = model.lastName.ToUpper();

                        string[] lastRole = Roles.GetRolesForUser(WebSecurity.CurrentUserName);

                        if (!Roles.IsUserInRole(model.userType))
                        {
                            Roles.RemoveUserFromRole(WebSecurity.CurrentUserName, lastRole[0]);

                            userToUpdate.userType = model.userType;
                            Roles.AddUserToRole(WebSecurity.CurrentUserName, model.userType);
                        }

                        userToUpdate.nickName = model.nickName;
                        //push to designated user type (section)

                        artistToUpdate.birthDay = model.UserProfile.birthDay;
                        artistToUpdate.education = model.UserProfile.education;
                        artistToUpdate.street = model.UserProfile.street;
                        artistToUpdate.city = model.UserProfile.city;
                        artistToUpdate.province = model.UserProfile.province;
                        artistToUpdate.postalCode = model.UserProfile.postalCode;
                        artistToUpdate.mobileNo = model.UserProfile.mobileNo;
                        artistToUpdate.landLine = model.UserProfile.landLine;
                        artistToUpdate.profileDesc = model.UserProfile.profileDesc;
                        artistToUpdate.Exhibitions = model.UserProfile.Exhibitions;

                        context.SaveChanges();
                        ViewBag.Success = "Profile updated!";
                        // Roles.DeleteCookie();
                        return View();
                        //    return RedirectToAction("Index");
                    }
                    return View(model);

                }

                ModelState.AddModelError("nickName", "Nick Name is already taken, please choose another nickname");
                return View(model);
            }
            catch
            {
                return View();
            }
        }


        //
        // GET: /Artist/Delete/5
        public ActionResult Verification()
        {
            var model = context.UserProfiles.Single(user => user.UserAccountId == WebSecurity.CurrentUserId);
            return View(model);
        }

        //
        // POST: /Artist/Delete/5
        [HttpPost]
        public ActionResult Verification(HttpPostedFileBase file)
        {
            try
            {
                //  ViewBag.Error = null;
                if (file != null)
                {
                    if (ArtControls.IsImage(file))
                    {
                        if (file.ContentLength < 4000000)
                        {
                            if (AccountControls.UploadID(file))
                            {
                               TempData["success"] =
                                    "ID successfully uploaded, Please Wait within 24hours for approval to start selling art. Thank you!";
                            }
                        }
                        else
                        {
                            TempData["error"] = "Image file exceed 4mb.";
                        }
                    }
                    else
                    {
                        TempData["error"] = ".jpg / .jpeg / .png file extensions only.";
                    }
                }
                else
                {
                    TempData["error"] = "Please choose a Picture of your Valid ID to be uploaded.";
                }

                var model = context.UserProfiles.Single(user => user.UserAccountId == WebSecurity.CurrentUserId);
                return View(model);
                //    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
               
                return View();
            }
        }



        [HttpPost]
        public ActionResult ChangePassword(ChangePassViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (WebSecurity.ChangePassword(WebSecurity.CurrentUserName, model.OldPassword, model.NewPassword))
                {
                    TempData["success"] = "Password Changed!";
                }
                else
                {
                    TempData["error"] = "Wrong Old password. Please try again";
                }

                return RedirectToAction("UpdateProfile");
                // return View("UpdateProfile");
            }
            return View();
        }



        public ActionResult PayOut()
        {
            //using (ArtContext db = new ArtContext())
            //{

            var DueForRedeem = context.OrderDetails.Where(cash => cash.ForSale.SellerId == WebSecurity.CurrentUserId && cash.Order.Paid &&
                                                   cash.OrderDetailStatus != "Cancelled" && cash.Returned != true && cash.ReadyToRedeem != true
                                                   && cash.Redeemed != true && cash.BuyerReceived).ToList();

            foreach (var date in DueForRedeem)
            {
                var tenDays = Convert.ToInt32(((date.BuyerReceivedDateTime.Value.AddDays(10) - DateTime.Now.Date).TotalDays));

                if (tenDays <= 0)
                {
                    date.ReadyToRedeem = true;
                    context.SaveChanges();
                }
            }

            PayoutViewModel model = new PayoutViewModel();
            SellControls sell = new SellControls();

            model.PendingOrderDetails = context.OrderDetails.Where(cash => cash.ForSale.SellerId == WebSecurity.CurrentUserId && cash.Order.Paid &&
                                               cash.OrderDetailStatus != "Cancelled" && cash.Returned != true && cash.ReadyToRedeem != true && cash.Redeemed != true).ToList();

            //cash.OrderDetailStatus == "Shipment Processing" || cash.OrderDetailStatus == "Shipped" || cash.OrderDetailStatus == "Paid" || cash.OrderDetailStatus == "Delivered"
            model.RedeemOrderDetails =
                context.OrderDetails.Where(cash => cash.ForSale.SellerId == WebSecurity.CurrentUserId && cash.Order.Paid
                                              && cash.ReadyToRedeem && cash.BuyerReceived && cash.Redeemed != true && cash.Returned != true).ToList();

            model.RedeemedOrderDetails = context.OrderDetails.Where(deemed => deemed.ForSale.SellerId == WebSecurity.CurrentUserId
                                            && deemed.Order.Paid && deemed.Redeemed).ToList();

            model.Payouts = context.Payouts.Where(p => p.UserAccountId == WebSecurity.CurrentUserId).ToList();

            model.PendingAmt = sell.GetPending();
            model.RedeemableAmt = sell.GetRedeemable();
            model.RedeemedAmt = sell.GetRedemeeded();

            return View(model);
        }

        [HttpPost]
        public ActionResult RequestPayout(Payout model)
        {
            if (ModelState.IsValid)
            {
                var redeemed =
                     context.OrderDetails.Where(cash => cash.ForSale.SellerId == WebSecurity.CurrentUserId && cash.Order.Paid
                                                   && cash.ReadyToRedeem && cash.BuyerReceived && cash.Redeemed != true && cash.Returned != true).ToList();


                var payOut = new Payout()
                                 {
                                     FullName = model.FullName,
                                     PaymentInfo = model.PaymentInfo,
                                     PayOutMethod = model.PayOutMethod,
                                     Status = "Payout Processing",
                                     DateRequested = DateTime.Now,
                                     RedeemedPayoutAmt = model.RedeemedPayoutAmt,
                                     UserAccountId = WebSecurity.CurrentUserId
                                 };

                context.Payouts.Add(payOut);

                foreach (var orderDetail in redeemed)
                {
                    orderDetail.Redeemed = true;
                }

                context.SaveChanges();

                //send mail
                string subject = "Pay Out Request is now on process";

                var controls = new EmailControls();
                string body = "<strong>Thank you for selling artworks in <website>. </strong> <br/> <br/> " +
                     "<br/> <br/> You requested the amount: ₱ " + model.RedeemedPayoutAmt + " thru  " + model.PayOutMethod + "<br> <br>"
                     + "with Payment information of - " + model.PaymentInfo + "<br> <br> Please give us 5 working days to process your request. "
                    +"<br> <br> Thank you! :)";
                string content = controls.PopulateBody("Pay Out Request is now on process", model.FullName, body);

                EmailControls.sendEmail("jerylsuarez@gmail.com", WebSecurity.CurrentUserName, "", "", subject, content);
                TempData["success"] = "Payout Request Complete, Please give us 5 working days to process your request. Thank you!";
           
                return RedirectToAction("PayOut");
            }
            TempData["error"] = "Payout Request Incomplete, Please try again. Thank you!";
            return RedirectToAction("PayOut");
        }

        public ActionResult Print()
        {
            return new ViewAsPdf("testPr", new { name = "jeryl" }) { FileName = "Test.pdf" };
            //   return new ActionAsPdf("testPr", new {name ="jeryl"}) { FileName = "Test.pdf" };
        }

        public ActionResult testPr(string name)
        {
            ViewBag.Name = name;
            return View();
        }


        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}
