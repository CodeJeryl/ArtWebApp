using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Functions;
using ArtProject2016.Migrations;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;

namespace ArtProject2016.Controllers
{
    public class HomeController : Controller
    {
        ArtContext db = new ArtContext();
        //
        // GET: /Home/
        public ActionResult Index()
        {
            using (ArtContext context = new ArtContext())
            {
                var newArt = db.ForSales.Where(art => art.ForPosting).OrderByDescending(or => or.datePosted).Take(8).ToList();
                return View(newArt);
            }

        }

        public ActionResult Gallery()
        {

            return View();
        }

        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(Message model)
        {
            using (ArtContext context = new ArtContext())
            {
                if (ModelState.IsValid)
                {
                    context.Messages.Add(model);
                    context.SaveChanges();
                    TempData["success"] =
                        "Your Message is successfully sent. Please give us 24-48hours to respond to your email. Thank you!";

                    string subject = "We received your message =)";

                    //  var firstName = db.UserAccounts.First(acc => acc.userName == model.userName).firstName;
                    var controls = new EmailControls();
                    //   string content = controls.PopulateBody(firstName, resetLink);  
                    string body = "<strong>Thank you for reaching us. </strong> <br/> <br/> " +
                        "Subject: " + model.Subject + "<br/> Message: " + model.Body +

                        "<br/> <br/>Please give us 24-48 hours to respond to your message. Thank you" +
                        "<br/> <br/> While waiting for our reply, feel free to browse our Online Gallery."; ;
                    string content = controls.PopulateBody("Message Received", model.FullName, body);

                    EmailControls.sendEmail("jerylsuarez@gmail.com", model.EmailAdd, "", "", subject, content);

                    return RedirectToAction("ContactUs");
                }
                return View(model);
            }

        }

        //
        // GET: /Home/Create
        public ActionResult FAQ()
        {
            return View();
        }


        [ChildActionOnly]
        public PartialViewResult ArtistMenu()
        {
            MenuViewModel model = new MenuViewModel();

            var artist = db.ForSales.Where(art => art.Sold != true).GroupBy(sd => sd.SellerAccount.nickName).Take(5).Select(g => g.FirstOrDefault());
            model.ForSales = artist.ToList();

            return PartialView("_ArtistMenu", model);
        }

        [ChildActionOnly]
        public PartialViewResult StyleMenu()
        {
            MenuViewModel model = new MenuViewModel();

            var categ = db.Styles;
            model.Styles = categ.Take(10).ToList();
            return PartialView("_StyleMenu", model);
        }

    }
}
