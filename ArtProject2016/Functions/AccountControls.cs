using System;
using System.Collections.Generic;
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
                    var idUpload = context.UserProfiles.Single(user => user.Id == WebSecurity.CurrentUserId);

                    string fileName = WebSecurity.CurrentUserId + "-";// +DateTime.Now.ToShortDateString();

                    file.SaveAs(HttpContext.Current.Server.MapPath("~/Upload/VerificationID/" + fileName));
                    idUpload.fileName = fileName;
                    idUpload.Path = "~/Upload/VerificationID/" + fileName;
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