using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class PaypalPayment
    {
        [Key]
        public int OrderId { get; set; }
        public string Token { get; set; }
        public string PayerId { get; set; }
        public string TransactionId { get; set; }
        public string Remarks { get; set; }

        public bool IPNverified { get; set; }
        public DateTime DateTime { get; set; }

        [Required]
        public virtual Order Order { get; set; }
    }
}