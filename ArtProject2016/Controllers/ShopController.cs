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
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
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

            if (forsale == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var related = db.ForSales.Where(ar => ar.SellerId == forsale.SellerId && ar.Id != id && ar.ForPosting).Take(3).ToList();

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
        public ActionResult CheckOutSummary(string PaypalPayment, string token)
        {
            if (PaypalPayment == "false" && token != null)
            {
                int orderId = (int)TempData["orderId"];
                var orderCancelled = db.Orders.Find(orderId);
                var orderDetailCancelled = db.OrderDetails.Where(item => item.OrderId == orderId).ToList();

                orderCancelled.OrderStatus = "Payment Cancelled.";

                foreach (var orderDetail in orderDetailCancelled)
                {
                    orderDetail.OrderDetailStatus = "Payment Cancelled.";
                    orderDetail.BuyerReceived = false;

                    var orderTrack = new OrderTracking()
                                         {
                                             DateTime = DateTime.Now,
                                             StatusType = "PayPal/CC Cancelled",
                                             Description = "PayPal/CC Payment was cancelled or an error occured. - " + token,
                                             OrderDetailId = orderDetail.Id
                                         };
                    db.OrderTrackings.Add(orderTrack);
                }

                db.SaveChanges();
            }

            var cartItems = db.Carts.Where(items => items.UserAccountId == WebSecurity.CurrentUserId).ToList();
            if (!cartItems.Any())
            {
                TempData["error"] = "you dont have any items in cart";
                return RedirectToAction("Gallery");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userAccount = db.UserAccounts.Single(user => user.Id == WebSecurity.CurrentUserId);
            var userProfile = db.UserProfiles.Single(user => user.UserAccountId == WebSecurity.CurrentUserId);

            if (PaypalPayment != null)
            {
                TempData["error"] = "Paypal Payment Cancelled. Please try again.";
            }

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

                                           PaymentType = viewModel.PaymentType,
                                           Paid = false,
                                           OrderStatus = "Waiting for Payment",

                                           OrderDate = DateTime.Now,

                                           UserAccountId = WebSecurity.CurrentUserId

                                       };

                    db.Orders.Add(newOrder);

                    foreach (Cart cartItem in cartItems)
                    {
                        var newOrderDetails = new OrderDetail()
                                                  {
                                                      Quantity = cartItem.Qty,
                                                      UnitPrice = cartItem.ForSale.Price,
                                                      OrderDetailStatus = "Waiting for Payment",
                                                      ForSaleId = cartItem.ForSaleId,
                                                      BuyerReceived = false,
                                                      ReadyToRedeem = false,
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

                    //send mail
                    string subject = "<website> Order Confirmation #: "+ newOrder.Id;
                    //default value if not bank
                    string bankDetails = "Please wait for another email confirmation about your payment. Thank you";

                    if(viewModel.PaymentType == "Bank")
                    {
                        bankDetails = "Total Amount to be paid:" + viewModel.Total;
                    }

                    var controls = new EmailControls();
                    string body = "<strong>Thank you for supporting local artist from <website>. </strong> <br/> <br/> " +
                                  "<br/> <br/>Your Order #" + newOrder.Id +" <br/> <br/> has been placed on "+ DateTime.Now.ToShortDateString() + " via " + viewModel.PaymentType + " Payment." +
                                  "<br/> <br/> " + bankDetails + "<br/> <br/> orderDetails here <br/> <br/> <em><small>Note: If you cancelled your order thru Paypal/Credit card payment, please disregard this email. </small> </em>";

                    string content = controls.PopulateBody("Website Order Confirmation", viewModel.UserAccount.firstName, body);
                    EmailControls.sendEmail("jerylsuarez@gmail.com", viewModel.UserAccount.userName, "", "", subject, content);


                    if (viewModel.PaymentType == "PayPal" || viewModel.PaymentType == "CreditCard")
                    {
                        PaypalControls pay = new PaypalControls();

                        var PaypalResult = pay.PaypalExpress(viewModel, newOrder.Id);
                        TempData["PayPal"] = "True";
                        TempData["orderId"] = newOrder.Id;
                        return Redirect(PaypalResult);
                    }
                    else
                    {
                        CartControls cart = new CartControls();
                        cart.EmptyCart();

                        TempData["PayPal"] = "False";
                        TempData["email"] = viewModel.UserAccount.userName;
                        TempData["orderId"] = newOrder.Id;
                        return Redirect("OrdersComplete");
                    }

               

                    // return RedirectToAction("Cart");
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

        //[HttpGet]
        //public ActionResult Paypal(string success)
        //{
        //    TempData["success"] = success;

        //    return RedirectToAction("cart", "shop");
        //}

        public void PayPalCheck(CheckoutViewModel viewModel)
        {
            PaypalControls pay = new PaypalControls();
            decimal test = viewModel.VoucherDeduction;
            //  string test1 = viewModel.VoucherCodeId;
            //     var PaypalResult = pay.PaypalExpress(viewModel);

            //TempData["error"] = token;
            //   Response.Redirect(PaypalResult);
            // return View("CheckoutSummary");

        }

        [HttpGet]
        public ActionResult PaymentProcessing(string PayerID, string token)
        {
            if (PayerID != null && token != null && TempData["orderId"] != null)
            {
                using (ArtContext db = new ArtContext())
                {
                    // TempData.Keep("orderId");
                    int OrderId = Convert.ToInt32(TempData["orderId"]);
                    var OrderModel = db.Orders.Find(OrderId);

                    var NewPaypalOrder = new PaypalPayment
                                             {
                                                 Token = token,
                                                 PayerId = PayerID,
                                                 Remarks = "Before Do Express Checkout Save",
                                                 DateTime = DateTime.Now,
                                                 Order = OrderModel
                                             };

                    db.PaypalPayments.Add(NewPaypalOrder);

                    CartControls cart = new CartControls();
                    cart.EmptyCart();

                    var OrderTotal = OrderModel.Total.ToString();
                    PaypalControls paypal = new PaypalControls();
                    string result = paypal.DoExpress(token, PayerID, OrderTotal);
                    if (result == "error")
                    {
                        TempData["error"] = "An error occured on Payment Processing.";
                        return RedirectToAction("Gallery", "Shop");
                    }



                    NewPaypalOrder.TransactionId = result;
                    db.SaveChanges();


                    TempData["orderId"] = OrderId;
                    return RedirectToAction("OrdersComplete");
                }
            }

            TempData["error"] = "An error occured on Payment Processing.";
            return RedirectToAction("Gallery", "Shop");

        }

        [HttpGet]
        public ActionResult OrdersComplete()
        {
            if (TempData["orderId"] != null)
            {
                ViewBag.OrderId = TempData["orderId"].ToString();

                return View();
            }

            TempData["error"] = "An error occured to orders complete page, Please check your MyOrders Page.";
            return RedirectToAction("Gallery", "Shop");
        }

        public EmptyResult IPNreceiver()
        {
            PaypalControls PayControls = new PaypalControls();

            var formVals = new Dictionary<string, string>();
            formVals.Add("cmd", "_notify-validate");
            //true for sandbox, false for live
            string response = PayControls.GetPayPalResponse(formVals, true);

            if (response == "VERIFIED")
            {

                string transactionID = Request["txn_id"];
                string sAmountPaid = Request["mc_gross"];
                string orderID = Request["custom"];
                string payStatus = Request["payment_status"];

                //_logger.Info("IPN Verified for order " + orderID);

                //validate the order
                Decimal amountPaid = 0;
                Decimal.TryParse(sAmountPaid, out amountPaid);

                //check if transaction is already processed
                var trans = db.Orders.SingleOrDefault(tran => tran.PayPal.TransactionId == transactionID);
                if (trans.PayPal.IPNverified)
                {
                    return new EmptyResult();
                }

                //Order order = _orderService.GetOrder(new Guid(orderID));
                Order order = db.Orders.Find(orderID);
                //check the amount paid

                if (PayControls.AmountPaidIsValid(order, amountPaid))
                {

                    // check that Payment_status=Completed
                    //' check that Txn_id has not been previously processed
                    //' check that Receiver_email is your Primary PayPal email
                    //' check that Payment_amount/Payment_currency are correct
                    //' process payment

                    //Address add = new Address();
                    //add.FirstName = Request["first_name"];
                    //add.LastName = Request["last_name"];
                    //add.Email = Request["payer_email"];
                    //add.Street1 = Request["address_street"];
                    //add.City = Request["address_city"];
                    //add.StateOrProvince = Request["address_state"];
                    //add.Country = Request["address_country"];
                    //add.Zip = Request["address_zip"];
                    //add.UserName = order.UserName;


                    //process it
                    try
                    {
                        if (payStatus == "Completed")
                        {
                            trans.Paid = true;
                            trans.OrderStatus = "Paid thru PayPal";
                            trans.PayPal.Remarks = payStatus + " - Amount: " + amountPaid;
                            trans.PayPal.DateTime = DateTime.Now;
                            trans.PayPal.IPNverified = true;

                            var orderDets =
                                db.OrderDetails.Where(dets => dets.OrderId == Convert.ToInt32(orderID)).ToList();
                            foreach (var orderDetail in orderDets)
                            {
                                orderDetail.OrderDetailStatus = "Shipment Processing";
                                orderDetail.BuyerReceived = false;
                                orderDetail.Redeemed = false; //default
                                orderDetail.ReadyToRedeem = false;

                                var orderTracki = new OrderTracking()
                                {
                                    DateTime = DateTime.Now,
                                    StatusType = "PayPal: Payment Received",
                                    Description = "Payment Received. Shipment will start processing.",
                                    OrderDetailId = orderDetail.Id
                                };
                                db.OrderTrackings.Add(orderTracki);
                            }

                            //send mail
                            string subject = "Paypal Payment received.";
                            var controls = new EmailControls();
                            string body = "<strong>Thank you for supporting local artist from <website>. </strong> <br/> <br/> " +
                                          "<br/> <br/> We already received your payment thru Paypal! <br/> <br/> We will start to process your orders.";
                            string content = controls.PopulateBody("Paypal Payment Successful", trans.FirstName, body);
                            EmailControls.sendEmail("jerylsuarez@gmail.com", trans.Username, "", "", subject, content);

                        }
                        else
                        {
                            trans.PayPal.Remarks = payStatus;
                            trans.PayPal.DateTime = DateTime.Now;
                        }

                        db.SaveChanges();
                        //_pipeline.AcceptPalPayment(order, transactionID, amountPaid);
                        //_logger.Info("IPN Order successfully transacted: " + orderID);
                        //   return RedirectToAction("Receipt", "Order", new { id = order.ID });
                    }
                    catch
                    {
                        //HandleProcessingError(order, x);
                        //  return View();
                    }
                }
                else
                {
                    //let fail - this is the IPN so there is no viewer
                }
            }



            return new EmptyResult();
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
