namespace Fpm.ProfileData
{
    public class ExceptionOptions
    {
        public const string AllServers = "ALL SERVERS";
    }

    public class UserDisplayNames
    {
        public const string Doris = @"Doris Hain";
        public const string UserWithNoRightsToAnything = @"Tim Packer";
    }

    public class ContentKeys
    {
        public const string Description = "description";
        public const string Introduction = "introduction";
        public const string RecentUpdates = "recent-updates";
    }

    public class SiteUrls
    {
        public const string TargetIndex = "/lookup-tables/targets";
    }

    public class YearTypeIds
    {
        public const int Calendar = 1;
        public const int Financial = 2;
        public const int Academic = 3;
        public const int FinancialMultiYearCumulativeQuarter = 10;
    }

    public class AreaTypeIds
    {
        public const int Undefined = 0;
        public const int LocalAuthority = 1;
        public const int Pct = 2;
        public const int Sha = 5;
        public const int Gor = 6;
        public const int GpPractice = 7;
        public const int Ward = 8;
        public const int County = 9;
        public const int CountyQuintile = 10;
        public const int Country = 15;
        public const int UnitaryAuthority = 16;
        public const int GpDeprivationDecile = 17;
        public const int Shape = 18;
        public const int Ccg = 19;
        public const int MentalHealthTrust = 20;
        public const int PheCentre = 43;
        public const int LocalAuthorityAndUnitaryAuthority = 101;
        public const int CountyAndUnitaryAuthority = 102;
        public const int OnsClusterGroup = 110;
    }

    public class AreaCodes
    {
        public const string CountyUa_Cambridgeshire = "E10000003";
        public const string CountyUa_Bedfordshire = "09";
        public const string CountyUa_CityOfLondon = "E09000001";
        public const string CountyUa_IslesOfScilly = "E06000053";
    }

    public class SexIds
    {
        public const int Male = 1;
        public const int Female = 2;
        public const int Persons = 4;
    }

    public class GroupIds
    {
        public const int SubstanceMisuse = 1000009;
        public const int TobaccoControlProfiles_KeyIndicators = 1938132885;
        public const int PhofWiderDeterminantsOfHealth = 1000041;
        public const int GpProfileSupportingIndicators = 1200006;
        public const int ArchivedIndicators = 3006000;
        public const int SevereMentalIllness_RiskFactors = 8000027;
        public const int SevereMentalIllness_Prevalence = 8000030;
        public const int SevereMentalIllness_Finance = 8000040;
    }

    public class IndicatorIds
    {
        public const int IDAOPI = 125;
        public const int PeopleWhoDieAtHome = 1114;
        public const int LongTermUnemployment = 734;
        public const int ChildrenInPoverty = 10101;
        public const int Under18Conceptions = 20401;
        public const int HipFractures = 41402;
        public const int ObesityYear6 = 90323;
        public const int LifeExpectancyAtBirth = 90366;
        public const int MrsaBloodstreamInfections = 91317;
        public const int IndicatorThatDoesNotExist = 1234567;
    }

    public class AgeIds
    {
        public const int AllAges = 1;
        public const int Years10To11 = 201;
        public const int LessThan16 = 169;
    }

    public class ProfileIds
    {
        public const int Undefined = -1;
        public const int ProfileThatDoesNotExist = 1;
        public const int Search = 13;
        public const int Phof = 19;
        public const int UnassignedIndicators = 23;
        public const int ArchivedIndicators = 24;
        public const int HealthProfiles = 26;
        public const int ChildrenAndYoungPeoplesHealthBenchmarkingTool = 39;
        public const int SexualHealth = 45;
        public const int Diabetes = 51;
    }

    public class UrlKeys
    {
        public const string Tobacco = "tobacco-control";
        public const string HealthProfiles = "health-profiles";
        public const string Phof = "public-health-outcomes-framework";
        public const string SevereMentalIllness = "severe-mental-illness";
    }

    public class Frequencies
    {
        public const int Annual = 1;
        public const int Quarterly = 2;
        public const int Monthly = 3;
    }

    public class FpmUserIds
    {
        public const int Doris = 11;
        public const int UserWithNoRightsToAnything = 34/*Tim Packer*/;
        public const int FarrukhAyub = 54;
    }

    public class PolarityIds
    {
        public const int NotApplicable = -1;
        public const int RagLowIsGood = 0;
        public const int RagHighIsGood = 1;
        public const int UseBlues = 99;
    }

    public class ValueNoteIds
    {
        public const int ThereIsDataQualityIssueWithThisValue = 401;
    }

    public class CategoryTypeIds
    {
        public const int Undefined = -1;
        public const int EthnicGroups7Categories = 4;
    }

    public class CategoryIds
    {
        public const int Undefined = -1;
        public const int Mixed = 2;
        public const int Black = 4;
    }

    public class SkinIds
    {
        public const int Core = 2;
    }

    public class IndicatorTextMetadataPropertyIds
    {
        public const int Name = 1;
    }

    public class ComparatorIds
    {
        public const int Subnational = 1;
        public const int National = 4;

        /// <summary>
        /// This is internal application constant and should not be saved to the database
        /// </summary>
        public const int NationalAndSubnational = 999;
    }

    public class DocumentNames
    {
        public const string DiabetesHospitalData = "Diabetes_Hospital_Data.xlsx";
    }

    public class CoreDataFilters
    {
        public const string CategoryTypeId = "CategoryTypeId";
        public const string AreaTypeId = "AreaTypeId";
        public const string AgeId = "AgeId";
        public const string SexId = "SexId";
        public const string YearRange = "YearRange";
        public const string Year = "[Year]";
        public const string Month = "[Month]";
        public const string Quarter = "[Quarter]";
    }


}