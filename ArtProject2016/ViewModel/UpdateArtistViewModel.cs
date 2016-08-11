using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtProject2016.Models;

namespace ArtProject2016.ViewModel
{
    public class UpdateArtistViewModel
    {
        public virtual UserAccount UserAccount { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}