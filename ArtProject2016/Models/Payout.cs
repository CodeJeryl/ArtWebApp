﻿using System;
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
        }

        public int Id { get; set; }

        [Required]
        public string PayOutMethod { get; set; } // paypal check etc.
        [Required]
        public decimal RedeemedPayout { get; set; }
        public string Status { get; set; } //Processing or Paid
        public DateTime DateTime { get; set; }

        public int UserAccountsId { get; set; }
        public ICollection<UserAccount> UserAccounts { get; set; }
    }
}