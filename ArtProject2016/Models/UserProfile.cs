using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class UserProfile
    {
        //public UserProfile()
        //{
        //    idVerified = false;
        //    emailVerified = false;
        //}

        [Key, ForeignKey("UserAccount")]
        public int UserAccountId { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? birthDay { get; set; }
        public string education { get; set; }

        //address
       
        public string street { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string postalCode { get; set; }
        
        public string mobileNo { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string landLine { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Profile Description")]
        public string profileDesc { get; set; }

        [DataType(DataType.MultilineText)]
        public string Exhibitions { get; set; }

        public bool isEmailVerified { get; set; }

        public bool isIdVerified { get; set; }
        public string fileName { get; set; }
        public string Path { get; set; }

        //FK
        public virtual UserAccount UserAccount { get; set; }

    }
}