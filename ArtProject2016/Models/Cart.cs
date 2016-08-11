using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int Qty { get; set; }

        public DateTime dateCreated { get; set; }

        public int ForSaleId { get; set; }
        public virtual ForSale ForSale { get; set; }

        public int UserAccountId { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}