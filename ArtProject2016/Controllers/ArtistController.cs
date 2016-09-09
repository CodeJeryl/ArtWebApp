using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Models;

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

            var MyArt = db.ForSales.Where(art => art.SellerAccount.nickName == nickName).ToList();
            
            return View(MyArt);
        }

	}
}