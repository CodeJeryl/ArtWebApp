using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string name { get; set; }

        [DataType(DataType.Date)]
        public DateTime? dateAdded { get; set; }

        public ICollection<ForSale> ForSales { get; set; }


    }
}