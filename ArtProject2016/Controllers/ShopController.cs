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


namespace ArtProject2016.Controllers
{
    public class ShopController : Controller
    {
        private ArtContext db = new ArtContext();

        // GET: /Shop/Gallery
        public ActionResult Gallery()
        {
            var forsales = db.ForSales.Include(f => f.BuyerAccount).Include(f => f.Category).Include(f => f.SellerAccount)
                                       .Where(r => r.ForPosting && r.Sold == false);
            return View(forsales.ToList());
        }

        // GET: /Shop/Details/5
        public ActionResult ArtDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            galleryViewModel viewModel = new galleryViewModel();
            ForSale forsale = db.ForSales.Find(id);
            var related = db.ForSales.Where(ar => ar.SellerId == forsale.SellerId && ar.Id != id).Take(3).ToList();

            viewModel.ForSale = forsale;
            viewModel.relatedForSale = related;
            ViewBag.ReturnUrl = Url.Action("ArtDetails");
            if (forsale == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        //public ActionResult _RelatedArt(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
           
        //    if (related == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(related);
        //}

        //[HttpPost]
        //public ActionResult AddCart(int id)
        //{
        //    return View();
        //}

        [HttpGet]
        [Authorize]
        public ActionResult Cart()
        {
            var model = db.Carts.Where(buy => buy.UserAccountId == WebSecurity.CurrentUserId).ToList();

            var cartControls = new CartControls();

            var viewModel = new CartViewModel()
                                {
                                    CartItems = model,
                                    SubTotal = cartControls.GetTotal(),
                                    CartTotal = cartControls.GetTotal(),
                                    CartCount = cartControls.GetCount()
                                };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult AddtoCart(int id)
        {
            var cartItems = new Cart()
                                {
                                 Qty = 1,
                                 ForSaleId = id,
                                 UserAccountId = WebSecurity.CurrentUserId,
                                 dateCreated = DateTime.Now
                                };

            db.Carts.Add(cartItems);
            db.SaveChanges();

            ViewBag.Success = "Added to Cart Successfully!";

           // var model = db.Carts.Where(buy => buy.UserAccountId == WebSecurity.CurrentUserId).ToList();
            
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            var itemToRemove = db.Carts.Single(item => item.Id == id && item.UserAccountId == WebSecurity.CurrentUserId);
           // var cart = new Cart();

            db.Carts.Remove(itemToRemove);
            db.SaveChanges();

            var cart = new CartControls();
            var results = new CartRemoveViewModel
            {
              //  Message = Server.HtmlEncode(albumName) +
              //      " has been removed from your shopping cart.",
                CartTotal = "₱" + cart.GetTotal().ToString("0.00"),
                  CartCount = cart.GetCount(),
             //   ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }

        [HttpPost]
        public ActionResult AddVoucher(string voucher)
        {
            var code = db.VoucherCodes.SingleOrDefault(v => v.VoucherName == voucher);

            if(code.VoucherName.Any())
            {
                  var cart = new CartControls();

                if(cart.GetTotal() >= code.VoucherMinOrder)
                {
                    var results = new CheckoutViewModel
                    {
                      // CartTotal = cart.GetTotal() - code.VoucherDeduction
                        //  DeleteId = id
                        VoucherDeduction = code.VoucherDeduction,
                        Total = cart.GetTotal() - code.VoucherDeduction
                    };

                    TempData["vDeduction"] = code.VoucherDeduction;
                    return Json(results);
                }
                return Json(false);
            }
            else
            {
                return Json(false);
            }
        }

        [HttpPost]
        public ActionResult AddWishlist(int id)
        {
            try
            {
            var wishArt = new WishList()
                                {
                                 ForSaleId = id,
                                 UserAccountId = WebSecurity.CurrentUserId,
                                 dateAdded = DateTime.Now
                                };

            db.WishLists.Add(wishArt);
            db.SaveChanges();

            ViewBag.Success = "Added to Wish List";

            return Json(ViewBag.Success);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
     

        [HttpGet]
        public ActionResult CheckoutDetails()
        {

            var userAccount = db.UserAccounts.Single(user => user.Id == WebSecurity.CurrentUserId);
            var userProfile = db.UserProfiles.Single(user => user.UserAccountId == WebSecurity.CurrentUserId);

            checkoutDetailsViewModel viewModel = new checkoutDetailsViewModel();

            viewModel.UserName = userAccount.userName;
            viewModel.FirstName = userAccount.firstName;
            viewModel.LastName = userAccount.lastName;
            //userprofile
            viewModel.City = userProfile.city;
            viewModel.LandLine = userProfile.landLine;
            viewModel.MobileNo = userProfile.mobileNo;
            viewModel.PostalCode = userProfile.postalCode;
            viewModel.Province = userProfile.province;
            viewModel.Street = userProfile.street;

            var cart = new CartControls();
            if (TempData["vDeduction"] != null)
            {
                viewModel.VoucherDeduction = (decimal) TempData["vDeduction"];
                viewModel.CartTotal = cart.GetTotal() - viewModel.VoucherDeduction;
            }
            else
            {
                viewModel.CartTotal = cart.GetTotal();
            }
            //  viewModel.DetailsChecked = true;
            viewModel.SubTotal = cart.GetTotal();
           
           
            //ForSale forsale = db.ForSales.Find(id);
            //if (forsale == null)
            //{
            //    return HttpNotFound();
            //}
                    
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckoutDetails(checkoutDetailsViewModel model)
        {
            var userToUpdate = db.UserAccounts.Find(WebSecurity.CurrentUserId);
            var profileToUpdate = db.UserProfiles.Single(a => a.UserAccountId == WebSecurity.CurrentUserId);


            userToUpdate.firstName = model.FirstName;
            userToUpdate.lastName = model.LastName;

            profileToUpdate.city = model.City;
            profileToUpdate.landLine = model.LandLine;
            profileToUpdate.mobileNo = model.MobileNo;
            profileToUpdate.postalCode = model.PostalCode;
            profileToUpdate.province = model.Province;
            profileToUpdate.street = model.Street;

            TempData["vDeduction"] = model.VoucherDeduction;

            db.SaveChanges();

            return RedirectToAction("CheckOutSummary");
        }


        [HttpGet]
        public ActionResult CheckOutSummary()
        {
            var cartItems = db.Carts.Where(items => items.UserAccountId == WebSecurity.CurrentUserId).ToList();
            if (!cartItems.Any())
            {
                return RedirectToAction("Gallery");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userAccount = db.UserAccounts.Single(user => user.Id == WebSecurity.CurrentUserId);
            var userProfile = db.UserProfiles.Single(user => user.UserAccountId == WebSecurity.CurrentUserId);

            var controls = new CartControls();

            CheckoutViewModel viewModel = new CheckoutViewModel();
            viewModel.CartItems = cartItems;
            viewModel.UserAccount = userAccount;
            viewModel.UserProfile = userProfile;
            //userprofile
            //viewModel.City = userProfile.city;
            //viewModel.LandLine = userProfile.landLine;
            //viewModel.MobileNo = userProfile.mobileNo;
            //viewModel.PostalCode = userProfile.postalCode;
            //viewModel.Province = userProfile.province;
            //viewModel.Street = userProfile.street;

            viewModel.SubTotal = controls.GetTotal();
           
            if (TempData["vDeduction"] != null)
            {
                viewModel.VoucherDeduction = (decimal)TempData["vDeduction"];
                viewModel.Total = controls.GetTotal() - (decimal)TempData["vDeduction"];
            }
            else
            {
                viewModel.Total = controls.GetTotal();

            }
            //ForSale forsale = db.ForSales.Find(id);
            //if (forsale == null)
            //{
            //    return HttpNotFound();
            //}
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOutSummary(CheckoutViewModel model)
        {



            var delCart = new CartControls();
            delCart.EmptyCart();

            return RedirectToAction("Done");
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
