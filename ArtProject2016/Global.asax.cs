using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using ArtProject2016.Migrations;
using ArtProject2016.Models;
using WebMatrix.WebData;

namespace ArtProject2016
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            using(var context = new ArtContext())
            {
                if(!Database.Exists("DefaultConnection"))
                {
                    Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ArtContext>());
                    context.Database.Initialize(false);
                }
            }

            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserAccounts", "Id", "userName", true);
            }





            //if (!WebSecurity.Initialized)
            //{
            //    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserAccounts", "Id", "userName", true);

            //    if (!Roles.RoleExists("Admin"))
            //    {
            //        Roles.CreateRole("Admin");
            //    }
            //    if (!Roles.RoleExists("Artist"))
            //    {
            //        Roles.CreateRole("Artist");
            //    }
            //    if (!Roles.RoleExists("Collector"))
            //    {
            //        Roles.CreateRole("Collector");
            //    }
            //}

          
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
