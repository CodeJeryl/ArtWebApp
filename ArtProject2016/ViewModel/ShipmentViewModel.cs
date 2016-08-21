using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Models;

namespace ArtProject2016.ViewModel
{
    public class ShipmentViewModel
    {

        public virtual OrderDetail OrderDetails { get; set; }
       // public virtual ForSale  ForSale { get; set; }

       [Required(ErrorMessage = "Please input the tracking number provided by the shipping company")]
        public string Track { get; set; }


        public List<ShippingCompany> _ShippingCompany { get; set; }

       [Required(ErrorMessage = "Please Choose the Shipping Company you sent the Art with.")]
       public int SelectedShippingId { get; set; }

        public IEnumerable<SelectListItem> ShippingCompanies
        {
            get
            {
                return new SelectList(_ShippingCompany,"Id","Name");
            }
        }

         
    }
}
