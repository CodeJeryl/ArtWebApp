using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtProject2016.Models;

namespace ArtProject2016.Functions
{
    public class ShopControls
    {
        private ArtContext db;

        public ShopControls()
        {
            db = new ArtContext();
        }

        public IQueryable<ForSale>  ForSales(ForSale forSale)
        {
            var result = db.ForSales.AsQueryable();

            return result;
        }
    }
}