using System;
using System.Collections.Generic;
using System.Linq;

namespace Profiles.DomainObjects
{
    public class ProfileDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ProfileUrlKey { get; set; }
        public IList<Domain> Domains { get; set; }
        public IList<string> ExtraJavaScriptFiles { get; set; }
        public IList<string> ExtraCssFiles { get; set; }
        public string AreasToIgnoreForSpineCharts { get; set; }
        public int DefaultAreaType { get; set; }
        public int EnumParentDisplay { get; set; }
        public int RagColourId { get; set; }
        public bool ArePdfs { get; set; }
        public bool StartZeroYAxis { get; set; }
        public int DefaultFingertipsTabId { get; set; }
        public int SkinId { get; set; }
        public bool HasOwnFrontPage { get; set; }
        public int? LeadProfileForCollectionId { get; set; }
        public string ExtraJavaScriptFilesString { get; set; }
        public string ExtraCssFilesString { get; set; }
        public bool ShowDataQuality { get; set; }
        public bool IsNational { get; set; }
        public bool HasTrendMarkers { get; set; }
        public string AccessControlGroup { get; set; }
        public LongerLivesProfileDetails LongerLivesProfileDetails { get; set; }
        public bool IsOfficialStatistics { get; set; }
        public bool HasDomains
        {
            get { return Domains.Any(); }
        }

        /// <summary>
        /// Does the profile have a skin that is used only for it.
        /// </summary>
        public bool HasExclusiveSkin
        {
            get { return SkinId != SkinIds.Core; }
        }

        public SpineChartMinMaxLabel SpineChartMinMaxLabel { get; set; }

    }
}
