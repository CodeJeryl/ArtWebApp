using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class ForSaleAlbum
    {
        public int Id { get; set; }
        public string fileName { get; set; }
        public string Path { get; set; }
        public string active { get; set; }
        public DateTime? datePosted { get; set; }

        //FK
        public int ForSaleId { get; set; }
        public virtual ForSale  ForSale { get; set; }
    }
}