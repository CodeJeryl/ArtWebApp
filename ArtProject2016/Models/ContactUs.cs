using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class ContactUs
    {
        public int Id { get; set; }

        [Display(Name = "Order ID (If Applicable)")]
        public int? OrderId { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Please input a valid Email Address")]
        public string EmailAdd { get; set; }

        [Required]
        [Display(Name = "Mobile Number")]
        [MinLength(11, ErrorMessage = "Please Check your Mobile #, Minimum of 11 Numbers =)")]
        public string MobileNumber { get; set; }

        [Required]
        public string Subject { get; set; }


        [Required]
        [DataType(DataType.MultilineText)]
        [MinLength(20,ErrorMessage = "Please elaborate your message, Thank you! Minimum of 20 characters =)")]
        public string Message { get; set; }

    }
}