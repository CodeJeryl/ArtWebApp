using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtProject2016.ViewModel
{
    public class checkoutDetailsViewModel
    {
        public checkoutDetailsViewModel()
        {
            VoucherDeduction = 0;
            //   DetailsChecked = false;
        }


       //dsdssd
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string MobileNo { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string LandLine { get; set; }
         [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal VoucherDeduction { get; set; }

         [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
         public decimal SubTotal { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal CartTotal { get; set; }

     //   public bool DetailsChecked { get; set; }
    }
}