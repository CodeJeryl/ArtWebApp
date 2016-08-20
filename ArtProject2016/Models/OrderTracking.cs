using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class OrderTracking
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string StatusType { get; set; }
        public string Description { get; set; }

        public int OrderDetailId { get; set; }
        public virtual OrderDetail  OrderDetail { get; set; }

    }
}