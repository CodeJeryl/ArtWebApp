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

                //categories == style
                context.Styles.AddOrUpdate(
                    c => c.name, new Style()
                    {
                        name = "Abstract",
                        dateAdded = DateTime.Now
                    },
                    new Style()
                    {
                        name = "Conceptual",
                        dateAdded = DateTime.Now
                    },
                    new Style()
                    {
                        name = "Fauvism",
                        dateAdded = DateTime.Now
                    },
                    new Style()
                    {
                        name = "Futurism",
                        dateAdded = DateTime.Now
                    },
                    new Style()
                    {
                        name = "Hyperrealism",
                        dateAdded = DateTime.Now
                    },
                    new Style()
                    {
                        name = "Impressionism",
                        dateAdded = DateTime.Now
                    },
                    new Style()
                    {
                        name = "Minimalism",
                        dateAdded = DateTime.Now
                    },
                    new Style()
                    {
                        name = "Pop",
                        dateAdded = DateTime.Now
                    },
                    new Style()
                    {
                        name = "Photorealism",
                        dateAdded = DateTime.Now
                    },
                    new Style()
                    {
                        name = "Surrealism",
                        dateAdded = DateTime.Now
                    });


                //WebSecurity.CreateUserAndAccount("jcsx99@yahoo.com", "321", new
                //{
                //    firstName = "Jeryl",
                //    lastName = "Suarez",
                //    userType = "Artist",
                //    nickName = "Jeryl Pogi"
                //});

                //WebSecurity.CreateUserAndAccount("jcs990@yahoo.com", "321", new
                //{
                //    firstName = "Colec",
                //    lastName = "tor",
                //    userType = "Collector",
                //    nickName = "Coleklek"
                //});

                WebSecurity.CreateUserAndAccount("jerylsuarez@gmail.com", "321", new
                                                                                     {
                                                                                         firstName = "Admin",
                                                                                         lastName = "Account",
                                                                                         userType = "Admin",
                                                                                         nickName = "AdminNickname"
                                                                                     });
                var Admin = new UserProfile()
                                {
                                    UserAccount = context.UserAccounts.First(a => a.userName == "jerylsuarez@gmail.com")
                                };
                context.UserProfiles.Add(Admin);
                Roles.AddUserToRole("jerylsuarez@gmail.com", "Admin");

                //var artist = new UserProfile()
                //{
                //    profileDesc = "A realist/impressionist artist who loves landscape, everyday scene, still-life florals and butterflies for his painting subjects. He took his fine arts studies in Manila, Philippines. And transform his paintings into lively, lovely atmosphere.",
                //    UserAccount = context.UserAccounts.First(a => a.userName == "jcsx99@yahoo.com")

                //};
                //var collector = new UserProfile()
                //{
                //    UserAccount = context.UserAccounts.First(a => a.userName == "jcs990@yahoo.com")
                //};

                //context.UserProfiles.Add(artist);
                //context.UserProfiles.Add(collector);

                //Roles.AddUserToRole("jcsx99@yahoo.com", "Artist");

                //Roles.AddUserToRole("jcs990@yahoo.com", "Collector");

                context.VoucherCodes.AddOrUpdate(
                    v => v.Id, new VoucherCode()
                                   {
                                       VoucherName = "Welcome",
                                       VoucherDeduction = 500,
                                       VoucherCount = 99,
                                       VoucherMinOrder = 5000,
                                       VoucherEnabled = true
                                   }

                    );

                context.ShippingCompanies.AddOrUpdate(
                    ship => ship.Id, new ShippingCompany()
                                           {
                                               Name = "LBC"
                                           },
                                           new ShippingCompany()
                                            {
                                                Name = "Air21"
                                            },
                                         new ShippingCompany()
                                           {
                                               Name = "Xend"
                                           });

                context.SaveChanges();

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
