using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
       
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        

        public int ForSaleId { get; set; }
        public virtual ForSale ForSale { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}