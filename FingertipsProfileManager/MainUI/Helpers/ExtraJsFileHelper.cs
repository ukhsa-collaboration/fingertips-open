using System.Collections.Generic;
using System.Text;

namespace Fpm.MainUI.Helpers
{
    /// <summary>
    /// Parses and generates the extra JS file string that determines which
    /// tabs are displayed in a Fingertips profile
    /// </summary>
    public class ExtraJsFileHelper : IFingertipsTabs
    {
        public bool IsMapTab { get; set; }
        public bool IsScatterPlotTab { get; set; }
        public bool IsEnglandTab { get; set; }
        public bool IsPopulationTab { get; set; }
        public bool IsReportsTab { get; set; }
        public bool IsBoxPlotTab { get; set; }
        public bool IsInequalitiesTab { get; set; }
        public CompareAreasOption CompareAreasOption { get; set; }

        public const string MapFileName = "PageMap.js";
        public const string ScatterPlotFileName = "PageScatterPlot.js";
        public const string EnglandFileName = "PageEngland.js";
        public const string PopulationFileName = "PagePopulationNew.js";
        public const string ReportsFileName = "PageReports.js";
        public const string BoxPlotFileName = "PageBoxPlots.js";
        public const string InequalitiesFileName = "PageInequalities.js";
        public const string BarChartAndFunnelPlotFileName = "PageBarChartAndFunnelPlot.js";

        /// <summary>
        /// Sets default values
        /// </summary>
        public ExtraJsFileHelper()
        {
            IsMapTab = true;
            CompareAreasOption = CompareAreasOption.BarChartAndFunnelPlot;
        }

        /// <summary>
        /// Parses and sets own state from existing JS files string
        /// </summary>
        public ExtraJsFileHelper(string extraJsFiles)
        {
            IsMapTab = extraJsFiles.Contains(MapFileName);
            IsScatterPlotTab = extraJsFiles.Contains(ScatterPlotFileName);
            IsEnglandTab = extraJsFiles.Contains(EnglandFileName);
            IsPopulationTab = extraJsFiles.Contains(PopulationFileName);
            IsReportsTab = extraJsFiles.Contains(ReportsFileName);
            IsBoxPlotTab = extraJsFiles.Contains(BoxPlotFileName);
            IsInequalitiesTab = extraJsFiles.Contains(InequalitiesFileName);

            CompareAreasOption = extraJsFiles.Contains(BarChartAndFunnelPlotFileName)
                ? CompareAreasOption.BarChartAndFunnelPlot
                : CompareAreasOption.BarChartOnly;
        }

        /// <summary>
        /// Get comma separated list of optional and mandatory JS file names
        /// </summary>
        public string GetExtraJsFiles()
        {
            var sb = new List<string>();

            sb.Add("PageTartanRug.js");

            if (IsScatterPlotTab) sb.Add(ScatterPlotFileName);
            if (IsMapTab) sb.Add(MapFileName);

            sb.Add("PageAreaTrends.js");

            sb.Add(CompareAreasOption == CompareAreasOption.BarChartAndFunnelPlot
                ? BarChartAndFunnelPlotFileName
                : "PageBarChart.js");

            sb.Add("PageAreaProfile.js");

            if (IsInequalitiesTab) sb.Add(InequalitiesFileName);
            if (IsEnglandTab) sb.Add(EnglandFileName);
            if (IsPopulationTab) sb.Add(PopulationFileName);
            if (IsBoxPlotTab) sb.Add(BoxPlotFileName);
            if (IsReportsTab) sb.Add(ReportsFileName);

            sb.Add("PageMetadata.js");
            sb.Add("PageDownload.js");

            return string.Join(",", sb);
        }
    }
}