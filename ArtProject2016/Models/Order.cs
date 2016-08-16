using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string LandLine { get; set; }
        public string MobileNo { get; set; }

        [Required]
        public string PaymentType { get; set; }

     
        public decimal SubTotal { get; set; }
        public decimal VoucherDeduction { get; set; }
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }

        public string OrderStatus { get; set; }

        public int? VoucherCodeId { get; set; }
        public virtual VoucherCode VoucherCode { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }

    }
}