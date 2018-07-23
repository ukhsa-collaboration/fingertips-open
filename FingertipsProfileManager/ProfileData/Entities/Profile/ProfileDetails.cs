using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Entities.User;
using System.Collections.Generic;

namespace Fpm.ProfileData.Entities.Profile
{
    public class ProfileDetails
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string UrlKey { get; set; }

        public string DefaultDomainName { get; set; }

        public string DomainDescription { get; set; }

        public IEnumerable<UserGroupPermissions> UserPermissions { get; set; }

        public int DefaultAreaTypeId { get; set; }
        public int ContactUserId { get; set; }
        public string ExtraJsFiles { get; set; }
        public string ExtraCssFiles { get; set; }
        public int EnumParentDisplay { get; set; }
        public string AreasIgnoredForSpineChart { get; set; }
        public int KeyColourId { get; set; }
        public int DefaultFingertipsTabId { get; set; }
        public bool ArePdfs { get; set; }
        public bool StartZeroYAxis { get; set; }
        public bool IsLive { get; set; }
        public string ReturnUrl { get; set; }
        public string AreasIgnoredEverywhere { get; set; }
        public int? SkinId { get; set; }
        public bool IsProfileViewable { get; set; }
        public bool ShowDataQuality { get; set; }
        public bool ShouldBuildExcel { get; set; }
        public bool HasTrendMarkers { get; set; }
        public bool UseTargetBenchmarkByDefault { get; set; }
        public string FrontPageAreaSearchAreaTypes { get; set; }
        public bool HasAnyData { get; set; }
        public bool HasStaticReports { get; set; }
        public bool IsNational { get; set; }
        public bool HasOwnFrontPage { get; set; }
        public bool AreIndicatorsExcludedFromSearch { get; set; }
        public string AccessControlGroup { get; set; }
        public string StaticReportsFolders { get; set; }
        public string StaticReportsLabel { get; set; }
        public string LeadProfileForCollectionIds { get; set; }
        public bool IsChangeFromPreviousPeriodShown { get; set; }

        public string SortBy { get; set; }

        public List<int> GroupIds { get; set; }

        public int SpineChartMinMaxLabelId { get; set; }

        public IEnumerable<AreaType> PdfAreaTypes { get; set; }

        public int NewDataTimeSpanInDays { get; set; }

        public void SetDefaultValues(string extraJsFiles)
        {
            DefaultAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority;
            ExtraJsFiles = extraJsFiles;
            ExtraCssFiles = "PageMap.css";
            EnumParentDisplay = 0;
            AreasIgnoredForSpineChart =
                AreaCodes.CountyUa_IslesOfScilly + "," + AreaCodes.CountyUa_CityOfLondon;
            KeyColourId = 0;
            DefaultFingertipsTabId = 0;
            SkinId = SkinIds.Core;
            ArePdfs = false;
            StartZeroYAxis = false;
            AreasIgnoredEverywhere = null;
            AreasIgnoredEverywhere = AreaCodes.CountyUa_Bedfordshire;
            IsNational = true;
        }
    }
}
