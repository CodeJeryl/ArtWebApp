using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;
using WebMatrix.WebData;

namespace ArtProject2016.Controllers
{
    [Authorize]
    public class ArtController : Controller
    {
        private ArtContext db = new ArtContext();

        // GET: /Art/
        public ActionResult Index()
        {
            if (WebSecurity.IsAuthenticated)
            {
                var model = db.ForSales.Where(fs => fs.SellerId == WebSecurity.CurrentUserId).ToList();

                //if (TempData["Success"] != null)
                //{
                //    ViewBag.Success = TempData["Success"].ToString();
                //}
                //if (TempData["Error"] != null)
                //{
                //    ViewBag.Error = TempData["Error"].ToString();
                //}

                return View(model);
            }
            
            ViewBag.error = "No results found";
            return (ViewBag);
        }
        
        // GET: Upload Picture
        public ActionResult Upload()
        {
            var model = db.UserProfiles.Any(user => user.Id == WebSecurity.CurrentUserId && user.isIdVerified == false);

            if (model)
            {
                return RedirectToAction("Verification", "Account");
            }
            else
            {
                uploadViewModel viewModel = new uploadViewModel();
                viewModel._Categories = db.Categories.ToList();
                return View(viewModel);
            }
        }

        //
        // POST: /Artist/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(uploadViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                model._Categories = db.Categories.ToList();
                if (ModelState.IsValid)
                {
                    if (files.Count(file => file != null && file.ContentLength > 0) > 0)
                    {
                        foreach (var file in files)
                        {
                            if (Functions.ArtControls.IsImage(file))
                            {
                                if (file.ContentLength < 4000000)
                                {

                                }
                                else
                                {
                                    ViewBag.Error = "Image file exceed 4mb.";
                                    return View();
                                }
                            }
                            else
                            {
                                ViewBag.Error = ".jpg / .jpeg / .png file extensions only.";
                                return View();
                            }
                        }
                    }

                    if (Functions.ArtControls.UploadArt(model, files))
                    {
                        ViewBag.Success = "Art Uploaded Successfully!";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    var allErrors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage));

                }

                return View(model);
            }
            catch
            {
                return View(model);
            }
        }

        // GET: /Art/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //  ForSale forsale = db.ForSales.Single(uid => uid.SellerId == WebSecurity.CurrentUserId || uid.Id == id);

            var forsale = db.ForSales.Single(uid => uid.SellerId == WebSecurity.CurrentUserId && uid.Id == id);
            //    var album = db.ForSaleAlbums.Where(uid => uid.ForSaleId == id).ToList();
            var category = db.Categories.ToList();

            if (forsale == null)
            {
                return HttpNotFound();
            }

            //if (TempData["Success"] != null)
            //{
            //    ViewBag.Success = TempData["Success"].ToString();
            //}

            uploadViewModel viewModel = new uploadViewModel();
            viewModel.ForSale = forsale;
            //    viewModel.ForSaleAlbumCollection = album;
            viewModel._Categories = category;

            viewModel.selectedCategoryId = forsale.CategoryID;
            viewModel.selectedYear = forsale.yearCreated;


            //     ViewBag.selectedCategoryId = new SelectList(db.Categories, "Id", "name", forsale.CategoryID);
            //     ViewBag.UserAccountId = new SelectList(db.UserAccounts, "Id", "userName", forsale.UserAccountId);
            return View(viewModel);
            /*
             Album album = db.Albums.Find(id);

    if (album == null)

        return HttpNotFound();

    AlbumSelectListViewModel aslvm = new AlbumSelectListViewModel(album, db.Artists, db.Genres);
             */
        }

        // POST: /Art/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, uploadViewModel model)
        {
            if (ModelState.IsValid)
            {
                var artUpdate = db.ForSales.Single(uid => uid.SellerId == WebSecurity.CurrentUserId || uid.Id == id);

                artUpdate.Title = model.ForSale.Title;
                artUpdate.mediumUsed = model.ForSale.mediumUsed;
                artUpdate.wSize = model.ForSale.wSize;
                artUpdate.hSize = model.ForSale.hSize;
                artUpdate.Price = model.ForSale.Price;
                artUpdate.yearCreated = model.selectedYear;
                artUpdate.Framed = model.ForSale.Framed;
                artUpdate.otherArtist = model.ForSale.otherArtist;
                artUpdate.otherArtistAddress = model.ForSale.otherArtistAddress;
                artUpdate.otherArtistName = model.ForSale.otherArtistName;
                artUpdate.dateUpdated = DateTime.Now;
                artUpdate.CategoryID = model.selectedCategoryId;

                //   db.Entry(forsale).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Successfuly Updated!";
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "name", model.ForSale.CategoryID);
            //      ViewBag.UserAccountId = new SelectList(db.UserAccounts, "Id", "userName", forsale.UserAccountId);
            return View(model);
        }


        // GET: /Art/Delete/5
        [HttpGet]
        public ActionResult DeleteImage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            /*     ForSaleAlbum forsaleAlbum =
                     db.ForSaleAlbums.SingleOrDefault(i => i.ForSale.SellerId == WebSecurity.CurrentUserId && i.Id == id);

                 if (forsaleAlbum == null)
                 {
                     return HttpNotFound();
                 }
                 return View(forsaleAlbum); */
            return View();
        }

        // POST: /Art/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteImage(int id)
        {
            //ForSaleAlbum forSaleAlbum = db.ForSaleAlbums.Find(id);
            //db.ForSaleAlbums.Remove(forSaleAlbum);
            //db.SaveChanges();
            //TempData["Success"] = "Image successfully deleted!";
            //return RedirectToAction("Edit", new { id = forSaleAlbum.ForSaleId });
            return View();
        }


        [HttpGet]
        public ActionResult DeleteArt(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForSale forsale =
                db.ForSales.SingleOrDefault(i => i.SellerId == WebSecurity.CurrentUserId && i.Id == id);
            
            if (forsale == null)
            {
                return HttpNotFound();
            }

            if(forsale.Sold)
            {
                TempData["Error"] = "Art cannot be deleted. It's already sold";
                return RedirectToAction("Index");
            }
            return View(forsale);
        }

        // POST: /Art/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteArt(int id)
        {
            var forsale = db.ForSales.Single(art => art.Id == id && art.Sold == false);
            if(forsale == null)
            {
              //  TempData["Error"] = "Art cannot be deleted";
                return HttpNotFound();
            }
           
                db.ForSales.Remove(forsale);

            //delete wishlist associated with the Art that will be deleted
            //onCascade delete is set to false
            var wish = db.WishLists.Where(list => list.ForSaleId == id);

            db.WishLists.RemoveRange(wish);

                db.SaveChanges();
                TempData["Success"] = "Art successfully deleted!";
          
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Wishlist()
        {
            var wishlist = db.WishLists.Where(wish => wish.UserAccountId == WebSecurity.CurrentUserId).ToList();

            
            return View(wishlist);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
