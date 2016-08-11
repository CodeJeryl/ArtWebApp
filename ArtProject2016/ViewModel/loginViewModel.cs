using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtProject2016.ViewModel
{
    public class loginViewModel
    {
        [Required]
        public string userName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

    }
}