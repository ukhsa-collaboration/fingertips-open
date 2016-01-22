using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fpm.ProfileData;

namespace Fpm.MainUI.ViewModels
{
    public class ProfileViewModel
    {
        public int Id { get; set; }

        [RequiredAttribute]
        [Display(Name = "Profile Name")]
        public string Name { get; set; }

        [RequiredAttribute]
        [Display(Name = "Profile Key")]
        [RegularExpression(@"^[a-zA-Z0-9_-]*$", ErrorMessage="Only alpha/numeric characters (without spaces) allowed.")]
        public string UrlKey { get; set; }

        [RequiredAttribute]
        [Display(Name = "Domain Name")]
        public string DefaultDomainName { get; set; }

        [RequiredAttribute]
        [Display(Name = "Domain Description")]
        public string DomainDescription { get; set; }

        public int DefaultAreaTypeId { get; set; }
        public int ContactUserId { get; set; }
        public string ExtraJsFiles { get; set; }
        public string ExtraCssFiles { get; set; }
        public int EnumParentDisplay { get; set; }
        public string AreasIgnoredForSpineChart { get; set; }
        public int KeyColourId { get; set; }
        public int DefaultFingertipsTabId { get; set; }
        
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
        
        public IEnumerable<ProfileAreaType> SelectedPdfAreaTypes { get; set; }

        public IEnumerable<ProfileUser> ProfileUsers { get; set; }


    }
}