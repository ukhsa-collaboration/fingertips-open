using Profiles.DataAccess;

namespace MainUISeleniumTest
{
    public class LongerLivesIds
    {
        public const string Map = "map";
        public const string AreaTypeLinkCcgs = "19";
        public const string AreaTypeLinkCountyUa = "102";
        public const string AreaTypeLinkDistrictUa = "101";       
        public const string InfoBox1 = "info_box_1";
        public const string InfoBox2 = "info_box_2";
        public const string RankingsHeader = "ranking-header";
        public const string RankingsNav = "rankings-nav";
        public const string GpCount = "gp_count";
        public const string DiabetesRankingsTable = "diabetes-rankings-table";
        public const string TartanRugIndicatorNameOnFirstRow = "rug-indicator-0";
        public const string MortalityRankingPopulation = "national-population";
        public const string MortalityPrematureDeaths = "national-overall";
        public const string MortalityRankingsTable = "mortality-rankings-table";

        // Area search
        public const string AreaSearchAutocompleteOptions = "ui-id-1";
    }

    public class FingertipsIds
    {
        public const string AreaMenu = "areaMenu";
        public const string AreaSearchText = "area-search-text";
        public const string AreaSearchLink = "area-search-link";
        public const string ProfilePerIndicator = "indicator-profile-origin-link";
        public const string ListOfIndicators="search-indicator-list";
    }

    public class Classes
    {
        public const string MapPopUp = "map-info-box";
        public const string AreaName = "area_name";
        public const string PracticePopulation = "practice_population";
        public const string NoSearchResultMessage = "tallCentralMessage";
        public const string MortalityRankingsHeader = "header";
        public const string PrimaryValueNoteTooltip = "primary-value-note-tooltip";
        public const string BarchartIndicatorTitle = "trendTitle";
    }

    public class AreaNames
    {
        public const string CcgWestLancashire = "NHS West Lancashire CCG";
        public const string CcgWalthamForest = "NHS Waltham Forest CCG";
        public const string CountyUaBathAndNorthEastSomerset = "Bath and North East Somerset";
        public const string DistrictUaTendring = "Tendring";
    }    

    public class XPaths
    {
        // Navigation header links
        public const string NavHome = "//*[@id='home-nav']/a";
        public const string NavNationalComparisons = "//*[@id='rankings-nav']/a";        
        public const string NavAboutTheProject = "//*[@id='about-project-nav']/a";
        public const string NavAboutTheData = "//*[@id='about-data-nav']/a";
        public const string NavKeyMessages = "//*[@id='key-messages-nav']/a";
        public const string NavConnect = "//*[@id='connect-nav']/a";

        public const string MortalityRankingsTableTrs = "//*[@id='mortality-rankings-table']/tbody/tr";
        public const string DiabetesRankingsTableTrs = "//*[@id='diabetes-rankings-table']/tbody/tr";  
      
        // Area type links on rankings page
        public const string AreaTypeLinkCcgs = "//*[@id='19']/a";
        public const string AreaTypeLinkDistrictUas = "//*[@id='101']/a";
        public const string AreaTypeLinkCountyUas = "//*[@id='102']/a";

        public const string DrugsAndAlcoholAreaRankingsTableName = "//*[@id='data_page_table']/thead/tr/th[2]/div/span";

        // Specific Indicators
        public const string PeopleWithDiabetesMeetingTreatmentTargets = "//*[@id='iid-90717']/a";

        // Originating profile list
        public const string ListOfProfilesInPopup = "//*[@id='profiles-for-selected-indicator']/div/ul/li";
    }
}
