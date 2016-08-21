using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace ArtProject2016.Functions
{
    public class EmailControls
    {
        private string PopulateBody(string Fname)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplate/Register.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{FirstName}", Fname);
            //body = body.Replace("{password}", password);
            return body;
        }


        public static void sendEmail(string sender, string recepient, string bcc, string cc, string subject, string body)
        {
            MailMessage email = new MailMessage();
            email.From = new MailAddress(sender);
            email.To.Add(new MailAddress(recepient));
            if (bcc != null && bcc != "") email.Bcc.Add(new MailAddress(bcc));
            if (cc != null && bcc != "") email.CC.Add(new MailAddress(cc));

            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = true;
            email.Priority = MailPriority.Normal;
            SmtpClient client = new SmtpClient();
             client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(sender, "qwerqwer21");
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.Send(email);

            //client.Credentials = new System.Net.NetworkCredential(sender, "");
            //client.Port = 587;
            //client.Host = "smtp.gmail.com";
            //client.Send(email);

           // string mess = this.PopulateBody(red1.GetInt32(0).ToString(), y.ToString());
          
          //  sendEmail("JerylSuarez@letranbataan.edu.ph", red1.GetString(2), "", "", "Letran Bataan Student Portal: Forgot Password", mess);
                           
        }
    }


}