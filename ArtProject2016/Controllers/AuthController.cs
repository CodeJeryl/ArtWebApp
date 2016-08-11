using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;


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
                        ModelState.AddModelError(string.Empty, "Username already exist!");
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
