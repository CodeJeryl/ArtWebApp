using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ArtProject2016.Models;

namespace ArtProject2016.ViewModel
{
    public class PayoutViewModel
    {
        public virtual List<OrderDetail> PendingOrderDetails { get; set; }
        public virtual List<OrderDetail>  RedeemOrderDetails { get; set; }
        public virtual List<OrderDetail> RedeemedOrderDetails  { get; set; }
        public virtual ICollection<Payout> Payouts { get; set; }


        public decimal PendingAmt { get; set; }
        public decimal RedeemableAmt { get; set; }
        public decimal RedeemedAmt { get; set; }
    }
}