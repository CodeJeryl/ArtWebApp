using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtProject2016.Models;

namespace ArtProject2016.ViewModel
{
    public class ShopLayoutViewModel
    {
        public List<Category>  Categories { get; set; }
        public List<UserAccount> UserAccounts { get; set; }
    }
}