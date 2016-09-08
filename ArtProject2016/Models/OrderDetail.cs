using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
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

        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }
        public bool Returned { get; set; }

        [Display(Name = "Shipping Company")]
        public string RetrunShippingCompany { get; set; }
        [Display(Name = "Return Tracking Number")]
        public string ReturnedTrackingNumber { get; set; }

        public bool BuyerReceived { get; set; }
        public DateTime? BuyerReceivedDateTime { get; set; }

        public bool ReadyToRedeem { get; set; }
        public bool Redeemed { get; set; }
           [Display(Name = "Price Adjustment")]
        public decimal PriceAdjustment { get; set; }

        public int ForSaleId { get; set; }
        public virtual ForSale ForSale { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public ICollection<OrderTracking> OrderTrackings { get; set; }

        [Display(Name = "Shipping Company")]
        public int? ShippingCompanyId { get; set; }
        public virtual ShippingCompany ShippingCompany { get; set; }


        //walaaaaaaaaa hahahah IDK

        //private decimal shipfee;

       // public readonly Expression<Func<OrderDetail, decimal>> SumShipping = c => c.PriceAdjustment + c.ForSale.ShippingFee;

        //[NotMapped]
        //public decimal SumShippingAdjustment
        //{
        //    get { return (shipfee + PriceAdjustment); }
        //  //  private set { shipfee = ForSale.ShippingFee; }

        //}
    }
}