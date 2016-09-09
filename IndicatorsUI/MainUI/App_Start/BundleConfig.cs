using Profiles.DataAccess;
using System.Collections.Generic;
using System.Web.Optimization;

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
            AddScripts(bundles, jsPath, "js-fingertips-js-file", GetJsFingertipsJsFile());
            AddScripts(bundles, jsPath, "js-longer-lives", GetJsLongerLives());
            AddScripts(bundles, jsPath, "js-practice-profiles", GetJsPracticeProfiles());
            AddScripts(bundles, jsPath, "js-fingertips-maps", GetJsFingertipsMaps());
            AddScripts(bundles, jsPath, "js-area-search", GetJsAreaSearch());

            // Add styles
            AddStyles(bundles, cssPath, "css-fingertips", GetCssFingertips());
            AddStyles(bundles, cssPath, "css-longer-lives", GetCssLongerLives());
            AddStyles(bundles, cssPath, "css-area-search", new[] { "AreaSearch/area-search.css" });
        }

        private static string[] GetJsPracticeProfiles()
        {
            var jsFiles = new[]
            {
                "vendor/chosen/chosen.jquery.js",
                "PageMetadata.js", "PagePopulation.js", "PagePracticeSpineChart.js",
                "PageScatter.js", "PagePracticeBarChart.js", "PagePracticeTrends.js",
                "PagePracticeSearch.js", "PageCluster.js", "SitePracticeProfiles.js"
            };
            return jsFiles;
        }

        private static string[] GetJsAreaSearch()
        {
            var jsFiles = new List<string>
            {
                "PageDownload.js",
                "AreaSearch/area-search.js"
            };
            AddEnvironmentFiles(jsFiles);
            return jsFiles.ToArray();
        }

        private static void AddEnvironmentFiles(List<string> jsFiles)
        {
            if (AppConfig.Instance.IsEnvironmentTest)
            {
                jsFiles.Add("EnvironmentTest.js");
            }
        }

        private static string[] GetJsLongerLives()
        {
            var jsFiles = new[]
            {
                "vendor/jquery-legacy/jquery.min.js",
                "mortality-jquery-ui-1.10.3.custom.min.js",
                "vendor/modernizr/modernizr.js",
                "longer-lives-min.js",
                "vendor/underscore/underscore-min.js",
                "vendor/hogan.js/hogan.min.js",
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
                "vendor/jquery-legacy/jquery.min.js",
                "vendor/chosen/chosen.jquery.js",
                "mortality-jquery-ui-1.10.3.custom.min.js",
                "vendor/highcharts/highcharts.js",
                "vendor/highcharts/highcharts-more.js",
                "vendor/highcharts/exporting.js",
                "vendor/underscore/underscore-min.js",
                "vendor/hogan.js/hogan.min.js",
                "common.js",
                "tooltip.js",
                "TooltipRecentTrends.js",
                "spineChart.js",
                "vendor/jquery.floatThead/jquery.floatThead.min.js",
                "vendor/js-cookie/js.cookie.js"
            };
            return jsFiles;
        }


        private static string[] GetJsFingertipsJsFile()
        {
            var jsFiles = new[]
            {
                "fingertips.js"
            };
            return jsFiles;
        }

        private static string[] GetJsFingertipsMaps()
        {
            // new version of esri-leaflet.js can be downloaded from http://esri.github.io/esri-leaflet/download
            var jsFiles = new[]
            {
                "vendor/leaflet/leaflet.js",
                "vendor/esri-leaflet-v1.0.0-rc.2/esri-leaflet.js",
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