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
    CCG: 19,
    DeprivationDecile: 23,
    Subregion: 46,
    DistrictUA: 101,
    CountyUA: 102,
    PheCentres2013: 103,
    PheCentres2015: 104,
    OnsClusterGroup: 110,
    AcuteTrusts: 118
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
    DeprivationDecileGp2015: 38
};

/**
* Enum of PHOLIO value type IDs.
* @class ValueTypeIds
*/
var ValueTypeIds = {
    IndirectlyStandardisedRate: 4,
    Ratio: 6,
    Count: 7,
    LifeExpectancy: 11
};

/**
* Enum of PHOLIO comparator method IDs.
* @class ComparatorMethodIds
*/
var ComparatorMethodIds = {
    SuicidePlan: 14,
    Quintiles: 15
};

/**
* Enum of PHOLIO polarity IDs.
* @class PolarityIds
*/
var PolarityIds = {
    RAGLowIsGood: 0,
    RAGHighIsGood: 1,
    Quintiles: 15,
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
    Phof: 19,
    PracticeProfiles: 20,
    Mortality: 22,
    HealthProfiles: 26,
    CommunityMentalHealth: 50,
    Diabetes: 51,
    Liver: 55,
    Hypertension: 67,
    Cancer: 71,
    Suicide: 73,
    DrugsAndAlcohol: 75,
    HealthChecks: 77,
    ChiMatWAY: 94,
    ChildHealth: 105,
    ChildHealthBehaviours: 129,
    LAScorecard: 140
};

var TrendMarkerValue = {
    Up: 1,
    Down: 2,
    NoChange: 3,
    CannotCalculate: 4
};