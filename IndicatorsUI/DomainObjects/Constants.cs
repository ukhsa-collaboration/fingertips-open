
namespace Profiles.DomainObjects
{
    public class UserNames
    {
        public const string Doris = "doris.hain";
    }

    public class ActiveDirectoryGroups
    {
        public const string FpmUsers = "Global.Fingertips.FingertipsProfileManager";
        public const string LibraryServices = "Global.CKO.LibraryServices";
    }

    public class ContentKeys
    {
        public const string Description = "description";
        public const string Introduction = "introduction";
        public const string RecentUpdates = "recent-updates";
        public const string Twitter = "twitter";
        public const string ContactUs = "contact-us";
        public const string FurtherInfo = "further-info";
        public const string Faqs= "faqs";
        public const string Inequalities = "inequalities";
        public const string Evidence = "evidence";

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

        /// <summary>
        /// Description of the site that appears in the meta keywords tag in the web page.
        /// </summary>
        public const string MetaKeywords = "meta-keywords";
    }

    public class ProfileUrlKeys
    {
        public const string AdultSocialCare = "adultsocialcare";
        public const string SevereMentalIllness = "severe-mental-illness";
        public const string ChildrenAndYoungPeoplesHealthBenchmarkingTool = "cyphof";
        public const string Phof = "public-health-outcomes-framework";
        public const string Tobacco = "tobacco-control";
        public const string LongerLives = "mortality";
        public const string TbStrategy = "tb-monitoring";
        public const string Diabetes = "diabetes";
        public const string HealthProfiles = "health-profiles";
        public const string SexualHealth = "sexualhealth";
        public const string PracticeProfiles = "general-practice";
        public const string MarmotHealthyLifeExpectancy = "marmot-healthy-life-expectancy";
        public const string MarmotIndicatorsForLocalAuthorities = "marmot-indicators";
    }

    public class ProfileIds
    {
        public const int Undefined = -1;
        public const int HealthInequalities = 7;
        public const int SubstanceMisuse = 17;
        public const int Tobacco = 18;
        public const int Phof = 19;
        public const int PracticeProfiles = 20;
        public const int LongerLives = 22;
        public const int HealthProfiles = 26;
        public const int MentalHealth = 36;
        public const int SevereMentalIllness = 41;
        public const int SexualHealth = 45;
        public const int Diabetes = 51;
        public const int LiverDisease = 55;
        public const int HealthChecks = 77;
        public const int DrugsAndAlcohol = 75;
    }

    public class GroupIds
    {
        public const int PracticeProfiles_SupportingIndicators = 1200006;
        public const int SevereMentalIllness_RiskFactors = 8000027;
        public const int SevereMentalIllness_Prevalence = 8000030;
        public const int SevereMentalIllness_Finance = 8000040;
        public const int Phof_HealthImprovment = 1000042;
        public const int Phof_PrematureMortality = 1000044;
        public const int DomainThatDoesNotExist = 123456;
    }

    public class IndicatorIds
    {
        public const int GapInLifeExpectancyAtBirth = 90365;
    }

    public class TabIds
    {
        public const int BarChart = 3;
        public const int ScatterPlot = 10;
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
        public const int DistrictAndUnitaryAuthority = 101;
        public const int CountyAndUnitaryAuthority = 102;
        public const int CCG = 19;
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
        public const string CcgNhsNorthumberland = "E38000130";
        public const string CcgWalthamForest = "E38000192";

        // GP Practices
        public const string GpPracticeThatchedHouseMedicalCentre = "F86639";
        public const string GpPracticeWestminsterAndPimlico = "E87014";
        public const string Cambridge = "Cambridge";

        //Regions
        public const string RegionEastOfEngland = "E12000006";

        // Country
        public const string England = "E92000001";
    }

    public class SkinIds
    {
        public const int Core= 2;
        public const int Phof = 3;
        public const int Tobacco = 4;
        public const int LongerLives = 5;
    }

    public class ProfileCollectionIds
    {
        public const int NationalProfiles = 4;
    }

    public class SpineChartMinMaxLabels
    {
        public const int DeriveFromLegendColours = 0;
        public const int LowestAndHighest = 1;
        public const int WorstAndBest = 2;
        public const int WorstLowestAndBestHighest = 3;
    }
}