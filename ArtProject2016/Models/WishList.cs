﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class WishList
    {
        public WishList()
        {
            listName = "Default Wish List";
        }

        public int Id { get; set; }
        public string listName { get; set; }
        public DateTime dateAdded { get; set; }

        public int ForSaleId { get; set; }
        public virtual ForSale ForSale { get; set; }

        public int UserAccountId { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}