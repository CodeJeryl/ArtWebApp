using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;
using WebMatrix.WebData;
using System.Drawing;
using System.Drawing.Drawing2D;

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
                        ForPosting = true,
                        artDescription = model.ForSale.artDescription,
                        datePosted = DateTime.Now,
                        SellerId = WebSecurity.CurrentUserId,
                        CategoryID = model.selectedCategoryId

                    });

                    ArtControls art = new ArtControls();
                    string path, FileName;
                    
                    if (files.Count(file => file != null && file.ContentLength > 0) > 0)
                    {
                        foreach (var file in files)
                        {
                            FileName = DateTime.Now.ToString("yyyyMMdd")+model.ForSale.Title+DateTime.Now.Ticks+Path.GetExtension(file.FileName);
                            path = HttpContext.Current.Server.MapPath("~/Upload/Pics/" + FileName);
                            art.ResizeStream(600,file.InputStream, path);
                            
                            //file.SaveAs(HttpContext.Current.Server.MapPath("~/Upload/Pics/" + file.FileName));
                            artUpload.fileName = FileName;
                            artUpload.Path = "~/Upload/Pics/" + FileName;
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


        public void ResizeStream(int imageSize, Stream filePath, string outputPath)
        {
            var image = Image.FromStream(filePath);

            int thumbnailSize = imageSize;
            int newWidth, newHeight;

            if (image.Width > image.Height)
            {
                newWidth = thumbnailSize;
                newHeight = image.Height*thumbnailSize/image.Width;
            }
            else
            {
                newWidth = image.Width*thumbnailSize/image.Height;
                newHeight = thumbnailSize;
            }

            var thumbnailBitmap = new Bitmap(newWidth, newHeight);

            var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            thumbnailGraph.DrawImage(image, imageRectangle);

            thumbnailBitmap.Save(outputPath, image.RawFormat);
            thumbnailGraph.Dispose();
            thumbnailBitmap.Dispose();
            image.Dispose();
        }
    }
}