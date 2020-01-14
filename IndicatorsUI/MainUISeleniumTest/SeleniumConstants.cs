
namespace IndicatorsUI.MainUISeleniumTest
{
    public class SeleniumUser
    {
        public const string EmailAddress = "user@seleniumtest.com";
        public const string Password = "Hello123";

        public const string EmailAddressForPredefinedLists = "user@listsfortesting.com";
        public const string PasswordForPredefinedLists = "Hello123";
    }

    public class PublicHealthDashboardIds
    {
        public const string Map = "map";
        public const string RankingsHeader = "ranking-header";
        public const string RankingsNav = "rankings-nav";

        // Area search
        public const string AreaSearchAutocompleteOptions = "ui-id-1";
    }

    public class FingertipsIds
    {
        public const string TartanRugIndicatorNameOnFirstRow = "rug-indicator-0";
        public const string AreaMenu = "areaMenu";
        public const string ParentAreaMenu = "regionMenu";
        public const string AreaSearchText = "area-search-text";
        public const string AreaSearchLink = "area-search-link";
        public const string ProfilePerIndicator = "indicator-profile-origin-link";
        public const string ListOfIndicators = "search-indicator-list";
        public const string InequalitiesTrends = "inequalities-tab-option-1";
        public const string InequalitiesLatestValues = "inequalities-tab-option-0";
        public const string GpPracticeSearchText = "gp-practice-search-text";
        public const string GpPracticeAutoComplete = "typeahead-container";
        
        // Tabs
        public const string TabOverview = "page-overview";
        public const string TabCompareIndicators = "page-compare-indicators";
        public const string TabMap = "page-map";
        public const string TabTrends = "page-trends";
        public const string TabCompareAreas = "page-indicators";
        public const string TabAreaProfiles = "page-area-profile";
        public const string TabInequalities = "page-inequalities";
        public const string TabEngland = "page-england";
        public const string TabPopulation = "page-population";
        public const string TabDefinitions = "page-metadata";
        public const string TabDownload = "page-download";
        public const string TabBoxPlot = "page-boxplots";
    }
    
    public class Classes
    {
        public const string AreaName = "area_name";
        public const string NoSearchResultMessage = "tall-central-message";
        public const string InformationTooltip = "info-tooltip";
        public const string InformationToolTipWithPosition = "info-tooltip-with-position";
        public const string CloseIcon = "close";
    }

    public class AreaNames
    {
        public const string EastSussex = "East Sussex";
    }

    public class XPaths
    {
        // Navigation header links
        public const string NavHome = "//*[@id='home-nav']/a";
        public const string NavNationalComparisons = "//*[@id='rankings-nav']/a";
        public const string NavMap = "//*[@id='map-with-data']/a";
        public const string NavAboutTheData = "//*[@id='about-data-nav']/a";

        // Originating profile list
        public const string ListOfProfilesInPopup = "//*[@id='profiles-for-selected-indicator']/div/ul/li";
    }
}
