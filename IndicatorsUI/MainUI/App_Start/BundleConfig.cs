using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using Profiles.DataAccess;

namespace Profiles.MainUI
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            var appConfig = AppConfig.Instance;
            string staticPath = "~/" + appConfig.JavaScriptVersionFolder;
            var jsPath = staticPath + "js/";
            var cssPath = staticPath + "css/";

            // Add scripts
            AddScripts(bundles, jsPath, "js-fingertips", GetJsFingertips());
            AddScripts(bundles, jsPath, "js-longer-lives", GetJsLongerLives());
            AddScripts(bundles, jsPath, "js-practice-profiles", GetJsPracticeProfiles());
            AddScripts(bundles, jsPath, "js-fingertips-maps", GetJsFingertipsMaps());

            // Add styles
            AddStyles(bundles, cssPath, "css-fingertips", GetCssFingertips());
            AddStyles(bundles, cssPath, "css-longer-lives", GetCssLongerLives());
        }

        private static string[] GetJsPracticeProfiles()
        {
            var jsFiles = new[]
            {
                "chosen.jquery.min.js",
                "PageMetadata.js", "PagePopulation.js", "PagePracticeSpineChart.js",
                "PageScatter.js", "PagePracticeBarChart.js", "PagePracticeTrends.js",
                "PagePracticeSearch.js", "PageCluster.js", "SitePracticeProfiles.js"
            };
            return jsFiles;
        }

        private static string[] GetJsLongerLives()
        {
            var jsFiles = new[]
            {
                "jquery-1.10.2.min.js",
                "mortality-jquery-ui-1.10.3.custom.min.js",
                "modernizr-2.6.2.min.js",
                "longer-lives-min.js",
                "underscore-min.js",
                "hogan.min.js",
                "common.js",
                "fingertips.js",
                "LongerLives/SiteBaseLongerLives.js"
            };
            return jsFiles;
        }

        private static string[] GetCssLongerLives()
        {
            var cssFiles = new[]
            {
                "longer-lives-normalize.css",
                "longer-lives-main.css",
                "longer-lives-map.css",
                "jquery-ui-1.10.1.css"
            };
            return cssFiles;
        }

        private static string[] GetCssFingertips()
        {
            var cssFiles = new[]
            {
                "common.css",
                "fingertips.css",
                "jquery-ui-1.10.1.css",
                "core-allpages.css",
                "chosen/chosen.min.css"
            };
            return cssFiles;
        }

        private static string[] GetJsFingertips()
        {
            var jsFiles = new[]
            {
                "jquery-1.10.2.min.js",
                "chosen.jquery.min.js",
                "mortality-jquery-ui-1.10.3.custom.min.js",
                "Highcharts/highcharts.js",
                "Highcharts/highcharts-more.js",
                "Highcharts/exporting.js",
                "underscore-min.js",
                "hogan.min.js",
                "common.js",
                "tooltip.js",
                "spineChart.js",
                "Floatahead/jquery.floatThead-slim.min.js",
                "js.cookie.js"
            };
            return jsFiles;
        }

        private static string[] GetJsFingertipsMaps()
        {
            // new version of esri-leaflet.js can be downloaded from http://esri.github.io/esri-leaflet/download
            var jsFiles = new[]
            {
                "leaflet.js",
                "tablesorter.min.js",
                "esri-leaflet.js",
                "PageMap.js"
            };
            return jsFiles;
        }

        private static string[] PrependUrlPath(string[] files, string preceedingPath)
        {
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = preceedingPath + files[i];
            }

            return files;
        }

        public static void AddStyles(BundleCollection bundles, string path, string bundleName,
            string[] files)
        {
            bundles.Add(new StyleBundle("~/bundles/" + bundleName).Include(
                PrependUrlPath(files, path)));
        }

        public static void AddScripts(BundleCollection bundles, string path, string bundleName,
            string[] files)
        {
            bundles.Add(new ScriptBundle("~/bundles/" + bundleName).Include(
                PrependUrlPath(files, path)));
        }
    }
}