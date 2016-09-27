using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtProject2016.Models;

namespace ArtProject2016.ViewModel
{
    public class galleryViewModel
    {
        public virtual ForSale ForSale { get; set; }
        public virtual IEnumerable<ForSale> relatedForSale  { get; set; }
        public virtual IEnumerable<ForSale> RelatedStyle { get; set; }
    }
}