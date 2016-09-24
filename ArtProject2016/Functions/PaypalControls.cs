using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Data;
using System.Configuration;
using System.Linq;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
using WebMatrix.WebData;



namespace ArtProject2016.Functions
{
    public class PaypalControls
    {
        public string PaypalExpress(CheckoutViewModel viewModel, int orderId)
        {
            using (ArtContext db = new ArtContext())
            {

                var cartItems = db.Carts.Where(items => items.UserAccountId == WebSecurity.CurrentUserId).ToList();
                var userAccount = db.UserAccounts.Single(user => user.Id == WebSecurity.CurrentUserId);
                var userProfile = db.UserProfiles.Single(user => user.UserAccountId == WebSecurity.CurrentUserId);

                viewModel.UserAccount = userAccount;
                viewModel.UserProfile =  userProfile;
                viewModel.CartItems = cartItems;


                PaymentDetailsType paymentDetail = new PaymentDetailsType();
                CurrencyCodeType currency = (CurrencyCodeType)EnumUtils.GetValue("PHP", typeof(CurrencyCodeType));
                //   SolutionTypeType sole = (SolutionTypeType)EnumUtils.GetValue("SOLE", typeof(SolutionTypeType));
                //  LandingPageType bill = (LandingPageType)EnumUtils.GetValue("BILLING", typeof(LandingPageType));


                List<PaymentDetailsItemType> paymentItems = new List<PaymentDetailsItemType>();
                //   List<PaymentDetailsItemType> paymentItem = new List<PaymentDetailsItemType>();

                decimal orderTotal = 0;
                for (int i = 0; i < cartItems.Count; i++)
                {
                    PaymentDetailsItemType paymentItem = new PaymentDetailsItemType();
                    paymentItem.Name = cartItems[i].ForSale.Title;
                    decimal itemAmount = cartItems[i].ForSale.Price;
                    //Regex.Replace(cartItems[0].ForSale.Price.ToString(), "[^0-9.]", "");
                    paymentItem.Amount = new BasicAmountType(currency, itemAmount.ToString());
                    int itemQuantity = cartItems[i].Qty;
                    paymentItem.Quantity = itemQuantity;

                    paymentItem.ItemHeight = new MeasureType("Inches", Convert.ToDecimal(cartItems[i].ForSale.hSize));
                    paymentItem.ItemWidth = new MeasureType("Inches", Convert.ToDecimal(cartItems[i].ForSale.wSize));

                    paymentItems.Add(paymentItem);
                    orderTotal += itemAmount * itemQuantity;
                }

                if (viewModel.VoucherCodeId != null && viewModel.VoucherDeduction > 0)
                {
                    PaymentDetailsItemType discount = new PaymentDetailsItemType();
                    discount.Name = "Voucher Discount";
                    discount.Description = "";
                    decimal NegativeDisc = viewModel.VoucherDeduction * -1;
                    discount.Amount = new BasicAmountType(currency, NegativeDisc.ToString());
                    paymentItems.Add(discount);
                    orderTotal += NegativeDisc;
                }

                // paymentItem.ItemCategory = (ItemCategoryType)EnumUtils.GetValue("Painting", typeof(ItemCategoryType));

                //  List<CartItem> myOrderList = myCartOrders.GetCartItems();

                //for (int i = 0; i < cartItems.Count; i++)
                //{
                //    encoder["L_PAYMENTREQUEST_0_NAME" + i] = myOrderList[i].Product.ProductName.ToString();
                //    encoder["L_PAYMENTREQUEST_0_AMT" + i] = myOrderList[i].Product.UnitPrice.ToString();
                //    encoder["L_PAYMENTREQUEST_0_QTY" + i] = myOrderList[i].Quantity.ToString();
                //}

                paymentDetail.PaymentDetailsItem = paymentItems;
                paymentDetail.ItemTotal =
                    new BasicAmountType((CurrencyCodeType)EnumUtils.GetValue("PHP", typeof(CurrencyCodeType)),
                                        orderTotal.ToString());
                paymentDetail.PaymentAction =
                    (PaymentActionCodeType)EnumUtils.GetValue("Sale", typeof(PaymentActionCodeType));
                paymentDetail.OrderTotal =
                    new BasicAmountType((CurrencyCodeType)EnumUtils.GetValue("PHP", typeof(CurrencyCodeType)),
                                        orderTotal.ToString());
                paymentDetail.NotifyURL = "http://localhost:60817/Shop/PaypalIPN";
                paymentDetail.Custom = orderId.ToString();
                List<PaymentDetailsType> paymentDetails = new List<PaymentDetailsType>();
                paymentDetails.Add(paymentDetail);
               

                SetExpressCheckoutRequestDetailsType ecDetails = new SetExpressCheckoutRequestDetailsType();
                ecDetails.ReturnURL = "http://localhost:60817/Shop/PaymentProcessing";
                ecDetails.CancelURL = "http://localhost:60817/Shop/CheckOutSummary?PaypalPayment=false";
                ecDetails.PaymentDetails = paymentDetails;
                ecDetails.NoShipping = "1";
                ecDetails.LandingPage = LandingPageType.BILLING;
                ecDetails.SolutionType = SolutionTypeType.SOLE;
                ecDetails.LocaleCode = "PH";
                ecDetails.BuyerEmail = WebSecurity.CurrentUserName;


                //PaymentDetailsType paymentDetail = new PaymentDetailsType();
                //CurrencyCodeType currency = (CurrencyCodeType)EnumUtils.GetValue("USD", typeof(CurrencyCodeType));
                //PaymentDetailsItemType paymentItem = new PaymentDetailsItemType();
                //paymentItem.Name = "item";
                //double itemAmount = 1.00;
                //paymentItem.Amount = new BasicAmountType(currency, itemAmount.ToString());
                //int itemQuantity = 1;
                //paymentItem.Quantity = itemQuantity;
                ////paymentItem.ItemCategory = (ItemCategoryType)EnumUtils.GetValue(, typeof(ItemCategoryType));
                //List<PaymentDetailsItemType> paymentItems = new List<PaymentDetailsItemType>();
                //paymentItems.Add(paymentItem);
                //paymentDetail.PaymentDetailsItem = paymentItems;

                //paymentDetail.PaymentAction = (PaymentActionCodeType)EnumUtils.GetValue("Sale", typeof(PaymentActionCodeType));
                //paymentDetail.OrderTotal = new BasicAmountType((CurrencyCodeType)EnumUtils.GetValue("USD", typeof(CurrencyCodeType)), (itemAmount * itemQuantity).ToString());
                //List<PaymentDetailsType> paymentDetails = new List<PaymentDetailsType>();
                //paymentDetails.Add(paymentDetail);

                //SetExpressCheckoutRequestDetailsType ecDetails = new SetExpressCheckoutRequestDetailsType();
                //ecDetails.ReturnURL = "https://devtools-paypal.com/guide/expresscheckout/dotnet?success=true";
                //ecDetails.CancelURL = "https://devtools-paypal.com/guide/expresscheckout/dotnet?cancel=true";
                //ecDetails.PaymentDetails = paymentDetails;

                SetExpressCheckoutRequestType request = new SetExpressCheckoutRequestType();
                request.Version = "104.0";
                request.SetExpressCheckoutRequestDetails = ecDetails;


                SetExpressCheckoutReq wrapper = new SetExpressCheckoutReq();
                wrapper.SetExpressCheckoutRequest = request;
                //Dictionary<string, string> sdkConfig = new Dictionary<string, string>();
                //sdkConfig.Add("mode", "security-test-sandbox");
                //sdkConfig.Add("account1.apiUsername", "jeryl.suarez-facilitator_api1.yahoo.com");
                //sdkConfig.Add("account1.apiPassword", "BGX735ARZFAS95ZN");
                //sdkConfig.Add("account1.apiSignature", "A2QoVdXW-3NyIOsjGGBweDnke5g2A3YC3DckQi-iAkuUZnpi4KveJ.7-");
                //sdkConfig.Add("account1.applicationId", "APP-80W284485P519543T");
                //PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(sdkConfig);


                Dictionary<string, string> config = PayPal.Manager.ConfigManager.Instance.GetProperties();


                //// Create the Classic SDK service instance to use.
                PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(config);
                //config
                SetExpressCheckoutResponseType setECResponse = new SetExpressCheckoutResponseType();
                setECResponse = service.SetExpressCheckout(wrapper);

                if (setECResponse != null)
                {
                    // Response envelope acknowledgement
                    string acknowledgement = "SetExpressCheckout API Operation - ";
                    acknowledgement += setECResponse.Ack.ToString();
                    // logger.Info(acknowledgement + "\n");
               //     Console.WriteLine(acknowledgement + "\n");

                    // # Success values
                    if (setECResponse.Ack.ToString().Trim().ToUpper().Equals("SUCCESS"))
                    {
                        // # Redirecting to PayPal for authorization
                        // Once you get the "Success" response, needs to authorise the
                        // transaction by making buyer to login into PayPal. For that,
                        // need to construct redirect url using EC token from response.
                        // For example,
                        string redirectURL =
                            "https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&useraction=commit&token=" +
                            setECResponse.Token;

                        return redirectURL;
                        // Express Checkout Token
                        //   logger.Info("Express Checkout Token : " + responseSetExpressCheckoutResponseType.Token + "\n");
                        //  Console.WriteLine("Express Checkout Token : " + setECResponse.Token + "\n");
                    }
                    // # Error Values
                    else
                    {
                        List<ErrorType> errorMessages = setECResponse.Errors;
                        foreach (ErrorType error in errorMessages)
                        {
                            //  logger.Debug("API Error Message : " + error.LongMessage);
                            Console.WriteLine("API Error Message : " + error.LongMessage + "\n");
                        }
                    }
                }

                return "error";

            }
        }

        //public string pay()
        //{
        //    PaymentDetailsType paymentDetail = new PaymentDetailsType();
        //    CurrencyCodeType currency = (CurrencyCodeType)EnumUtils.GetValue("USD", typeof(CurrencyCodeType));
        //    PaymentDetailsItemType paymentItem = new PaymentDetailsItemType();
        //    paymentItem.Name = "item";
        //    double itemAmount = 1.00;
        //    paymentItem.Amount = new BasicAmountType(currency, itemAmount.ToString());
        //    int itemQuantity = 1;
        //    paymentItem.Quantity = itemQuantity;
        //    //paymentItem.ItemCategory = (ItemCategoryType)EnumUtils.GetValue(, typeof(ItemCategoryType));
        //    List<PaymentDetailsItemType> paymentItems = new List<PaymentDetailsItemType>();
        //    paymentItems.Add(paymentItem);
        //    paymentDetail.PaymentDetailsItem = paymentItems;

        //    paymentDetail.PaymentAction =
        //        (PaymentActionCodeType)EnumUtils.GetValue("Sale", typeof(PaymentActionCodeType));
        //    paymentDetail.OrderTotal =
        //        new BasicAmountType((CurrencyCodeType)EnumUtils.GetValue("USD", typeof(CurrencyCodeType)),
        //                            (itemAmount * itemQuantity).ToString());
        //    List<PaymentDetailsType> paymentDetails = new List<PaymentDetailsType>();
        //    paymentDetails.Add(paymentDetail);

        //    SetExpressCheckoutRequestDetailsType ecDetails = new SetExpressCheckoutRequestDetailsType();
        //    ecDetails.ReturnURL = "https://devtools-paypal.com/guide/expresscheckout/dotnet?success=true";
        //    ecDetails.CancelURL = "https://devtools-paypal.com/guide/expresscheckout/dotnet?cancel=true";
        //    ecDetails.PaymentDetails = paymentDetails;

        //    SetExpressCheckoutRequestType request = new SetExpressCheckoutRequestType();
        //    request.Version = "104.0";
        //    request.SetExpressCheckoutRequestDetails = ecDetails;

        //    SetExpressCheckoutReq wrapper = new SetExpressCheckoutReq();
        //    wrapper.SetExpressCheckoutRequest = request;

        //    Dictionary<string, string> config = PayPal.Manager.ConfigManager.Instance.GetProperties();

        //    PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(config);
        //    SetExpressCheckoutResponseType setECResponse = service.SetExpressCheckout(wrapper);

        //    return setECResponse.Token;
        //}


        public string DoExpress(string token, string payerId, string OrderTotal)
        {
            DoExpressCheckoutPaymentRequestType request = new DoExpressCheckoutPaymentRequestType();
            request.Version = "104.0";
            DoExpressCheckoutPaymentRequestDetailsType requestDetails = new DoExpressCheckoutPaymentRequestDetailsType();

            requestDetails.Token = token;
            requestDetails.PayerID = payerId;

            request.DoExpressCheckoutPaymentRequestDetails = requestDetails;

            PaymentDetailsType paymentDetail = new PaymentDetailsType();
            paymentDetail.PaymentAction = PaymentActionCodeType.SALE;
            paymentDetail.OrderTotal = new BasicAmountType((CurrencyCodeType)EnumUtils.GetValue("PHP", typeof(CurrencyCodeType)), OrderTotal);
            

            SellerDetailsType seller = new SellerDetailsType();
            seller.PayPalAccountID = PayPal.Manager.ConfigManager.Instance.GetProperties()["sellerPaypalAccnt"];
            paymentDetail.SellerDetails = seller;

            List<PaymentDetailsType> paymentDetails = new List<PaymentDetailsType>();
            paymentDetails.Add(paymentDetail);

            requestDetails.PaymentDetails = paymentDetails;
            
            DoExpressCheckoutPaymentReq wrapper = new DoExpressCheckoutPaymentReq();
            wrapper.DoExpressCheckoutPaymentRequest = request;

            Dictionary<string, string> config = PayPal.Manager.ConfigManager.Instance.GetProperties();

            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(config);
            DoExpressCheckoutPaymentResponseType doECResponse = service.DoExpressCheckoutPayment(wrapper);

            if (doECResponse != null)
            {
                // Response envelope acknowledgement
                string acknowledgement = "DoExpressCheckoutPayment API Operation - ";
                acknowledgement += doECResponse.Ack.ToString();

             //   Console.WriteLine(acknowledgement + "\n");

                // # Success values
                if (doECResponse.Ack.ToString().Trim().ToUpper().Equals("SUCCESS"))
                {
                    // Transaction identification number of the transaction that was
                    // created.
                    // This field is only returned after a successful transaction
                    // for DoExpressCheckout has occurred.
                    if (doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo != null)
                    {
                        IEnumerator<PaymentInfoType> paymentInfoIterator = doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo.GetEnumerator();
                        while (paymentInfoIterator.MoveNext())
                        {
                            PaymentInfoType paymentInfo = paymentInfoIterator.Current;
                            //   logger.Info("Transaction ID : " + paymentInfo.TransactionID + "\n");
                          //  Console.WriteLine("Transaction ID : " + paymentInfo.TransactionID + "\n");
                            return paymentInfo.TransactionID;
                        }
                    }
                }
                // # Error Values
                else
                {
                    List<ErrorType> errorMessages = doECResponse.Errors;
                    foreach (ErrorType error in errorMessages)
                    {
                        //  logger.Debug("API Error Message : " + error.LongMessage);
                        Console.WriteLine("API Error Message : " + error.LongMessage + "\n");
                    }
                }


            }
            return "error";
        }

        public string GetPayPalResponse(Dictionary<string, string> formVals, bool useSandbox)
        {

            string paypalUrl = useSandbox ? "https://www.sandbox.paypal.com/cgi-bin/webscr"
                : "https://www.paypal.com/cgi-bin/webscr";


            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(paypalUrl);

            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            byte[] param = HttpContext.Current.Request.BinaryRead(HttpContext.Current.Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);

            StringBuilder sb = new StringBuilder();
            sb.Append(strRequest);

            foreach (string key in formVals.Keys)
            {
                sb.AppendFormat("&{0}={1}", key, formVals[key]);
            }
            strRequest += sb.ToString();
            req.ContentLength = strRequest.Length;

            //for proxy
            //WebProxy proxy = new WebProxy(new Uri("http://urlort#");
            //req.Proxy = proxy;
            //Send the request to PayPal and get the response
            string response = "";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII))
            {
                streamOut.Write(strRequest);
                streamOut.Close();
                using (StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    response = streamIn.ReadToEnd();
                }
            }

            return response;
        }

       public bool AmountPaidIsValid(Order order, decimal amountPaid)
        {

            //pull the order
            bool result = true;

            if (order != null)
            {
                if (order.Total > amountPaid)
                {
                    //_logger.Warn("Invalid order amount to PDT/IPN: " + order.ID + "; Actual: " + amountPaid.ToString("C") + "; Should be: " + order.Total.ToString("C") + "user IP is " + Request.UserHostAddress);
                    result = false;
                }
            }
            else
            {
                //_logger.Warn("Invalid order ID passed to PDT/IPN; user IP is " + Request.UserHostAddress);
            }
            return result;

        }

        //public string IPNvalidator(string message)
        //{
        //    string result = message;

        //    //string strSandbox = "https://www.sandbox.paypal.com/cgi-bin/webscr";
        //    //string strLive = "https://www.paypal.com/cgi-bin/webscr";
        //    string url = ConfigurationManager.AppSettings["PayPalUrl"];
        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

        //    //Set values for the request back
        //    req.Method = "POST";  
        //    req.ContentType = "application/x-www-form-urlencoded";
        //    byte[] param = Request.BinaryRead(HttpContext.Current.Request.ContentLength);
        //    string strRequest = Encoding.ASCII.GetString(param);
        //    strRequest += "&cmd=_notify-validate";
        //    req.ContentLength = strRequest.Length;


        //    //Send the request to PayPal and get the response
        //    StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
        //    streamOut.Write(strRequest);

        //    streamOut.Close();
        //    StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
        //    string strResponse = streamIn.ReadToEnd();
        //    streamIn.Close();

        //    if (strResponse == "VERIFIED")
        //    {
        //        // strRequest is a long string delimited by '&'
        //        string[] responseArray = strRequest.Split('&');

        //        List<KeyValuePair<string, string>> lkvp = new List<KeyValuePair<string, string>>();

        //        string[] temp;

        //        // for each key value pair
        //        foreach (string i in responseArray)
        //        {
        //            temp = i.Split('=');
        //            lkvp.Add(new KeyValuePair<string, string>(temp[0], temp[1]));
        //        }

        //        // now we have a list of key value pairs
        //        string firstName = string.Empty;
        //        string lastName = string.Empty;
        //        string address = string.Empty;
        //        string city = string.Empty;
        //        string state = string.Empty;
        //        string zip = string.Empty;
        //        string payerEmail = string.Empty;
        //        string contactPhone = string.Empty;

        //        foreach (KeyValuePair<string, string> kvp in lkvp)
        //        {
        //            switch (kvp.Key)
        //            {
        //                case "payer_email":
        //                    payerEmail = kvp.Value.Replace("%40", "@");
        //                    break;
        //                case "first_name":
        //                    firstName = kvp.Value;
        //                    break;
        //                case "last_name":
        //                    lastName = kvp.Value;
        //                    break;
        //                case "address_city":
        //                    city = kvp.Value.Replace("+", " ");
        //                    break;
        //                case "address_state":
        //                    state = kvp.Value.Replace("+", " ");
        //                    break;
        //                case "address_street":
        //                    address = kvp.Value.Replace("+", " ");
        //                    break;
        //                case "address_zip":
        //                    zip = kvp.Value;
        //                    break;
        //                case "contact_phone":
        //                    contactPhone = kvp.Value;
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

        //        //string userName = payerEmail;
        //        //string password = Membership.GeneratePassword(8, 0);

        //        //MembershipCreateStatus status = new MembershipCreateStatus();
        //        //MembershipUser newUser = Membership.CreateUser(userName, password, userName, null, null, true, out status);

        //        //ProfileCommon pc = ProfileCommon.Create(userName) as ProfileCommon;
        //        //pc.Address.PostalCode = zip;
        //        //pc.Address.Address = address;
        //        //pc.Address.City = city;
        //        //pc.Address.State = state;
        //        //pc.Personal.FirstName = firstName;
        //        //pc.Personal.LastName = lastName;
        //        //pc.Contacts.DayPhone = contactPhone;
        //        //pc.Save();

        //        //if (status == MembershipCreateStatus.Success)
        //        //{
        //        //    Roles.AddUserToRole(userName, "User");

        //        //    //send email to user indicating username and password
        //        //    SendEmailToUser(userName, password, firstName, lastName, payerEmail);
        //        //}

        //        // need to figure out a way to catch unwanted responses here... redirect somehow
        //    }
        //    else if (strResponse == "INVALID")
        //    {
        //        //log for manual investigation
        //    }
        //    else
        //    {
        //        //log response/ipn data for manual investigation
        //    }


        //    return result;
        //}

    }



}
