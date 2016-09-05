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
                if (ModelState.IsValid)
                {
                    var userToUpdate = context.UserAccounts.Find(WebSecurity.CurrentUserId);
                    var artistToUpdate = context.UserProfiles.Single(a => a.UserAccountId == WebSecurity.CurrentUserId);

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
            catch
            {
                return View();
            }
        }


        //
        // GET: /Artist/Delete/5
        public ActionResult Verification()
        {
            var model = context.UserProfiles.Single(user => user.Id == WebSecurity.CurrentUserId);
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
                    if (Functions.ArtControls.IsImage(file))
                    {
                        if (file.ContentLength < 4000000)
                        {
                            if (Functions.AccountControls.UploadID(file))
                            {
                                ViewBag.Success = "Successfully uploaded ID. Please Wait within 24hours for approval to start selling art.";
                            }
                        }
                        else
                        {
                            ViewBag.Error = "Image file exceed 4mb.";
                        }
                    }
                    else
                    {
                        ViewBag.Error = ".jpg / .jpeg / .png file extensions only.";
                    }
                }
                else
                {
                    ViewBag.Error = "Please choose a Picture of your Valid ID to be uploaded.";
                }

                return View();

                //    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
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
            using (ArtContext db = new ArtContext())
            {
                PayoutViewModel model = new PayoutViewModel();
                SellControls sell = new SellControls();

                model.PendingOrderDetails =
                    db.OrderDetails.Where(cash => cash.ForSale.SellerId == WebSecurity.CurrentUserId && cash.Order.Paid
                                                  && cash.OrderDetailStatus == "Shipment Processing" ||
                                                  cash.OrderDetailStatus == "Shipped" ||
                                                  cash.OrderDetailStatus == "Paid" ||
                                                  cash.OrderDetailStatus == "Delivered" && cash.Redeemed != true).ToList();

                model.RedeemOrderDetails =
                    db.OrderDetails.Where(cash => cash.ForSale.SellerId == WebSecurity.CurrentUserId && cash.Order.Paid
                                                  && cash.OrderDetailStatus == "Finished" && cash.Redeemed != true).ToList();

                model.PendingAmt = sell.GetPending();
                model.RedeemableAmt = sell.GetRedeemable();

                return View(model);
                
            }
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
