using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Management;
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
            //using(var context = new ArtContext())
            //{
            //    if (!Database.Exists("Sql2016"))
            //    {
            //        Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ArtContext>());
            //        context.Database.Initialize(false);
            //    }
            //}

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

        private void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            var httpException = ex as HttpException ?? ex.InnerException as HttpException;
            if (httpException == null) return;

            if (httpException.WebEventCode == WebEventCodes.RuntimeErrorPostTooLarge)
            {
              //  TempData["success"] = "Success. ID no: " + newOrder.Id.ToString();
                
                //handle the error
                //Response.Write("Too big a file, dude"); //for example
            }
        }
    }
}
