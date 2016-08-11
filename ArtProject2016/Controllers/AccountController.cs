using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ArtProject2016.Models;
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


        public ActionResult Index(UserProfile model)
        {
            return View(model);
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
            viewModel.UserAccount = user;
            viewModel.UserProfile = artist;
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

                    
                    userToUpdate.firstName = model.UserAccount.firstName;
                    userToUpdate.lastName = model.UserAccount.lastName;
                    userToUpdate.userType = model.UserAccount.userType;
                    userToUpdate.nickName = model.UserAccount.nickName;
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


        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}
