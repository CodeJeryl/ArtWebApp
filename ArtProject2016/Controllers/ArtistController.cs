using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;

namespace ArtProject2016.Controllers
{
    public class ArtistController : Controller
    {
        private ArtContext db;

        public ArtistController()
        {
            db = new ArtContext();
        }

        public ActionResult MyGallery(string nickName)
        {
            nickName = Server.HtmlEncode(nickName);
            MyGalleryViewModel model = new MyGalleryViewModel();

            var MyAccnt = db.UserAccounts.FirstOrDefault(acc => acc.nickName == nickName);

            if(MyAccnt == null)
            {
               // TempData["notFound"] = nickName;
               
                return RedirectToAction("NotFound", new { id = nickName });
            }

            var OtherArtist = db.UserAccounts.Where(oth => oth.ForSaleSeller.Any()).Take(20).ToList();
            var MyArt = db.ForSales.Where(art => art.SellerAccount.nickName == nickName).ToList();
            
           
            model.ForSales = MyArt;
            model.UserAccount = MyAccnt;
            model.ForSalesArtist = OtherArtist;

            return View(model);
        }
        
        [HttpGet]
        public ActionResult NotFound(string id)
        {
            var OtherArtist = db.UserAccounts.Where(oth => oth.ForSaleSeller.Any()).Take(20).ToList();
            var CloseEnough = db.ForSales.Where(clo => clo.SellerAccount.nickName.Contains(id)).ToList();
            ViewBag.NotFound = id; ;
            MyGalleryViewModel model = new MyGalleryViewModel();
            model.ForSalesArtist = OtherArtist;
            model.ForSales = CloseEnough;

            return View(model);
        }

	}
}