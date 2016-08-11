using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ArtProject2016.Models;

namespace ArtProject2016.ViewModel
{
    public class CheckoutViewModel
    {
        public virtual UserAccount UserAccount { get; set; }
        public virtual UserProfile  UserProfile { get; set; }

        //[Required]
        //public string Street { get; set; }
        //[Required]
        //public string City { get; set; }
        //[Required]
        //public string Province { get; set; }
        //[Required]
        //public string PostalCode { get; set; }

        //[Required]
        //public string MobileNo { get; set; }
        //[DataType(DataType.PhoneNumber)]
        //public string LandLine { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal SubTotal { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Total { get; set; }

        public int VoucherCodeId { get; set; }
        public decimal VoucherDeduction { get; set; }
        [Required]
        public string PaymentType { get; set; }

        public virtual List<Cart> CartItems { get; set; }
    }
}