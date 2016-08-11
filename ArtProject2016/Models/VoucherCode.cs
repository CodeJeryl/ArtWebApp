using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class VoucherCode
    {
        public int Id { get; set; }
        public string VoucherName { get; set; }
        public decimal VoucherDeduction { get; set; }
        public int VoucherCount { get; set; }
        public decimal VoucherMinOrder { get; set; }
        public bool VoucherEnabled { get; set; }

        public IQueryable<Order> Orders { get; set; }
    }
}