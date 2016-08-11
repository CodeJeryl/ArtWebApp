using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtProject2016.ViewModel
{
    public class CartRemoveViewModel
    {
        public string Message { get; set; }
      ////  public decimal SubTotal { get; set; }
        public string CartTotal { get; set; }
        public int CartCount { get; set; }
        public int ItemCount { get; set; }
        public int DeleteId { get; set; }
    }
}