using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class UserAccount
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //public Guid Id
        //{
        //    get { return new Guid(); }
        //    set { _Id = value; }

        //};
        [Required]
        public string userName { get; set; }
        //[Required]
        //[DataType(DataType.Password)]
        //public string password { get; set; }
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        [Required]
        public string nickName { get; set; }
        [Required]
        public string userType { get; set; }


        public virtual ICollection<UserProfile> UserProfiles { get; set; }

        [InverseProperty("BuyerAccount")]
        public virtual ICollection<ForSale> ForSaleBuyer { get; set; }
        [InverseProperty("SellerAccount")]
        public virtual ICollection<ForSale> ForSaleSeller  { get; set; }

        public virtual ICollection<WishList> WishLists { get; set; }
    }
}