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
    [Authorize]
    public class ShopController : Controller
    {
        private ArtContext db = new ArtContext();

        // GET: /Shop/Gallery
        [AllowAnonymous]
        public ActionResult Gallery()
        {
            var forsales = db.ForSales.Include(f => f.BuyerAccount).Include(f => f.Category).Include(f => f.SellerAccount)
                                       .Where(r => r.ForPosting && r.Sold == false);
            return View(forsales.ToList());
        }

        // GET: /Shop/Details/5
        [AllowAnonymous]
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
            var code = db.VoucherCodes.SingleOrDefault(v => v.VoucherName == voucher.Trim() && v.VoucherEnabled);

            if (code != null)
            {
                var cart = new CartControls();

                if (cart.GetTotal() >= code.VoucherMinOrder)
                {
                    CheckoutViewModel viewModel = new CheckoutViewModel();

                    decimal subTotal = cart.GetTotal();
                    // CartTotal = cart.GetTotal() - code.VoucherDeduction
                    //  DeleteId = id
                    viewModel.SubTotal = subTotal;
                    viewModel.VoucherCodeId = code.Id;
                    viewModel.VoucherDeduction = code.VoucherDeduction;
                    viewModel.Total = subTotal - code.VoucherDeduction;

                    TempData["vDeduction"] = code.VoucherDeduction;
                    //     return Json(results); 
                    // return Json(viewModel, JsonRequestBehavior.AllowGet);
                    return PartialView("_summary", viewModel);
                    //  return Json(new { success = true, message = PartialView("Evil", viewModel) });
                }

                return Json(new { vouchAdd = "false", Error = "Need a Minimum of " + code.VoucherMinOrder.ToString() + " to use this voucher" });
            }
            return Json(new { vouchAdd = "false", Error = "Voucher is not valid anymore!" });

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

                return Json(true);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult RemoveWishlist(int id)
        {
            var removeWish =
                db.WishLists.Single(wish => wish.ForSaleId == id && wish.UserAccountId == WebSecurity.CurrentUserId);


            db.WishLists.Remove(removeWish);
            db.SaveChanges();

            // ViewBag.Success = "Wishlist removed";
            return Json(true);
        }


        [HttpGet]
        public ActionResult CheckoutDetails()
        {
            var cartItems = db.Carts.Where(items => items.UserAccountId == WebSecurity.CurrentUserId).ToList();
            if (!cartItems.Any())
            {
                TempData["error"] = "you dont have any items in cart";
                return RedirectToAction("Gallery");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userAccount = db.UserAccounts.Single(user => user.Id == WebSecurity.CurrentUserId);
            var userProfile = db.UserProfiles.Single(user => user.UserAccountId == WebSecurity.CurrentUserId);

            checkoutDetailsViewModel viewModel = new checkoutDetailsViewModel();

            //viewModel.UserName = userAccount.userName;
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
                viewModel.VoucherDeduction = (decimal)TempData["vDeduction"];
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
            if (ModelState.IsValid)
            {
                var userToUpdate = db.UserAccounts.Find(WebSecurity.CurrentUserId);
                var profileToUpdate = db.UserProfiles.Single(a => a.UserAccountId == WebSecurity.CurrentUserId);


                userToUpdate.firstName = model.FirstName.ToUpper();
                userToUpdate.lastName = model.LastName.ToUpper();

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
            return View(model);
        }


        [HttpGet]
        public ActionResult CheckOutSummary()
        {
            var cartItems = db.Carts.Where(items => items.UserAccountId == WebSecurity.CurrentUserId).ToList();
            if (!cartItems.Any())
            {
                TempData["error"] = "you dont have any items in cart";
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
        public ActionResult CheckOutSummary(CheckoutViewModel viewModel)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                var cartItems = db.Carts.Where(items => items.UserAccountId == WebSecurity.CurrentUserId).ToList();
                var userAccount = db.UserAccounts.Single(user => user.Id == WebSecurity.CurrentUserId);
                var userProfile = db.UserProfiles.Single(user => user.UserAccountId == WebSecurity.CurrentUserId);

                viewModel.UserAccount = userAccount;
                viewModel.UserProfile = userProfile;
                viewModel.CartItems = cartItems;

                //if(viewModel.VoucherCodeId == 0)
                //{
                //    viewModel.VoucherCodeId = null;
                //}
                if (ModelState.IsValid)
                {
                    var newOrder = new Order()
                                       {
                                           Username = userAccount.userName,
                                           FirstName = userAccount.firstName,
                                           LastName = userAccount.lastName,

                                           //address
                                           Street = userProfile.street,
                                           City = userProfile.city,
                                           Province = userProfile.province,
                                           PostalCode = userProfile.postalCode,
                                           MobileNo = userProfile.mobileNo,
                                           LandLine = userProfile.landLine,


                                           VoucherCodeId = viewModel.VoucherCodeId,
                                           VoucherDeduction = viewModel.VoucherDeduction,

                                           SubTotal = viewModel.SubTotal,
                                           Total = viewModel.Total,

                                           PaymentType = "wala pa",
                                           OrderStatus = "Processing...",

                                           OrderDate = DateTime.Now

                                       };

                    db.Orders.Add(newOrder);

                    foreach (Cart cartItem in cartItems)
                    {
                        var newOrderDetails = new OrderDetail()
                                                  {
                                                      Quantity = cartItem.Qty,
                                                      UnitPrice = cartItem.ForSale.Price,
                                                      ForSaleId = cartItem.ForSaleId,
                                                      OrderId = newOrder.Id
                                                  };
                        db.OrderDetails.Add(newOrderDetails);

                        var updateArt = db.ForSales.Find(cartItem.ForSaleId);
                        updateArt.Sold = true;
                        updateArt.BuyerId = WebSecurity.CurrentUserId;
                    } 
                    db.SaveChanges();

                    dbContextTransaction.Commit();
                    //  TempData["s"] = viewModel.SubTotal;
                    //  TempData["d"] = viewModel.Total;
                    //        var delCart = new CartControls();
                    //     delCart.EmptyCart();
                    TempData["success"] = "Success. ID no: " + newOrder.Id.ToString();
                    return RedirectToAction("Cart");
                }
                //else
                //{
                //    var message = string.Join(" | ", ModelState.Values
                //         .SelectMany(v => v.Errors)
                //         .Select(e => e.ErrorMessage));

                //    TempData["Error"] = message;
                //    return View(model);
                //}
                // var cartItems = db.Carts.Where(items => items.UserAccountId == WebSecurity.CurrentUserId).ToList();
                //model.CartItems = cartItems;
                return View(viewModel);
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
    }
}
