using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;
using WebMatrix.WebData;

namespace ArtProject2016.Functions
{
    public class AccountControls
    {
       


        public static bool UploadID(HttpPostedFileBase file)
        {
            try
            {
                using (var context = new ArtContext())
                {
                    var idUpload = context.UserProfiles.Single(user => user.UserAccountId == WebSecurity.CurrentUserId);

                    string fileName = WebSecurity.CurrentUserId + "-" +DateTime.Now.Ticks + Path.GetExtension(file.FileName); ;// +DateTime.Now.ToShortDateString();

                    file.SaveAs(HttpContext.Current.Server.MapPath("~/Upload/VerificationID/" + fileName));
                    idUpload.fileName = fileName;
                    idUpload.Path = "~/Upload/VerificationID/" + fileName;
                  //  idUpload.UserAccount = idUpload.UserAccount;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}