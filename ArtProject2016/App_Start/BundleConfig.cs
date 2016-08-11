using System.Web;
using System.Web.Optimization;

namespace ArtProject2016
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //   "~/Scripts/jquery-{version}.js"
            bundles.Add(new ScriptBundle("~/Scripts/js/Top").Include("~/Scripts/js/jquery-1.11.0.min.js")
                .Include("~/Scripts/js/respond.min.js").Include("~/Scripts/js/bootstrap.min.js"));


            bundles.Add(new ScriptBundle("~/Scripts/js/Bot").Include("~/Scripts/js/jquery.cookie.js")
               .Include("~/Scripts/js/waypoints.min.js").Include("~/Scripts/js/modernizr.js")
               .Include("~/Scripts/js/bootstrap-hover-dropdown.js")
               .Include("~/Scripts/js/owl.carousel.min.js").Include("~/Scripts/js/front.js")
               .Include("~/Scripts/jquery.validate.min.js").Include("~/Scripts/jquery.validate.unobtrusive.min.js"));
            

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));



            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/Css/allCss").Include(
                      "~/Content/css/font-awesome.css").Include(
                      "~/Content/css/bootstrap.min.css").Include(
                      "~/Content/css/animate.min.css").Include(
                       "~/Content/css/owl.carousel.css").Include(
                       "~/Content/css/owl.theme.css").Include(
                        "~/Content/css/style.default.css").Include(
                          "~/Content/css/custom.css"));


            //debug to false 
            //BundleTable.EnableOptimizations = true;

        }
    }
}
