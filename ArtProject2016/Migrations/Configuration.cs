using System.Web.Helpers;
using System.Web.Security;
using ArtProject2016.Models;
using WebMatrix.WebData;

namespace ArtProject2016.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<ArtContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }




        //public class MyInit : DropCreateDatabaseIfModelChanges<ArtContext>
        //{
        protected override void Seed(ArtContext context)
        {

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            try
            {
                

            //    context.SaveChanges();

                if (!WebSecurity.Initialized)
                {
                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserAccounts", "Id", "userName", true);

                    if (!Roles.RoleExists("Admin"))
                    {
                        Roles.CreateRole("Admin");
                    }
                    if (!Roles.RoleExists("Artist"))
                    {
                        Roles.CreateRole("Artist");
                    }
                    if (!Roles.RoleExists("Collector"))
                    {
                        Roles.CreateRole("Collector");
                    }
                }

                context.Categories.AddOrUpdate(
                    c => c.Id, new Category()
                    {
                        name = "Painting",
                        dateAdded = DateTime.Now
                    },
                                new Category()
                                {
                                    name = "Drawing",
                                    dateAdded = DateTime.Now
                                });


                WebSecurity.CreateUserAndAccount("qwer", "321", new
                {
                    firstName = "Jeryl",
                    lastName = "Suarez",
                    userType = "Artist",
                    nickName = "Jeryl Pogi"
                });

                WebSecurity.CreateUserAndAccount("qwert", "321", new
                {
                    firstName = "Colec",
                    lastName = "tor",
                    userType = "Collector",
                    nickName = "Coleklek"
                });


                var artist = new UserProfile()
                {
                    profileDesc = "A realist/impressionist artist who loves landscape, everyday scene, still-life florals and butterflies for his painting subjects. He took his fine arts studies in Manila, Philippines. And transform his paintings into lively, lovely atmosphere.",
                    UserAccount = context.UserAccounts.First(a => a.userName == "qwer"),
                    isIdVerified = true
                };
                var collector = new UserProfile()
                {
                    UserAccount = context.UserAccounts.First(a => a.userName == "qwert"),
                    isIdVerified = true
                };

                context.UserProfiles.Add(artist);
                context.UserProfiles.Add(collector);

                Roles.AddUserToRole("qwer", "Artist");

                Roles.AddUserToRole("qwert", "Collector");

                context.VoucherCodes.AddOrUpdate(
                    v => v.Id, new VoucherCode()
                                   {
                                       VoucherName = "First Order Voucher",
                                       VoucherDeduction = 500,
                                       VoucherCount = 10,
                                       VoucherMinOrder = 5000,
                                       VoucherEnabled = true
                                   }
                    
                    );

                ////    var role = System.Web.Security.Roles.Provider;
                //    if (!Roles.RoleExists("Admin"))
                //    {
                //        Roles.CreateRole("Admin");
                //    }
                //    else if(!Roles.RoleExists("Artist"))
                //    {
                //        Roles.CreateRole("Admin");
                //    }
                //    else if(!Roles.RoleExists("Collector"))
                //    {
                //        Roles.CreateRole("Collector");
                //    }


            }
            catch (Exception ex)
            {
                // ex.Message;
            }



        }

    }
}
