
namespace IndicatorsUI.DomainObjects
{
    public class UserNames
    {
        public const string Doris = "doris.hain";
    }

    public class ActiveDirectoryGroups
    {
        public const string FpmUsers = "Global.Fingertips.FingertipsProfileManager";
        public const string LibraryServices = "Global.CKO.LibraryServices";
        public const string Phof = "Global.Fingertips.AccessControlPhof";
    }

    public class ContentKeys
    {
        public const string Description = "description";
        public const string Introduction = "introduction";
        public const string IntroductionSidebar = "introduction-sidebar";
        public const string RecentUpdates = "recent-updates";
        public const string Twitter = "twitter";
        public const string ContactUs = "contact-us";
        public const string FurtherInfo = "further-info";
        public const string Faqs = "faqs";
        public const string FurtherInformation = "further-information";
        public const string FurtherResources = "further-resources";

        /// <summary>
        /// The instructions on the map page of Longer Lives
        /// </summary>
        public const string MapHelp = "help-guide";
        public const string ConnectHeader = "connect-header";
        public const string ConnectHeaderText = "connect-header-text";
        public const string ConnectBodyText = "connect-body-text";

        /// <summary>
        /// Extra download link on rankings page
        /// </summary>
        public const string DownloadLink = "download-link";

        /// <summary>
        /// Description of the site that appears in the meta description tag in the web page.
        /// </summary>
        public const string MetaDescription = "meta-description";
    }

    public class ProfileUrlKeys
    {
        public const string SevereMentalIllness = "severe-mental-illness";
        public const string Phof = "public-health-outcomes-framework";
        public const string LongerLives = "mortality";
        public const string TbStrategy = "tb-monitoring";
        public const string Diabetes = "diabetes";
        public const string HealthProfiles = "health-profiles";
        public const string Populations = "populations";
        public const string SexualHealth = "sexualhealth";
        public const string MentalHealth = "mental-health";
        public const string PracticeProfiles = "general-practice";
        public const string MarmotIndicatorsForLocalAuthorities = "marmot-indicators";
        public const string PublicHealthDashboard = "public-health-dashboard";
        public const string DeveloperTestProfile = "devtestprofile";
    }

    public class ProfileIds
    {
        public const int Undefined = -1;
        public const int Tobacco = 18;
        public const int Phof = 19;
        public const int PracticeProfiles = 20;
        public const int HealthProfiles = 26;
        public const int SevereMentalIllness = 41;
        public const int SexualHealth = 45;
        public const int LiverDisease = 55;
        public const int SuicidePrevention = 73;
        public const int HealthChecks = 77;
        public const int LocalAlcoholProfiles = 87;
        public const int ChildHealth = 105;
        public const int WiderDeterminantsOfHealth = 130;
        public const int Populations = 132;
        public const int Diabetes = 139;
        public const int PublicHealthDashboard = 140;
    }

    public class GroupIds
    {
        public const int SevereMentalIllness_PsychosisPathway = 1938132719;
        public const int SevereMentalIllness_RiskFactors = 8000027;
        public const int SevereMentalIllness_Finance = 8000040;
        public const int Phof_HealthImprovment = 1000042;
        public const int Phof_WiderDeterminantsofHealth = 1000041;
        public const int Phof_HealthProtection = 1000043;
        public const int Phof_PrematureMortality = 1000044;
        public const int Phof_OverarchingIndicators = 1000049;
        public const int DomainThatDoesNotExist = 123456;
    }

    public class IndicatorIds
    {
        public const int GapInLifeExpectancyAtBirth = 90365;
        public const int LifeExpectancyAtBirth = 90366;
    }

    public class SexIds
    {
        public const int Male = 1;
        public const int Female = 2;
        public const int Persons = 4;
    }

    public class AgeIds
    {
        public const int AllAges = 1;
    }

    public class AreaTypeIds
    {
        public const int Region = 6;
        public const int Ward = 8;
        public const int CountyAndUnitaryAuthority = 102;
        public const int AcuteTrusts = 118;
        public const int CcgPostApr2017 = 152;
        public const int CcgPreApr2017 = 153;
        public const int CcgSince2018 = 154;
    }

    public class AreaCodes
    {
        // Local authorities
        public const string Derby = "E06000015";
        public const string Northumberland = "E06000057";
        public const string IslesOfScilly = "E06000053";
        public const string Croydon = "E09000008";
        public const string Hartlepool = "E06000001";

        // CCGs
        public const string CcgWalthamForest = "E38000192";

        // GP Practices
        public const string GpPracticeThatchedHouseMedicalCentre = "F86639";

        //Regions
        public const string RegionEastOfEngland = "E12000006";

        // Country
        public const string England = "E92000001";
    }

    public class SkinIds
    {
        public const int Core = 2;
        public const int LongerLives = 5;
    }

    public class ProfileCollectionIds
    {
        public const int NationalProfiles = 4;
        //For Highlighted
        public const int HighlightedProfiles = 11;
    }

    public class TabIds
    {
        public const int TartanRug = 0;
        public const int AreaProfile = 1;
        public const int CompareAreas = 3;
        public const int Inequalities = 7;
        public const int Map = 8;
        public const int ScatterPlot = 10;
        public const int Reports = 13;
        public const int Trends = 4;
        public const int Population = 12;
        public const int England = 11;
    }

    public class NeighbourTypes
    {
        public const int CIPFA = 1;
        public const int TenMostSimilarCCGs = 2;
        public const int ChildrensServicesStatisticalNeighbours = 3;
    }

    public class NotifyEmailTemplates
    {
        public const string ResetPassword = "3e7a8124-68cc-4fb8-b21e-b030df40ec60";
        public const string VerifyEmailAddress = "b2d4e6f5-1f62-47ff-a9fd-0b06793a4a1a";
    }

    public class MaximumFieldLengths
    {
        public const int EmailAddress = 50;
        public const int Password = 25;
    }

    public class FeatureFlags
    {
        public const string EmailVerification = "emailVerification";
    }

    public class GoogleAnalytics
    {
        public const string EventCollectionUrl = "https://www.google-analytics.com/collect";
        public const int CodeFromAnonymousClient = 555;
        public const string TrackingLiveId = "UA-43455403-1";
        public const string TrackingDevelopmentId = "UA-43455403-16";
        public const int GoogleAnalyticsVersion = 1;
    }
}