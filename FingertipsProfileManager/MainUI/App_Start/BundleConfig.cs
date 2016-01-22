using System.Web.Optimization;

namespace Fpm.MainUI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new StyleBundle("~/css/bootstrap")
               .Include("~/" + AppConfig.CssPath + "bootstrap.css")
               .Include("~/" + AppConfig.CssPath + "bootstrap-theme.css")
               );

            bundles.Add(new StyleBundle("~/css/site")
               .Include("~/" + AppConfig.CssPath + "site.css")
               );

            bundles.Add(new ScriptBundle("~/js/jquery")
                .Include("~/" + AppConfig.JsPath + "jquery-1.8.3.js")
                .Include("~/" + AppConfig.JsPath + "jquery-ui-1.9.2.js")
                .Include("~/" + AppConfig.JsPath + "jquery.validate.min.js")
                .Include("~/" + AppConfig.JsPath + "jquery.validate.unobtrusive.min.js")
                );
            
            bundles.Add(new ScriptBundle("~/js/tablesorter")
                .Include("~/" + AppConfig.JsPath + "jquery.tablesorter.js")
                .Include("~/" + AppConfig.JsPath + "jquery.tablesorter.widgets.js")
                );
            
            bundles.Add(new ScriptBundle("~/js/underscore")
               .Include("~/" + AppConfig.JsPath + "underscore-min.js"));

            bundles.Add(new ScriptBundle("~/js/modernizr")
              .Include("~/" + AppConfig.JsPath + "modernizr-2.6.2.min.js"));
            
            bundles.Add(new ScriptBundle("~/js/bootstrap")
               .Include("~/" + AppConfig.JsPath + "bootstrap.js"));


            bundles.Add(new ScriptBundle("~/js/common")
               .Include("~/" + AppConfig.JsPath + "included-on-all-pages.js"));

            bundles.Add(new ScriptBundle("~/bundles/profile-management")
                .Include("~/"+ AppConfig.JsPath + "profile-management.js")
                );
        }
    }
}