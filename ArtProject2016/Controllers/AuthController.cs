using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Functions;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;
using WebMatrix.WebData;
using Membership = System.Web.Security.Membership;


namespace ArtProject2016.Controllers
{

    public class AuthController : Controller
    {
        private ArtContext db = new ArtContext();

        // GET: /Account/
        public ActionResult Index()
        {
            return View("RegLogin");
        }




        public ActionResult RegLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }


        public ActionResult _Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            //UserAccount model = new UserAccount();
            return View("RegLogin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Login(loginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Functions.Membership.UserLogin(model.userName, model.password))
                {
                    if(!string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    return RedirectToAction("Index", "Account");
                    //  return RedirectToLocal(returnUrl);
                    //  FormsAuthentication.SetAuthCookie(model.userName, true);
                }
            }
            return View(model);

        }

        public ActionResult _Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            //UserAccount model = new UserAccount();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Register(registerViewModel model,string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = db.UserAccounts.Where(us => us.userName == model.userName);

                    if (!user.Any())
                    {
                        if (Functions.Membership.RegisterIsValid(model))
                        {
                            if (!string.IsNullOrEmpty(returnUrl))
                            {
                                return RedirectToLocal(returnUrl);
                            }
                            return RedirectToAction("Index", "Account");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Email Address already exist!");
                    }

                }
                
                return View(model);
            } 
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                throw;
            }
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View("RegLogin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            // Username is also the Email Address
            var user = Membership.GetUser(model.userName);

            if(user == null)
            {
                ModelState.AddModelError("","User/Email not exist");
                TempData["error"] = "User/Email not exist, Please try again";
            }
            else
            {
                var token = WebSecurity.GeneratePasswordResetToken(model.userName);
                var resetLink = "<a href='" + Url.Action("ResetPassword", "Auth", new { rt = token }, "http") + "'>Reset Password</a>";
                //get user emailid

                //send mail
                string subject = "Test Email: Password Reset Token";
                
                var firstName = db.UserAccounts.First(acc => acc.userName == model.userName).firstName;
                var controls = new EmailControls();
                string body = controls.PopulateBody(firstName, resetLink);

              //   string body = "<b>Please find the Password Reset Token</b><br/>" + resetLink; //edit it
                try
                {
                    EmailControls.sendEmail("jerylsuarez@gmail.com", model.userName, "", "", subject, body);
                    TempData["success"] = "Forgot password procedure sent to your email address. - " + model.userName;
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Error occured while sending email." + ex.Message;
                }
                //only for testing
             //   TempData["Message"] = resetLink;
            }
            return View("RegLogin");
        }

        [HttpGet]
        public ActionResult ResetPassword(string rt)
        {
            var model = new ResetPasswordViewModel();
            model.rt = rt;

            return View(model);
       }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(WebSecurity.ResetPassword(model.rt, model.NewPassword))
                {
                    TempData["success"] = "Password Changed. You can now login";
                }
                else
                {
                    TempData["error"] = "Forgot password request token expired. Please click forgot password link below";
                }
                return RedirectToAction("RegLogin");
                
            }

            return View(model);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
    }
}
