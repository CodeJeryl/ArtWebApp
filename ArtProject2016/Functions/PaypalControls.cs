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
        public string PaypalExpress(CheckoutViewModel viewModel)
        {
            using (ArtContext db = new ArtContext())
            {

                var cartItems = db.Carts.Where(items => items.UserAccountId == WebSecurity.CurrentUserId).ToList();
                var userAccount = db.UserAccounts.Single(user => user.Id == WebSecurity.CurrentUserId);
                var userProfile = db.UserProfiles.Single(user => user.UserAccountId == WebSecurity.CurrentUserId);

                viewModel.UserAccount = userAccount;
                viewModel.UserProfile = userProfile;
                viewModel.CartItems = cartItems;


                PaymentDetailsType paymentDetail = new PaymentDetailsType();
                CurrencyCodeType currency = (CurrencyCodeType)EnumUtils.GetValue("PHP", typeof(CurrencyCodeType));
                PaymentDetailsItemType paymentItem = new PaymentDetailsItemType();
                paymentItem.Name = cartItems[0].ForSale.Title;
                decimal itemAmount = 10000;  //Regex.Replace(cartItems[0].ForSale.Price.ToString(), "[^0-9.]", "");
                paymentItem.Amount = new BasicAmountType(currency, itemAmount.ToString());
                int itemQuantity = cartItems[0].Qty;
                paymentItem.Quantity = itemQuantity;
                // paymentItem.ItemCategory = (ItemCategoryType)EnumUtils.GetValue(, typeof(ItemCategoryType));
                List<PaymentDetailsItemType> paymentItems = new List<PaymentDetailsItemType>();
                paymentItems.Add(paymentItem);
                paymentDetail.PaymentDetailsItem = paymentItems;

                paymentDetail.PaymentAction = (PaymentActionCodeType)EnumUtils.GetValue("Sale", typeof(PaymentActionCodeType));
                paymentDetail.OrderTotal = new BasicAmountType((CurrencyCodeType)EnumUtils.GetValue("PHP", typeof(CurrencyCodeType)), (itemAmount * itemQuantity).ToString());
                List<PaymentDetailsType> paymentDetails = new List<PaymentDetailsType>();
                paymentDetails.Add(paymentDetail);

                SetExpressCheckoutRequestDetailsType ecDetails = new SetExpressCheckoutRequestDetailsType();
                ecDetails.ReturnURL = "http://localhost:60817/Shop/PayPalCheckout?success=true";
                ecDetails.CancelURL = "http://localhost:60817/Shop/PayPalCheckout?success=false";
                ecDetails.PaymentDetails = paymentDetails;

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
                Dictionary<string, string> sdkConfig = new Dictionary<string, string>();
                sdkConfig.Add("mode", "security-test-sandbox");
                sdkConfig.Add("account1.apiUsername", "jeryl.suarez-facilitator_api1.yahoo.com");
                sdkConfig.Add("account1.apiPassword", "BGX735ARZFAS95ZN");
                sdkConfig.Add("account1.apiSignature", "A2QoVdXW-3NyIOsjGGBweDnke5g2A3YC3DckQi-iAkuUZnpi4KveJ.7-");
                sdkConfig.Add("account1.applicationId", "APP-80W284485P519543T");
                PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(sdkConfig);

                //Dictionary<string, string> config = PayPal.Manager.ConfigManager.Instance.GetProperties();
                //// Create the Classic SDK service instance to use.
                //PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(config);
                //config
                SetExpressCheckoutResponseType setECResponse = new SetExpressCheckoutResponseType();
                setECResponse = service.SetExpressCheckout(wrapper);

                if (setECResponse != null)
                {
                    // Response envelope acknowledgement
                    string acknowledgement = "SetExpressCheckout API Operation - ";
                    acknowledgement += setECResponse.Ack.ToString();
                    // logger.Info(acknowledgement + "\n");
                    Console.WriteLine(acknowledgement + "\n");

                    // # Success values
                    if (setECResponse.Ack.ToString().Trim().ToUpper().Equals("SUCCESS"))
                    {
                        // # Redirecting to PayPal for authorization
                        // Once you get the "Success" response, needs to authorise the
                        // transaction by making buyer to login into PayPal. For that,
                        // need to construct redirect url using EC token from response.
                        // For example,
                       string redirectURL="https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token="+setECResponse.Token;

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

        public string pay()
        {
            PaymentDetailsType paymentDetail = new PaymentDetailsType();
            CurrencyCodeType currency = (CurrencyCodeType)EnumUtils.GetValue("USD", typeof(CurrencyCodeType));
            PaymentDetailsItemType paymentItem = new PaymentDetailsItemType();
            paymentItem.Name = "item";
            double itemAmount = 1.00;
            paymentItem.Amount = new BasicAmountType(currency, itemAmount.ToString());
            int itemQuantity = 1;
            paymentItem.Quantity = itemQuantity;
            //paymentItem.ItemCategory = (ItemCategoryType)EnumUtils.GetValue(, typeof(ItemCategoryType));
            List<PaymentDetailsItemType> paymentItems = new List<PaymentDetailsItemType>();
            paymentItems.Add(paymentItem);
            paymentDetail.PaymentDetailsItem = paymentItems;

            paymentDetail.PaymentAction = (PaymentActionCodeType)EnumUtils.GetValue("Sale", typeof(PaymentActionCodeType));
            paymentDetail.OrderTotal = new BasicAmountType((CurrencyCodeType)EnumUtils.GetValue("USD", typeof(CurrencyCodeType)), (itemAmount * itemQuantity).ToString());
            List<PaymentDetailsType> paymentDetails = new List<PaymentDetailsType>();
            paymentDetails.Add(paymentDetail);

            SetExpressCheckoutRequestDetailsType ecDetails = new SetExpressCheckoutRequestDetailsType();
            ecDetails.ReturnURL = "https://devtools-paypal.com/guide/expresscheckout/dotnet?success=true";
            ecDetails.CancelURL = "https://devtools-paypal.com/guide/expresscheckout/dotnet?cancel=true";
            ecDetails.PaymentDetails = paymentDetails;

            SetExpressCheckoutRequestType request = new SetExpressCheckoutRequestType();
            request.Version = "104.0";
            request.SetExpressCheckoutRequestDetails = ecDetails;

            SetExpressCheckoutReq wrapper = new SetExpressCheckoutReq();
            wrapper.SetExpressCheckoutRequest = request;
            Dictionary<string, string> sdkConfig = new Dictionary<string, string>();
            sdkConfig.Add("mode", "security-test-sandbox");
            sdkConfig.Add("account1.apiUsername", "jb-us-seller_api1.paypal.com");
            sdkConfig.Add("account1.apiPassword", "WX4WTU3S8MY44S7F");
            sdkConfig.Add("account1.apiSignature", "AFcWxV21C7fd0v3bYYYRCpSSRl31A7yDhhsPUU2XhtMoZXsWHFxu-RWy");
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(sdkConfig);
            SetExpressCheckoutResponseType setECResponse = service.SetExpressCheckout(wrapper);

            return setECResponse.Token;
        }
    }




    public class NVPAPICaller
    {
        //Flag that determines the PayPal environment (live or sandbox)
        private const bool bSandbox = true;
        private const string CVV2 = "CVV2";

        // Live strings.
        private string pEndPointURL = "https://api-3t.paypal.com/nvp";
        private string host = "www.paypal.com";

        // Sandbox strings.
        private string pEndPointURL_SB = "https://api-3t.sandbox.paypal.com/nvp";
        private string host_SB = "www.sandbox.paypal.com";

        private const string SIGNATURE = "SIGNATURE";
        private const string PWD = "PWD";
        private const string ACCT = "ACCT";

        //Replace <Your API Username> with your API Username
        //Replace <Your API Password> with your API Password
        //Replace <Your Signature> with your Signature
        public string APIUsername = "jeryl.suarez-facilitator_api1.yahoo.com";
        private string APIPassword = "BGX735ARZFAS95ZN";
        private string APISignature = "A2QoVdXW-3NyIOsjGGBweDnke5g2A3YC3DckQi-iAkuUZnpi4KveJ.7-";
        private string Subject = "";
        private string BNCode = "PP-ECWizard";


        //HttpWebRequest Timeout specified in milliseconds 
        private const int Timeout = 15000;
        private static readonly string[] SECURED_NVPS = new string[] { ACCT, CVV2, SIGNATURE, PWD };

        public void SetCredentials(string Userid, string Pwd, string Signature)
        {
            APIUsername = Userid;
            APIPassword = Pwd;
            APISignature = Signature;
        }

        public bool ShortcutExpressCheckout(string amt, ref string token, ref string retMsg)
        {
            if (bSandbox)
            {
                pEndPointURL = pEndPointURL_SB;
                host = host_SB;
            }

            string returnURL = "https://localhost:60817/Shop/PaypalCheckout";
            string cancelURL = "https://localhost:60817/Shop/PaypalCancel";

            NVPCodec encoder = new NVPCodec();
            encoder["METHOD"] = "SetExpressCheckout";
            encoder["RETURNURL"] = returnURL;
            encoder["CANCELURL"] = cancelURL;
            encoder["BRANDNAME"] = "Wingtip Toys Sample Application";
            encoder["PAYMENTREQUEST_0_AMT"] = amt;
            encoder["PAYMENTREQUEST_0_ITEMAMT"] = amt;
            encoder["PAYMENTREQUEST_0_PAYMENTACTION"] = "Sale";
            encoder["PAYMENTREQUEST_0_CURRENCYCODE"] = "PHP";

            // Get the Shopping Cart Products
            using (ArtContext db = new ArtContext())
            {

                var myOrderList =
                    db.Carts.Where(item => item.UserAccountId == WebSecurity.CurrentUserId).ToList();

                for (int i = 0; i < myOrderList.Count; i++)
                {
                    encoder["L_PAYMENTREQUEST_0_NAME" + i] = myOrderList[i].ForSale.Title;//myOrderList[i].Product.ProductName.ToString();
                    encoder["L_PAYMENTREQUEST_0_AMT" + i] = myOrderList[i].ForSale.Price.ToString();
                    encoder["L_PAYMENTREQUEST_0_QTY" + i] = myOrderList[i].Qty.ToString();
                }
            }

            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            NVPCodec decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                token = decoder["TOKEN"];
                string ECURL = "https://" + host + "/cgi-bin/webscr?cmd=_express-checkout" + "&token=" + token;
                retMsg = ECURL;
                return true;
            }
            else
            {
                retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];
                return false;
            }
        }

        public bool GetCheckoutDetails(string token, ref string PayerID, ref NVPCodec decoder, ref string retMsg)
        {
            if (bSandbox)
            {
                pEndPointURL = pEndPointURL_SB;
            }

            NVPCodec encoder = new NVPCodec();
            encoder["METHOD"] = "GetExpressCheckoutDetails";
            encoder["TOKEN"] = token;

            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                PayerID = decoder["PAYERID"];
                return true;
            }
            else
            {
                retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];

                return false;
            }
        }

        public bool DoCheckoutPayment(string finalPaymentAmount, string token, string PayerID, ref NVPCodec decoder, ref string retMsg)
        {
            if (bSandbox)
            {
                pEndPointURL = pEndPointURL_SB;
            }

            NVPCodec encoder = new NVPCodec();
            encoder["METHOD"] = "DoExpressCheckoutPayment";
            encoder["TOKEN"] = token;
            encoder["PAYERID"] = PayerID;
            encoder["PAYMENTREQUEST_0_AMT"] = finalPaymentAmount;
            encoder["PAYMENTREQUEST_0_CURRENCYCODE"] = "PHP";
            encoder["PAYMENTREQUEST_0_PAYMENTACTION"] = "Sale";

            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                return true;
            }
            else
            {
                retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];

                return false;
            }
        }

        public string HttpCall(string NvpRequest)
        {
            string url = pEndPointURL;

            string strPost = NvpRequest + "&" + buildCredentialsNVPString();
            strPost = strPost + "&BUTTONSOURCE=" + HttpUtility.UrlEncode(BNCode);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Timeout = Timeout;
            objRequest.Method = "POST";
            objRequest.ContentLength = strPost.Length;

            try
            {
                using (StreamWriter myWriter = new StreamWriter(objRequest.GetRequestStream()))
                {
                    myWriter.Write(strPost);
                }
            }
            catch (Exception)
            {
                // No logging for this tutorial.
            }

            //Retrieve the Response returned from the NVP API call to PayPal.
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            string result;
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }

        private string buildCredentialsNVPString()
        {
            NVPCodec codec = new NVPCodec();

            if (!IsEmpty(APIUsername))
                codec["USER"] = APIUsername;

            if (!IsEmpty(APIPassword))
                codec[PWD] = APIPassword;

            if (!IsEmpty(APISignature))
                codec[SIGNATURE] = APISignature;

            if (!IsEmpty(Subject))
                codec["SUBJECT"] = Subject;

            codec["VERSION"] = "88.0";

            return codec.Encode();
        }

        public static bool IsEmpty(string s)
        {
            return s == null || s.Trim() == string.Empty;
        }
    }

    public sealed class NVPCodec : NameValueCollection
    {
        private const string AMPERSAND = "&";
        private const string EQUALS = "=";
        private static readonly char[] AMPERSAND_CHAR_ARRAY = AMPERSAND.ToCharArray();
        private static readonly char[] EQUALS_CHAR_ARRAY = EQUALS.ToCharArray();

        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            bool firstPair = true;
            foreach (string kv in AllKeys)
            {
                string name = HttpUtility.UrlEncode(kv);
                string value = HttpUtility.UrlEncode(this[kv]);
                if (!firstPair)
                {
                    sb.Append(AMPERSAND);
                }
                sb.Append(name).Append(EQUALS).Append(value);
                firstPair = false;
            }
            return sb.ToString();
        }

        public void Decode(string nvpstring)
        {
            Clear();
            foreach (string nvp in nvpstring.Split(AMPERSAND_CHAR_ARRAY))
            {
                string[] tokens = nvp.Split(EQUALS_CHAR_ARRAY);
                if (tokens.Length >= 2)
                {
                    string name = HttpUtility.UrlDecode(tokens[0]);
                    string value = HttpUtility.UrlDecode(tokens[1]);
                    Add(name, value);
                }
            }
        }

        public void Add(string name, string value, int index)
        {
            this.Add(GetArrayName(index, name), value);
        }

        public void Remove(string arrayName, int index)
        {
            this.Remove(GetArrayName(index, arrayName));
        }

        public string this[string name, int index]
        {
            get { return this[GetArrayName(index, name)]; }
            set { this[GetArrayName(index, name)] = value; }
        }

        private static string GetArrayName(int index, string name)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index cannot be negative : " + index);
            }
            return name + index;
        }


    }
}


