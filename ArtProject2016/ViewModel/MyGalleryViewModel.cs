using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtProject2016.Models;

namespace ArtProject2016.ViewModel
{
    public class MyGalleryViewModel
    {
        public virtual UserAccount UserAccount { get; set; }
        public List<ForSale> ForSales { get; set; }
        public List<UserAccount> ForSalesArtist { get; set; } 
    }
}