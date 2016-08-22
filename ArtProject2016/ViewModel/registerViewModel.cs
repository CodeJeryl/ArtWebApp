using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtProject2016.ViewModel
{
    public class registerViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Please input valid Email Address")]
        public string userName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("password",ErrorMessage = "Password did not match")]
        public string RetypePassword { get; set; }

        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        [Required]
        public string nickName { get; set; }

        [Required]
        public string userType { get; set; }
    }
}