'use strict';


/**
* Enum of PHOLIO area type IDs.
* @class AreaTypeIds
*/
var AreaTypeIds = {
    District: 1,
    Region: 6,
    Practice: 7,
    County: 9,
    AcuteTrust: 14,
    Country: 15,
    UnitaryAuthority: 16,
    GpShape: 18,
    DeprivationDecile: 23,
    Subregion: 46,
    DistrictUA: 101,
    CountyUA: 102,
    PheCentres2013: 103,
    PheCentres2015: 104,
    OnsClusterGroup2001: 110,
    OnsClusterGroup2011: 115,
    AcuteTrusts: 118,
    CombinedAuthorities: 126,
    CCGPostApr2017: 152,
    CCGPreApr2017: 153,
    CCGSince2018: 154,
    Uk: 159,
    AreaList: 30000
};

/**
* Enum of PHOLIO category type IDs.
* @class CategoryTypeIds
*/
var CategoryTypeIds = {
    DeprivationDecileCountyUA2015: 39,
    DeprivationDecileDistrictUA2015: 40,
    HealthProfilesSSILimit: 5,
    DeprivationDecileCCG2010: 11,
    DeprivationDecileGp2015: 38,
    SocioeconomicGroup:59
};

/**
* Enum of PHOLIO value type IDs.
* @class ValueTypeIds
*/
var ValueTypeIds = {
    IndirectlyStandardisedRatio: 2,
    IndirectlyStandardisedRate: 4,
    Ratio: 6,
    Count: 7,
    Score: 8,
    LifeExpectancy: 11
};

/**
* Enum of PHOLIO comparator method IDs.
* @class ComparatorMethodIds
*/
var ComparatorMethodIds = {
    SingleOverlappingCIsForOneCiLevel: 1,
    SuicidePlan: 14,
    Quintiles: 15,
    Quartiles: 16,
    SingleOverlappingCIsForTwoCiLevels: 17
};

/**
* Enum of PHOLIO polarity IDs.
* @class PolarityIds
*/
var PolarityIds = {
    NotApplicable: -1,
    RAGLowIsGood: 0,
    RAGHighIsGood: 1,
    BlueOrangeBlue: 99
}

/**
* Enum of PHOLIO sex IDs.
* @class SexIds
*/
var SexIds = {
    Male: 1,
    Female: 2,
    Person: 4
};

/**
* Enum of PHOLIO age IDs.
* @class AgeIds
*/
var AgeIds = {
    AllAges: 1
};


/**
* Enum of PHOLIO profile IDs.
* @class ProfileIds
*/
var ProfileIds = {
    SearchResults: 13,
    Tobacco:18,
    Phof: 19,
    PracticeProfiles: 20,
    Mortality: 22,
    HealthProfiles: 26,
    CommonMentalHealthDisorders: 40,
    SevereMentalIllness: 41,
    CommunityMentalHealth: 50,
    Diabetes: 51,
    Liver: 55,
    Hypertension: 67,
    Cancer: 71,
    Suicide: 73,
    DrugsAndAlcohol: 75,
    HealthChecks: 77,
    Dementia: 84,
    SuicidePrevention: 91,
    ChiMatWAY: 94,
    MentalHealthJsna: 98,
    ChildHealth: 105,
    ChildHealthBehaviours: 129,
    ChildrenYoungPeoplesWellBeing: 133,
    PublicHealthDashboard: 140
};

var TrendMarkerValue = {
    Up: 1,
    Down: 2,
    NoChange: 3,
    CannotCalculate: 4
};

var ValueNoteIds = {
    DataQualityIssue: 401
};

var IndicatorIds = {
    Deprivation: 338,
    SuicidePlan: 92607,
    LarcPrescribed: 92254,
    LivingInAqmas: 93384
}

var GroupIds = {
    PracticeProfiles: {
        Population: 1200006
    },
    HealthChecks: {
        HealthCheck: 1938132782,
        DiseaseAndDeath: 1938132785
    },
    Diabetes: {
        Complications: 1938132699
    },
    DrugsAndAlcohol: {
        PrevalenceAndRisks: 1938132771,
        TreatmentAndRecovery: 1938132772
    },
    Cancer: {
        IncidenceAndMortality: 1938132749
    },
    Suicide: {
        SuicideData: 1938132762
    }
}