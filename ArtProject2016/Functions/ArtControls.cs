using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;
using WebMatrix.WebData;

namespace ArtProject2016.Functions
{
    public class ArtControls
    {
        public static bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".jpeg" }; // add more if u like...

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        public static bool UploadArt(uploadViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                using (var context = new ArtContext())
                {
                    var artUpload = context.ForSales.Add(new ForSale()
                    {
                        Title = model.ForSale.Title,
                        mediumUsed = model.ForSale.mediumUsed,
                        wSize = model.ForSale.wSize,
                        hSize = model.ForSale.hSize,
                        ArtistPrice = model.ForSale.ArtistPrice,
                        Profit = model.ForSale.Profit,
                        ShippingFee = model.ForSale.ShippingFee,
                        Price = model.ForSale.Price,
                        yearCreated = model.selectedYear,
                        Framed = model.ForSale.Framed,
                        otherArtist = model.ForSale.otherArtist,
                        otherArtistName = model.ForSale.otherArtistName,
                        otherArtistAddress = model.ForSale.otherArtistAddress,
                        artDescription = model.ForSale.artDescription,
                        datePosted = DateTime.Now,
                        SellerId = WebSecurity.CurrentUserId,
                        CategoryID = model.selectedCategoryId

                    });

                  

                    if (files.Count(file => file != null && file.ContentLength > 0) > 0)
                    {
                        foreach (var file in files)
                        {
                            file.SaveAs(HttpContext.Current.Server.MapPath("~/Upload/Pics/" + file.FileName));
                            artUpload.fileName = file.FileName;
                            artUpload.Path = "~/Upload/Pics/" + file.FileName;
                        }
                    }

                    context.ForSales.Add(artUpload);
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