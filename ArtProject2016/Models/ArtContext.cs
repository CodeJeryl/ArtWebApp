using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using ArtProject2016.Migrations;

namespace ArtProject2016.Models
{
    public class ArtContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        //    modelBuilder.Entity<ForSale>()
        //                .HasOptional(m => m.BuyerAccount)
        //                .WithMany(t => t.ForSaleBuyer)
        //                .HasForeignKey(m => m.BuyerId)
        //                .WillCascadeOnDelete(false);

        //    modelBuilder.Entity<ForSale>()
        //                .HasRequired(m => m.SellerAccount)
        //                .WithMany(t => t.ForSaleSeller)
        //                .HasForeignKey(m => m.SellerId)
        //                .WillCascadeOnDelete(false);
        }

        public ArtContext(): base("DefaultConnection")
        {
            
        }

        public ArtContext(string connstring) : base(connstring)
        {
            
        }

        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }
      
        public DbSet<ForSale> ForSales { get; set; }
        public DbSet<WishList> WishLists { get; set; }

        public DbSet<VoucherCode> VoucherCodes { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderTracking> OrderTrackings { get; set; }
        public DbSet<ShippingCompany> ShippingCompanies { get; set; }

        public DbSet<PaypalPayment> PaypalPayments { get; set; }

      // For multiple picture table
      //  public DbSet<ForSaleAlbum> ForSaleAlbums { get; set; }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Class: {0}, Property: {1}, Error: {2}",
                            validationErrors.Entry.Entity.GetType().FullName,
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }

                throw;  // You can also choose to handle the exception here...
            }
        }
      
        /*         
            context.ArtistProfiles.AddOrUpdate(
                artist => artist.ID,
                new ArtistProfile() { artistName = "Jo", artistLastName = "Suarez", birthDate = DateTime.Now,net = 50000},
                new ArtistProfile() { artistName = "Van", artistLastName = "Gogh", birthDate = DateTime.Now,net = 12000},
                new ArtistProfile() { artistName = "Irish", artistLastName = "Malang", birthDate = DateTime.Now,net = 21000}
                );

            context.SaveChanges();

            context.Courses.AddOrUpdate(
                c => c.ID,
                new Course() {Title = "Fine Arts", Credits = 3, ArtistProfile = context.ArtistProfiles.FirstOrDefault(a => a.artistName == "Jo")},
                  new Course() { Title = "IT", Credits = 3, ArtistProfile = context.ArtistProfiles.FirstOrDefault(a => a.artistName == "Irish") }
               );
         */
    }
}