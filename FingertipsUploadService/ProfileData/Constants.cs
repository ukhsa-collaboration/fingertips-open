namespace FingertipsUploadService.ProfileData
{
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