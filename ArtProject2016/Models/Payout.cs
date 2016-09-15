using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class Payout
    {
        public Payout()
        {
            Status = "Processing Payout";
            Paid = false;
        }

        public int Id { get; set; }

        [Required]
        public string PayOutMethod { get; set; } // paypal check etc.
        [Required]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal RedeemedPayoutAmt { get; set; }

        [Required]
        public string FullName { get; set; }
        [Required]
        public string PaymentInfo { get; set; } //bank info,etc.

        public string Status { get; set; } //Processing or Paid
        public DateTime DateRequested { get; set; }

        public DateTime? DateGiven { get; set; }
        public bool Paid { get; set; }

        public int UserAccountId { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}