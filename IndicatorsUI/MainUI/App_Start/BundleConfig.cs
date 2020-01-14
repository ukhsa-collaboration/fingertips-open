using IndicatorsUI.DataAccess;
using System.Collections.Generic;
using System.Web.Optimization;

namespace IndicatorsUI.MainUI
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif

            var appConfig = AppConfig.Instance;
            var staticPath = "~/" + appConfig.JavaScriptVersionFolder;
            var jsPath = staticPath + "js/";
            var cssPath = staticPath + "css/";
            var angularPath = staticPath + "angular-app-dist/";

            // Add scripts
            AddScripts(bundles, jsPath, "js-fingertips", GetJsFingertips());
            AddScripts(bundles, jsPath, "js-fingertips-js-file", GetJsFingertipsJsFile());
            AddScripts(bundles, jsPath, "js-longer-lives", GetJsLongerLives());
            AddScripts(bundles, jsPath, "js-fingertips-maps", GetJsFingertipsMaps());
            AddScripts(bundles, jsPath, "js-area-search", GetJsAreaSearch());
            AddScripts(bundles, jsPath, "js-html-2-canvas", GetJsHtml2Canvas());
            AddScripts(bundles, jsPath, "js-site-key-indicators", "SiteKeyIndicators.js");
            AddScripts(bundles, jsPath, "js-twitter", "PageTwitterFeed.js");

            // Scripts: Indicator lists
            AddScripts(bundles, jsPath, "js-indicator-list-edit", "Indicatorlist/indicator-list-edit.js");
            AddScripts(bundles, jsPath, "js-indicator-list-index", "Indicatorlist/indicator-list-index.js");
            AddScripts(bundles, jsPath, "js-indicator-list-view", "Indicatorlist/indicator-list-view.js");

            // Add single files
            AddSingleScriptFiles(bundles, jsPath);
            AddSingleStyleFiles(bundles, cssPath);

            // Add styles
            AddStyles(bundles, cssPath, "css-fingertips", GetCssFingertips());
            AddStyles(bundles, cssPath, "css-longer-lives", GetCssLongerLives());
            AddStyles(bundles, cssPath, "css-area-search", "AreaSearch/area-search.css");
            AddStyles(bundles, cssPath, "css-core-content-pages", "core-contentpages.css");
            AddStyles(bundles, cssPath, "css-core-data-page", "core-datapage.css");
            AddStyles(bundles, cssPath, "css-phof", "phof.css");
            AddStyles(bundles, cssPath, "css-login", "Login/login.css");

            // Styles: Indicator lists
            AddStyles(bundles, cssPath, "css-indicator-list-edit", "IndicatorList/indicator-list-edit.css");
            AddStyles(bundles, cssPath, "css-indicator-list-index", "IndicatorList/indicator-list-index.css");

            // Styles: Area lists
            AddStyles(bundles, cssPath, "css-area-list-edit", "AreaList/area-list-edit.css");
            AddStyles(bundles, cssPath, "css-area-list-index", "AreaList/area-list-index.css");

            // Add Angular files
            AddScripts(bundles, angularPath, "js-angular", GetAngularFiles());
            AddStyles(bundles, angularPath, "css-angular", "styles.css");
        }

        private static void AddSingleScriptFiles(BundleCollection bundles, string path)
        {
            string[] files = new[]
            {
                "PageTartanRug.js",
                "PageScatterPlot.js",
                "PageMap.js",
                "PageAreaTrends.js",
                "PageBarChart.js",
                "PageAreaProfile.js",
                "PageInequalities.js",
                "PageEngland.js",
                "PagePopulationNew.js",
                "PageBoxPlots.js",
                "PageMetadata.js",
                "PageDownload.js",
                "PageReports.js",
                "SiteSearch.js"
            };

            foreach (var file in files)
            {
                AddScripts(bundles, path, file, file);
            }
        }

        private static void AddSingleStyleFiles(BundleCollection bundles, string path)
        {
            string[] files = new[]
            {
                "PageMap.css"
            };

            foreach (var file in files)
            {
                AddStyles(bundles, path, file, file);
            }
        }

        private static string[] GetAngularFiles()
        {
            var jsFiles = new[]
            {
                "main.js",
                "polyfills.js",
                "runtime.js",
                "styles.js",
                "vendor.js"
            };
            return jsFiles;
        }

        private static string[] GetJsHtml2Canvas()
        {
            var jsFiles = new[]
            {
                "vendor/html2canvas/html2canvas.min.js",
                "vendor/html2canvas/html2canvas.svg.min.js"
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

            return jsFiles.ToArray();
        }

        private static string[] GetJsLongerLives()
        {
            var jsFiles = new[]
            {
                "vendor/jquery/jquery.min.js",
                "mortality-jquery-ui-1.10.3.custom.min.js",
                "vendor/modernizr/modernizr.js",
                "vendor/underscore/underscore-min.js",
                "vendor/hogan.js/hogan.min.js",
                "pholio-constants.js",
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
                "vendor/font-awesome/font-awesome.min.css",
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
                "bootstrap/bootstrap.min.css",
                "common.css",
                "jquery-ui-1.10.1.css",
                "core-allpages.css",
                "chosen/chosen.min.css",
                "font-awesome.min.css"
            };
            return cssFiles;
        }

        private static string[] GetJsFingertips()
        {
            var jsFiles = new[]
            {
                "vendor/jquery/jquery.min.js",
                "vendor/tether/tether.min.js",
                "vendor/chosen/chosen.jquery.js",
                "mortality-jquery-ui-1.10.3.custom.min.js",
                "vendor/popper.js/popper.min.js", // Required by Bootstrap
                "vendor/bootstrap/bootstrap.min.js",
                "vendor/underscore/underscore-min.js",
                "vendor/hogan.js/hogan.min.js",
                "pholio-constants.js",
                "common.js",
                "tooltip.js",
                "TooltipRecentTrends.js",
                "vendor/jquery.floatThead/jquery.floatThead.min.js",
                "vendor/js-cookie/js.cookie.js"
            };
            return jsFiles;
        }

        private static string[] GetJsFingertipsJsFile()
        {
            var jsFiles = new[]
            {
                "fingertips.js",
                "fingertipsGlobal.js"
            };
            return jsFiles;
        }

        private static string[] GetJsFingertipsMaps()
        {
            return new[] { "PageMap.js" };
        }

        private static string[] PrependUrlPath(string[] files, string preceedingPath)
        {
            for (var i = 0; i < files.Length; i++)
                files[i] = preceedingPath + files[i];

            return files;
        }

        public static void AddStyles(BundleCollection bundles, string path, string bundleName,
            params string[] files)
        {
            bundles.Add(new StyleBundle("~/bundles/" + bundleName).Include(
                PrependUrlPath(files, path)));
        }

        public static void AddScripts(BundleCollection bundles, string path, string bundleName,
            params string[] files)
        {
            bundles.Add(new Bundle("~/bundles/" + bundleName).Include(
                PrependUrlPath(files, path)));
        }
    }
}