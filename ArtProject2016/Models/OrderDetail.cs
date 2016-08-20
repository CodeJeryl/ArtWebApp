using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
       
        public int Quantity { get; set; }
        [Display(Name = "Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Status")]
        public string OrderDetailStatus { get; set; }
        public bool Shipped { get; set; }
            [Display(Name = "Shipping Company")]
        public string ShippingCompany { get; set; }
        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }
        public bool Returned { get; set; }

        [Display(Name = "Shipping Company")]
        public string RetrunShippingCompany { get; set; }
        [Display(Name = "Tracking Number")]
        public string ReturnedTrackingNumber { get; set; }

        public int ForSaleId { get; set; }
        public virtual ForSale ForSale { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public ICollection<OrderTracking> OrderTrackings { get; set; }
    }
}