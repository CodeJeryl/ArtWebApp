﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Models;

namespace ArtProject2016.ViewModel
{
    public class uploadViewModel
    {
        public uploadViewModel()
        {
         //   ForSaleAlbumCollection = new List<ForSaleAlbum>();
        }

        public virtual ForSale ForSale { get; set; }
       // public virtual ForSaleAlbum ForSaleAlbum { get; set; }
      //  public virtual ICollection<ForSaleAlbum> ForSaleAlbumCollection { get; set; }

        [Required]
        public string selectedYear { get; set; }

        public IEnumerable<SelectListItem> Years
        {
            get { return new SelectList(Enumerable.Range(1950, DateTime.Now.Year - 1949).ToList()); }

        }

        public List<Style> _Styles { get; set; }
        public int selectedStyleId { get; set; }

        public IEnumerable<SelectListItem> Styles
        {
            get
            {
                return new SelectList(_Styles, "Id", "name");
            }

        }

        //public uploadViewModel()
        //{
        //    // IEnumerable<SelectListItem>  Categories = new List<SelectListItem>();
        //}
    }
}