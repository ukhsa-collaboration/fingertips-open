using System.Collections.Generic;
using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Entities.User;

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
        public bool IsNational { get; set; }
        public bool HasOwnFrontPage { get; set; }
        public bool AreIndicatorsExcludedFromSearch { get; set; }
        public string AccessControlGroup { get; set; }

        public string SortBy { get; set; }

        public List<int> GroupIds { get; set; }

        public int SpineChartMinMaxLabelId { get; set; }

        public IEnumerable<AreaType>PdfAreaTypes { get; set; }

        public void SetDefaultValues()
        {
            DefaultAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority;
            ExtraJsFiles =
                "PageTartanRug.js,+map,PageAreaTrends.js,PageBarChartAndFunnelPlot.js,PageAreajs,PageMetadata.js,PageDownload.js";
            ExtraCssFiles = "+map";
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
