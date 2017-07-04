
namespace PholioVisualisation.PholioObjects
{
    public class AreaTypeIds
    {
        public const int Undefined = 0;
        public const int District = 1;
        public const int Pct = 2;
        public const int Lsoa = 4;
        public const int Sha = 5;
        public const int GoRegion = 6;
        public const int GpPractice = 7;
        public const int County = 9;
        public const int CountyQuintile = 10;
        public const int AcuteTrust = 14;
        public const int Country = 15;
        public const int UnitaryAuthority = 16;
        public const int Shape = 18;
        public const int Ccg = 19;
        public const int MentalHealthTrust = 20;
        public const int AmbulanceTrust = 41;
        public const int PheCentreObsolete = 43;
        public const int Subregion = 46;
        public const int PheCentreFrom2013 = 48;
        public const int PheCentreFrom2015 = 49;
        public const int PheCentreFrom2013To2015 = 50;
        public const int CountyAndUnitaryAuthorityDeprivationDecile = 23;
        public const int DistrictAndUnitaryAuthority = 101;
        public const int CountyAndUnitaryAuthority = 102;
        public const int PheCentresFrom2013To2015 = 103;
        public const int PheCentresFrom2015 = 104;
        public const int OnsClusterGroup2001 = 110;
        public const int StrategicClinicalNetwork = 112;
        public const int OnsClusterGroup2011 = 115;
        public const int CombinedMentalHealthAndAcuteTrust = 117;
        public const int AcuteTrustsIncludingCombinedMentalHealthTrusts = 118;
        public const int MentalHealthTrustsIncludingCombinedAcuteTrusts = 119;
        public const int Stp = 120;
    }

    public class CategoryIds
    {
        public const int Undefined = -1;

        // Deprivation
        public const int MostDeprivedDecile = 1;
        public const int MostDeprivedQuintile = 1;
        public const int LeastDeprivedQuintile = 5;

        // Ethnicity (7 groups)
        public const int EthnicityWhite = 1;
        public const int EthnicityMixed = 2;
        public const int EthnicityAsian = 3;
        public const int EthnicityBlack = 4;
        public const int EthnicityOther = 5;
    }

    public class CategoryTypeIds
    {
        public const int Undefined = -1;
        public const int DeprivationDecileGp2010 = 1;
        public const int DeprivationDecileCountyAndUA2010 = 2;
        public const int DeprivationDecileDistrictAndUA2010 = 3;
        public const int EthnicGroups7 = 4;
        public const int LimitsForHealthProfilesLifeExpectancyChart = 5;
        public const int LsoaDeprivationQuintilesInEngland2010 = 6;
        public const int LsoaDeprivationDecilesWithinArea2010 = 9;
        public const int LsoaDeprivationQuintilesWithinArea2010 = 10;
        public const int EthnicGroups5 = 33;
        public const int DeprivationDecileGp2015 = 38;
        public const int DeprivationDecileCountyAndUA2015 = 39;
        public const int DeprivationDecileDistrictAndUA2015 = 40;
        public const int LsoaDeprivationQuintilesInEngland2015 = 41;
        public const int LsoaDeprivationDecilesWithinArea2015 = 48;
        public const int LsoaDeprivationQuintilesWithinArea2015 = 50;
    }

    public class SexIds
    {
        public const int NotApplicable = -1;
        public const int Male = 1;
        public const int Female = 2;
        public const int Persons = 4;
    }

    public class ValueTypeIds
    {
        public const int DirectlyStandardisedRate = 1;
        public const int CrudeRate = 3;
        public const int Proportion = 5;
        public const int Count = 7;
    }

    public class ValueNoteIds
    {
        public const int NoNote = 0;

        // Value calculated on the fly
        public const int ValueAggregatedFromAllKnownGeographyValues = 500;

        // Value precalculated to be stored in PHOLIO
        public const int ValueAggregatedFromAllKnownGeographyValuesByFingertips = 506;
    }

    public class AgeIds
    {
        public const int AllAges = 1;
        public const int From5To9 = 4;
        public const int From10To14 = 5;
        public const int From15To19 = 6;
        public const int From20To24 = 7;
        public const int From25To29 = 8;
        public const int From30To34 = 9;
        public const int From35To39 = 10;
        public const int From40To44 = 11;
        public const int From45To49 = 12;
        public const int From50To54 = 13;
        public const int From55To59 = 14;
        public const int From60To64 = 15;
        public const int From65To69 = 16;
        public const int From70To74 = 17;
        public const int From75To79 = 18;
        public const int From80To84 = 19;
        public const int Over85 = 20;
        public const int From85To89 = 21;
        public const int Over90 = 22;
        public const int Over65 = 27;
        public const int From0To4 = 28;
        public const int Is6 = 35;
        public const int Is65 = 94;
        public const int Under75 = 163;
        public const int Over18 = 168;
        public const int Under16 = 169;
        public const int Weeks6To8 = 170;
        public const int Plus17 = 187;
        public const int Plus15 = 188;
        public const int From5To15 = 193;
        public const int From10To11 = 201;
        public const int From16To64 = 204;
        public const int Over60 = 214;
        public const int From18To75 = 234;
        public const int From90To95 = 281;
        public const int Over95 = 282;
    }

    public class IndicatorIds
    {
        public const int IndicatorHasNoData = 2;
        public const int OverallPrematureDeaths = 108;
        public const int AdultSmokingRelatedDeaths = 113;
        public const int QofListSize = 114;
        public const int IDAOPI = 125;
        public const int QofPoints = 295;
        public const int QuinaryPopulations = 337;
        public const int DeprivationScoreIMD2010 = 338;
        public const int IDACI = 339;
        public const int IDAOPI2 = 340;
        public const int PatientsThatWouldRecommendPractice = 347;
        public const int Aged0To4Years = 639;
        public const int AgedOver85Years = 642;
        public const int LifeExpectancyMsoaBasedEstimate = 650;
        public const int EstimatedPrevalenceOfCHD = 720;
        public const int PercentageLivingInEachDeprivationQuintile = 734;
        public const int AdultSmokingPrevalence = 750;
        public const int YoungPeopleInTreatmentNotRetainedForMoreThan12Weeks = 909;
        public const int VulnerablePeopleAchievingIndependentLiving = 1116;
        public const int Population = 1170;
        public const int IncidenceOfMalignantMelanoma = 1763;
        public const int DeathsFromLungCancer = 1203;
        public const int OralCancerRegistrations = 1206;
        public const int EthnicityEstimates = 1679;
        public const int DeprivationBottom20Percent = 1730;
        public const int HealthInequalitiesEmergencyHospitalAdmissionsByEthnicGroup = 2538;
        public const int ChildrenInLowIncomeFamilies = 10101;
        public const int PupilAbsence = 10301;
        public const int KilledAndSeriouslyInjuredOnRoads = 11001;
        public const int ViolentCrime = 11202;
        public const int StatutoryHomelessness = 11501;
        public const int BreastfeedingInitiation = 20201;
        public const int Breastfeeding6To8Weeks = 20202;
        public const int SmokingAtTimeOfDelivery = 20301;
        public const int TeenagePregnancy = 20401;
        public const int HospitalStaysForSelfHarm = 21001;
        public const int VaccinationCoverageDatp = 30303;
        public const int MortalityRateFromCausesConsideredPreventable = 40301;
        public const int AdultUnder75MortalityRateCvd = 40401;
        public const int AdultUnder75MortalityRateCancer = 40501;
        public const int HipFractures = 41401;
        public const int AdultPhysicalActivity = 90275;
        public const int ObesityYear6 = 90323;
        public const int HealthyLifeExpectancyAtBirth = 90362;
        public const int LifeExpectancyAtBirth = 90366;
        public const int SchoolReadiness = 90634;
        public const int AdultExcessWeight = 90640;
        public const int ExcessWinterDeaths = 90641;
        public const int BMIRecorded = 90689;
        public const int SyphilisDiagnosis = 90742;
        public const int ObeseAdults = 90743;
        public const int HIVLateDiagnosis = 90791;
        public const int IncidenceOfMalignantMelanoma2 = 90834;
        public const int PercentageOfPeoplePerDeprivationQuintile = 90851;
        public const int XAxisOfHealthProfilesChartAtBottomOfPage2 = 90852;
        public const int EstimatedPrevalenceCommonMentalHealthDisorders = 90853;
        public const int LifeExpectancyAt65 = 91102;
        public const int LongTermUnemployment = 91133;
        public const int NumberInTreatmentAtSpecialistDrugMisuseServices = 91181;
        public const int SexuallyTransmittedInfection = 91306;
        public const int IncidenceOfTB = 91361;
        public const int TreatmentCompletionForTB = 91367;
        public const int AlcoholAdmissionsToHospital = 91414;
        public const int GcseAchievement = 92199;
        public const int AdultDrugMisuse = 91467;
        public const int PopulationEating5aDay = 91477;
        public const int DeprivationScoreIMD2015 = 91872;
        public const int AgedUnder18 = 92309;
        public const int StatutoryHomelessness2 = 92314;
        public const int SurgicalSiteInfectionHipProsthesis = 92368;
        public const int NumberPrescribedAntibioticItems = 92377;
        public const int SurgicalSiteInfectionKneeProsthesis = 92420;
        public const int AdultSmokingPrevalence2 = 92443;
        public const int SuicidePreventionPlan = 92607;
        public const int OnsMidYearPopulationEstimates = 92708;
        public const int PercentageEthnicMinorities = 92860;
        public const int DependencyRatio = 92862;
        public const int PopulationProjection = 92869;
        public const int SlopeIndexOfInequalityForLifeExpectancy = 92901;
        public const int Under18AlcoholSpecificStays = 92904;
    }

    public class ConfidenceIntervalMethodIds
    {
        public const int Byars = 2;
    }

    public class UnitIds
    {
        public const int Days = 7;
    }

    public class PolarityIds
    {
        public const int NotApplicable = -1;
        public const int RagLowIsGood = 0;
        public const int RagHighIsGood = 1;
        public const int BlueOrangeBlue = 99;
    }

    public class YearTypeIds
    {
        public const int Calendar = 1;
        public const int Financial = 2;
        public const int Academic = 3;
        public const int FinancialRollingYearQuarterly = 4;
        public const int CalendarRollingYearQuarterly = 5;
        public const int CalendarRollingYearMonthly = 6;
        public const int FinancialSingleYearCumulativeQuarter = 7;
        public const int AugustToJuly = 8;
        public const int MarchToFebruary = 9;
        public const int FinancialMultiYearCumulativeQuarter = 10;
    }

    public class AreaCodes
    {
        public const string NotAnActualCode = "notanactualcode";

        public const string England = "E92000001";

        public const string Sha_EastOfEngland = "E18000006";
        public const string Sha_London = "E18000007";

        public const string Gor_EastMidlands = "E12000004";
        public const string Gor_EastOfEngland = "E12000006";
        public const string Gor_London = "E12000007";
        public const string Gor_NorthEast = "E12000001";
        public const string Gor_NorthWest = "E12000002";
        public const string Gor_SouthEast = "E12000008";
        public const string Gor_SouthWest = "E12000009";
        public const string Gor_WestMidlands = "E12000005";
        public const string Gor_YorkshireHumber = "E12000003";

        public const string CommissioningRegionLondon = "E40000003";

        public const string Pct_Wiltshire = "E16000140";
        public const string Pct_Sheffield = "E16000077";
        public const string Pct_StocktonOnTees = "E16000020";
        public const string Pct_Bedfordshire = "E16000104";
        public const string Pct_Suffolk = "E16000127";
        public const string Pct_Luton = "E16000029";
        public const string Pct_Cambridgeshire = "E16000124";
        public const string Pct_SouthEastEssex = "E16000103";
        public const string Pct_Norfolk = "E16000125";
        public const string Pct_MidEssex = "E16000130";
        public const string Pct_Hounslow = "E16000036";
        public const string Pct_Bassetlaw = "E16000023";
        public const string Pct_Ashton = "E16000032";
        public const string Pct_Bolton = "E16000148";

        public const string CountyUa_Cambridgeshire = "E10000003";
        public const string CountyUa_Bedfordshire = "09";
        public const string CountyUa_Bexley = "e09000004";
        public const string CountyUa_CentralBedfordshire = "E06000056";
        public const string CountyUa_CityOfLondon = "E09000001";
        public const string CountyUa_Bedford = "E06000055";
        public const string CountyUa_Manchester = "E08000003";
        public const string CountyUa_Buckinghamshire = "E10000002";
        public const string CountyUa_Cumbria = "E10000006";
        public const string CountyUa_NorthTyneside = "E08000022";
        public const string CountyUa_Newcastle = "E08000021";
        public const string CountyUa_IslesOfScilly = "E06000053";
        public const string CountyUa_Leicestershire = "E10000018";

        public const string DistrictUa_SouthCambridgeshire = "E07000012";
        public const string DistrictUa_Mansfield = "E07000174";

        public const string Ccg_AireDaleWharfdaleAndCraven = "E38000001";
        public const string Ccg_Barnet = "E38000005";
        public const string Ccg_Chiltern = "E38000033";
        public const string Ccg_CambridgeshirePeterborough = "E38000026";
        public const string Ccg_Wirral = "E38000208";
        public const string Ccg_Farnham = "E38000118";
        public const string Ccg_Kernow = "E38000089";

        public const string La_Wychavon = "E07000238";
        public const string La_Hyndburn = "E07000120";
        public const string La_Haringay = "E09000014";

        public const string Gp_Sawston = "D81043";
        public const string Gp_MeersbrookSheffield = "C88631";
        public const string Gp_Addingham = "B83620";
        public const string Gp_Burnham = "F81126";
        public const string Gp_BermudaBasingstoke = "J82077";
        public const string Gp_Thornbury = "E85001";
        public const string Gp_AdamHouseSandiacre = "C81026";
        public const string Gp_LeasoweWirral = "N85640";
        public const string Gp_KingStreetBlackpool = "P81043";
        public const string Gp_MonkfieldCambourne = "D81637";
        public const string Gp_PracticeInBarnetCcg = "E83027";
        public const string Gp_CrossfellHealthCentre = "A81019";
        public const string Gp_AlbionSurgery = "N82095";

        public const string OnsGroup_ProsperingSouthernEngland = "ONS_5.09";

        public const string NearestNeighbours_Derby = "nn-1-E06000015";

        public const string DeprivationDecile_Utla3 = "cat-2-3";

        public const string Stp_Devon = "E54000037";
    }

    public class GroupIds
    {
        public const int Search = 1;
        public const int HealthInequalities = 1000008;
        public const int Phof_WiderDeterminantsOfHealth = 1000041;
        public const int Phof_HealthcarePrematureMortality = 1000044;
        public const int Phof_HealthProtection = 1000043;
        public const int SexualAndReproductiveHealth = 8000057;
        public const int Population = 1938133081;
        public const int PopulationSummary = 1200006;

        // Practice profiles
        public const int PracticeProfiles_PracticeSummary = 2000005;
        public const int PracticeProfiles_SecondaryCareUse = 2000007;
        public const int PracticeProfiles_IndicatorsForNeedsAssessment = 2000009;
        public const int PracticeProfiles_Cvd = 3000010;
        public const int PracticeProfiles_SupportingIndicators = 1200006;
        public const int PracticeProfiles_Diabetes = 2000002;
        public const int PracticeProfiles_CvdHeartFailure = 3000009;

        public const int SubstanceMisuse_Over18Misuse = 1000031;
        public const int LongerLives = 1000001;

        public const int Diabetes_TreatmentTargets = 1938132700;
        public const int Diabetes_CareProcesses = 1938132728;
        public const int Diabetes_PrevalenceAndRisk = 1938132727;
        public const int DiabetesSupportingIndicators_ValuesAtTopOfPage = 1938132706;

        public const int Hypertension_Detection = 1938132758;
        public const int Hypertension_RiskAndPrevention = 1938132757;
        public const int HealthProfiles_OurCommunities = 3007000;
        public const int HealthProfiles_AllSpineChartIndicators = 1938132701;
        public const int Suicide_RelatedRiskFactors = 1938132831;
    }

    public class ProfileIds
    {
        public const int Undefined = -1;
        public const int ChildHealth = 2;
        public const int AdultSocialCare = 8;
        public const int Search = 13;
        public const int SubstanceMisuse = 17;
        public const int Tobacco = 18;
        public const int Phof = 19;
        public const int PracticeProfiles = 20;
        public const int LongerLives = 22;
        public const int Unassigned = 23;
        public const int Archived = 24;
        public const int HealthProfiles = 26;
        public const int ChildrenAndYoungPeoplesBenchmarkingTool = 39;
        public const int SexualHealth = 45;
        public const int Diabetes = 51;
        public const int HealthProfilesSupportingIndicators = 52;
        public const int CardioVascularDisease = 53;
        public const int LongerLivesDiabetesSupportingIndicators = 59;
        public const int HealthChecks = 65;
        public const int HyperTension = 67;
        public const int LocalAlcoholProfilesForEngland = 87;
        public const int CancerServices = 92;
        public const int WhatAboutYouth = 94;
        public const int MentalHealthJsna = 98;
        public const int PhysicalActivity = 99;
        public const int Amr = 101;
        public const int ChildHealthProfiles = 105;
        public const int Suicide = 91;
    }

    public class ComparatorIds
    {
        public const int Subnational = 1;
        public const int Target = 2;
        public const int England = 4;
    }

    public class NearestNeighbourTypeIds
    {
        public const int Cipfa = 1;
    }

    public class ComparatorMethodIds
    {
        public const int NoComparison = -1;
        public const int SingleOverlappingCIs = 1;
        public const int SpcForProportions = 5;
        public const int SpcForDsr = 6;
        public const int DoubleOverlappingCIs = 12;
        public const int SuicidePreventionPlan = 14;
        public const int Quintiles = 15;
    }

    public class IndicatorMetadataTextColumnNames
    {
        public const string Name = "Name";
        public const string NameLong = "NameLong";
        public const string DataSource = "DataSource";
        public const string Definition = "Definition";
        public const string Source = "DataSource";
        public const string Notes = "Notes";
        public const string Links = "Links";
        public const string IndicatorNumber = "RefNum";
        public const string IndicatorContent = "IndicatorContent";
    }

    public class BespokeTargets
    {
        public const string ComparedWithPreviousYearEnglandValue = "last-year-england";
        public const string TargetPercentileRange = "nth-percentile-range";
    }

    public class TargetIds
    {
        public const int Undefined = 0;
    }

    public class ContentKeys
    {
        public const string CkanDescription = "ckan-group-description";
        public const string Test = "test-key";
    }

    public class ApplicationEnvironments
    {
        public const string Staging = "staging";
        public const string Live = "live";
    }

    public class ChimatResourceIds
    {
        public const int NotFound = -1;
        public const int Cumbria = 273454;
    }

    public class ChimatWayResourceIds
    {
        public const int NotFound = -1;
        public const int Cumbria = 279208;
    }

    public class SuicidePlanStatus
    {
        public const int Exists = 1;
        public const int InDevelopment = 2;
        public const int None = 3;
    }

    public enum TrendMarker
    {
        CannotBeCalculated = 0, // i.e. not enough data points
        Increasing = 1,
        Decreasing = 2,
        NoChange = 3
    }


}
