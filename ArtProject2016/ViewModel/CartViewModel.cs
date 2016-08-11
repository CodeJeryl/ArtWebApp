using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ArtProject2016.Models;

namespace ArtProject2016.ViewModel
{
    public class CartViewModel
    {
        public List<Cart> CartItems { get; set; }
        public int CartCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal SubTotal { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal CartTotal { get; set; }
    }
}