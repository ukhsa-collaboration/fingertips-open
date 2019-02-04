using System.Collections.Generic;
using System.Linq;

namespace IndicatorsUI.DomainObjects
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
        public string AreasToIgnoreEverywhere { get; set; }
        public int DefaultAreaType { get; set; }
        public int EnumParentDisplay { get; set; }
        public bool ArePdfs { get; set; }
        public bool StartZeroYAxis { get; set; }
        public int DefaultFingertipsTabId { get; set; }
        public int SkinId { get; set; }
        public bool HasOwnFrontPage { get; set; }

        public string ProfileCollectionIdsString { get; set; }
        public List<int> ProfileCollectionIds
        {
            get
            {
                return new IntListStringParser(ProfileCollectionIdsString).IntList;
            }
        }

        public string ExtraJavaScriptFilesString { get; set; }
        public string ExtraCssFilesString { get; set; }
        public bool ShowDataQuality { get; set; }
        public bool IsNational { get; set; }
        public bool HasRecentTrends { get; set; }
        public bool UseTargetBenchmarkByDefault { get; set; }
        public string AccessControlGroup { get; set; }
        public LongerLivesProfileDetails LongerLivesProfileDetails { get; set; }
        public bool IsOfficialStatistics { get; set; }
        public string FrontPageAreaSearchAreaTypes { get; set; }
        public bool HasAnyData { get; set; }
        public bool HasStaticReports { get; set; }
        public string StaticReportsFolders { get; set; }
        public string StaticReportsLabel { get; set; }

        /// <summary>
        /// Whether to display the trend marker for the change between the most recent data
        /// and the previous time period
        /// </summary>
        public bool IsChangeFromPreviousPeriodShown { get; set; }

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

        /// <summary>
        /// Title for displaying to the user. Any internal extensions are removed
        /// </summary>
        public string DisplayTitle
        {
            get
            {
                // Remove longer lives extension
                if (LongerLivesProfileDetails != null)
                {
                    return Title.Replace("- Longer Lives", "");
                }

                return Title;
            }
        }
    }
}
