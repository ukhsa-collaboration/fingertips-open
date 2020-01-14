using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.MainUI.Skins;
using System.Collections.Generic;

namespace IndicatorsUI.MainUI.Models
{
    public class PageModel
    {
        private readonly IAppConfig appConfig;
        private Skin skin;

        public PageModel(IAppConfig appConfig)
        {
            this.appConfig = appConfig;
            ProfileId = ProfileIds.Undefined;
        }

        public PageType PageType = PageType.Undefined;

        public Skin Skin
        {
            get
            {
                // Need to return null skin to prevent exceptions in dev environment when browser dev tools window is open
                return skin ?? SkinFactory.NullSkin();
            }
            set { skin = value; }
        }

        public string BridgeServicesUrl { get; set; }
        public string CoreServicesUrl { get; set; }
        public string PageTitle { get; set; }
        public string IgnoredSpineChartAreas { get; set; }
        public string CssPath { get; private set; }
        public string JsPath { get; private set; }

        public bool UseMinifiedJavaScript { get; set; }

        public string GetTwitterHandle()
        {
            if (ProfileId == ProfileIds.Phof)
            {
                return "PHoutcomes";
            }

            if (skin.IsLongerLives)
            {
                return "PHE_uk";
            }

            return "";
        }

        public bool IsOfficialStatistics { get; set; }
        public bool HasRecentTrends { get; set; }
        public bool UseTargetBenchmarkByDefault { get; set; }

        /// <summary>
        /// Whether or not PDFs are available for the profile.
        /// </summary>
        public bool AreAnyPdfsForProfile { get; internal set; }

        public bool StartZeroYAxis { get; set; }
        public int DefaultFingertipsTabId { get; set; }

        /// <summary>
        /// Whether to display the profile title instead of the skin title.
        /// </summary>
        public bool DisplayProfileTitle { get; set; }

        public bool HasExclusiveSkin { get; set; }
        public string FrontPageAreaSearchAreaTypes { get; set; }
        public bool HasAnyData { get; set; }
        public bool HasStaticReports { get; set; }
        public int ProfileId { get; set; }

        public IList<ProfileCollection> ProfileCollections { get; set; }
        public ProfileCollection NationalProfileCollection { get; set; }

        public ProfileCollection HighlightedProfileCollection { get; set; }

        /// <summary>
        ///     Used to limit search results and parent areas that are displayed in the region menu.
        /// </summary>
        public int TemplateProfileId { get; set; }

        public string Title
        {
            get
            {
                return Skin.IsTitle
                    ? Skin.Title
                    : GetCoreTitle();
            }
        }

        public bool IsTwitterAccount
        {
            get { return ProfileId == ProfileIds.Phof; }
        }

        public void SetJavaScriptVersionFolder(string folder)
        {
            string url = appConfig.StaticContentUrl + folder;

            JsPath = url + "js/";
            CssPath = url + "css/";
        }

        private bool IsPageTitle()
        {
            return string.IsNullOrWhiteSpace(PageTitle) == false;
        }

        private string GetCoreTitle()
        {
            string tagLine = IsPageTitle()
                ? " - " + PageTitle
                : string.Empty;

            return "Public Health Profiles" + tagLine;
        }

    }
}