using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class ForSale
    {
        public ForSale()
        {
           // Sold = false;
           // ForPosting = false;
            Profit = 0;
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Medium")]
        public string mediumUsed { get; set; }

        [Required]
        [Display(Name = "Width")]
        public string wSize { get; set; }

        [Required]
        [Display(Name = "Height")]
        public string hSize { get; set; }

        [Required(ErrorMessage = "Please input price of your art")]
        [Display(Name = "Art Price")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal ArtistPrice { get; set; }

        [Required(ErrorMessage = "Please click the Compute button to remove this error")]
        [Display(Name = "Profit")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Profit { get; set; }

        [Required(ErrorMessage = "Please click the Compute button to remove this error")]
        [Display(Name = "Shipping Fee")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal ShippingFee { get; set; }

        [Required(ErrorMessage = "Please click the Compute button to remove this error")]
        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        
        [Display(Name = "Artwork Framed?")]
        public bool Framed { get; set; }
        
        [Display(Name= "Year Created")]
        public string yearCreated { get; set; }
        
        [Required]
        [StringLength(5000,MinimumLength = 50,ErrorMessage = "Minimum of 50 characters, Tell a story :)")]
        [Display(Name = "Art Description")]
        [DataType(DataType.MultilineText)]
        public string artDescription { get; set; }

        [Display(Name = "Created by other artist?")]
        public bool otherArtist { get; set; }

        [Display(Name = "Artist Name")]
        public string otherArtistName { get; set; }
        [Display(Name = "Artist Address")]
        public string otherArtistAddress { get; set; }

        public string fileName { get; set; }
        public string Path { get; set; }

        public DateTime? datePosted { get; set; }
        public DateTime? dateUpdated { get; set; }
        public bool  ForPosting { get; set; }

        public bool Sold { get; set; }

        public int? BuyerId { get; set; }
        [ForeignKey("BuyerId")]
        public virtual UserAccount BuyerAccount { get; set; }
        
        public int SellerId { get; set; }
        [ForeignKey("SellerId")]
        public virtual UserAccount SellerAccount { get; set; }

       // [Display(Name = "Art Category")]
        public int StyleID { get; set; }
        public virtual Style Style { get; set; }

        public virtual ICollection<WishList> WishLists { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }

        

    }
}