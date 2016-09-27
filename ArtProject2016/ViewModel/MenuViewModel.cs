using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtProject2016.Models;

namespace ArtProject2016.ViewModel
{
    public class MenuViewModel
    {
        public List<ForSale> ForSales { get; set; }
        public List<Style> Styles { get; set; }
    }
}