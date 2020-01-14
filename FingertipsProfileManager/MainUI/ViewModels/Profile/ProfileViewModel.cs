using Fpm.MainUI.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Fpm.MainUI.ViewModels.Profile
{
    public class ProfileViewModel : IFingertipsTabs
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Profile Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Profile Key")]
        [RegularExpression(@"^[a-zA-Z0-9_-]*$", ErrorMessage = "Only alpha/numeric characters (without spaces) allowed.")]
        public string UrlKey { get; set; }

        [Required]
        [Display(Name = "Domain Name")]
        public string DefaultDomainName { get; set; }

        [Required]
        [Display(Name = "Domain Description")]

        public int DefaultAreaTypeId { get; set; }
        public List<string> ContactUserIds { get; set; }
        public string ExtraCssFiles { get; set; }
        public string AreasIgnoredForSpineChart { get; set; }
        public int DefaultFingertipsTabId { get; set; }

        public bool StartZeroYAxis { get; set; }
        public bool IsLive { get; set; }
        public string ReturnUrl { get; set; }
        public string AreasIgnoredEverywhere { get; set; }
        public int? SkinId { get; set; }
        public bool IsProfileViewable { get; set; }
        public bool ShowDataQuality { get; set; }
        public bool ShowOfficialStatistic { get; set; }
        public bool ShouldBuildExcel { get; set; }
        public bool HasTrendMarkers { get; set; }
        public bool UseTargetBenchmarkByDefault { get; set; }
        public string FrontPageAreaSearchAreaTypes { get; set; }
        public bool IsNational { get; set; }
        public bool HasOwnFrontPage { get; set; }
        public bool HasAnyData { get; set; }
        public bool HasStaticReports { get; set; }
        public bool AreIndicatorsExcludedFromSearch { get; set; }
        public string AccessControlGroup { get; set; }
        public string StaticReportsLabel { get; set; }
        public string StaticReportsFolders { get; set; }
        public string LeadProfileForCollectionIds { get; set; }
        public bool IsChangeFromPreviousPeriodShown { get; set; }

        public int SpineChartMinMaxLabelId { get; set; }

        public IEnumerable<ProfileAreaType> SelectedPdfAreaTypes { get; set; }

        public IEnumerable<ProfileUser> ProfileUsers { get; set; }
        public SelectList AllUsers { get; set; }

        public int NewDataDeploymentCount { get; set; }
        public bool AreIndicatorNamesDisplayedWithNumbers { get; set; }

        // Tab options
        public bool IsMapTab { get; set; }
        public bool IsScatterPlotTab { get; set; }
        public bool IsEnglandTab { get; set; }
        public bool IsPopulationTab { get; set; }
        public bool IsReportsTab { get; set; }
        public bool IsBoxPlotTab { get; set; }
        public bool IsInequalitiesTab { get; set; }
        public bool IsCompareAreasTab { get; set; }
    }
}