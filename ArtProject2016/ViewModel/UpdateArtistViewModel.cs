using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ArtProject2016.Models;

namespace ArtProject2016.ViewModel
{
    public class UpdateArtistViewModel
    {
      
       
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        [Required]
        public string nickName { get; set; }
     
        public string userType { get; set; }

        public virtual UserProfile UserProfile { get; set; }
    }
}