using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class ShippingCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}