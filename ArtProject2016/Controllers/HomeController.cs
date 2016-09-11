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

namespace ArtProject2016.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
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
        public ActionResult ContactUs(ContactUs model)
        {
            using (ArtContext db = new ArtContext())
            {
                if (ModelState.IsValid)
                {
                    db.ContactUses.Add(model);
                    db.SaveChanges();
                    TempData["success"] =
                        "Your Message is successfully sent. Please give us 24-48hours to respond to your email. Thank you!";

                     string subject = "We received your message =)";

                          //  var firstName = db.UserAccounts.First(acc => acc.userName == model.userName).firstName;
                            var controls = new EmailControls();
                            //   string content = controls.PopulateBody(firstName, resetLink);  
                            string body = "<strong>Thank you for contacting us. </strong> <br/> <br/> " +
                                "Subject: " + model.Subject + "<br/> Message: "+ model.Message +

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

       
        //
        // POST: /Home/Create
     
    }
}
