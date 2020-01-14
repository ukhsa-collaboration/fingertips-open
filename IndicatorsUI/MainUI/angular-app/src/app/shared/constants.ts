export class AgeIds {
    public static readonly NotApplicable = -1;
    public static readonly NotAnActualAge = 0;
    public static readonly AllAges = 1;
    public static readonly From5To9 = 4;
    public static readonly From10To14 = 5;
    public static readonly From15To19 = 6;
    public static readonly From20To24 = 7;
    public static readonly From25To29 = 8;
    public static readonly From30To34 = 9;
    public static readonly From35To39 = 10;
    public static readonly From40To44 = 11;
    public static readonly From45To49 = 12;
    public static readonly From50To54 = 13;
    public static readonly From55To59 = 14;
    public static readonly From60To64 = 15;
    public static readonly From65To69 = 16;
    public static readonly From70To74 = 17;
    public static readonly From75To79 = 18;
    public static readonly From80To84 = 19;
    public static readonly Over85 = 20;
    public static readonly From85To89 = 21;
    public static readonly Over90 = 22;
    public static readonly Over65 = 27;
    public static readonly From0To4 = 28;
    public static readonly Is6 = 35;
    public static readonly Is65 = 94;
    public static readonly Under75 = 163;
    public static readonly Over18 = 168;
    public static readonly Under16 = 169;
    public static readonly Weeks6To8 = 170;
    public static readonly From18To64 = 183;
    public static readonly Plus17 = 187;
    public static readonly Plus15 = 188;
    public static readonly From5To15 = 193;
    public static readonly From4To5 = 200;
    public static readonly From10To11 = 201;
    public static readonly From16To64 = 204;
    public static readonly Over2 = 205;
    public static readonly Over60 = 214;
    public static readonly From40To74 = 219;
    public static readonly From18To75 = 234;
    public static readonly From90To94 = 281;
    public static readonly Over95 = 282;
}

export class AreaCodes {
    public static readonly England = 'E92000001';
    public static readonly Uk = 'UK0000000';
}

export class AreaTypeIds {
    public static readonly District = 1;
    public static readonly MSOA = 3;
    public static readonly LSOA = 4;
    public static readonly Region = 6;
    public static readonly Practice = 7;
    public static readonly Ward = 8;
    public static readonly County = 9;
    public static readonly AcuteTrust = 14;
    public static readonly Country = 15;
    public static readonly UnitaryAuthority = 16;
    public static readonly GpShape = 18;
    public static readonly DeprivationDecile = 23;
    public static readonly Subregion = 46;
    public static readonly DistrictUA = 101;
    public static readonly CountyUA = 102;
    public static readonly PheCentres2013 = 103;
    public static readonly PheCentres2015 = 104;
    public static readonly OnsClusterGroup = 110;
    public static readonly Scn = 112;
    public static readonly AcuteTrusts = 118;
    public static readonly Stp = 120;
    public static readonly CombinedAuthorities = 126;
    public static readonly CcgSinceApr2017 = 152;
    public static readonly CcgPreApr2017 = 153;
    public static readonly CcgSinceApr2018 = 154;
    public static readonly Uk = 159;
    public static readonly CcgSinceApr2019 = 165;
    public static readonly CountyUAPostApr2019 = 202;
    public static readonly AreaList = 30000;
};

export class CategoryTypeIds {
    public static readonly Undefined = -1;
    public static readonly DeprivationDecileGp2010 = 1;
    public static readonly DeprivationDecileCountyUA2010 = 2;
    public static readonly DeprivationDecileDistrictUA2010 = 3;
    public static readonly EthnicGroups7 = 4;
    public static readonly HealthProfilesSSILimit = 5;
    public static readonly LsoaDeprivationQuintilesInEngland2010 = 6;
    public static readonly DeprivationDecileCcg2010 = 8;
    public static readonly LsoaDeprivationDecilesWithinArea2010 = 9;
    public static readonly LsoaDeprivationQuintilesWithinArea2010 = 10;
    public static readonly DeprivationDecileCCG2010 = 11;
    public static readonly EthnicGroups5 = 33;
    public static readonly DeprivationDecileGp2015 = 38;
    public static readonly DeprivationDecileCountyUA2015 = 39;
    public static readonly DeprivationDecileDistrictUA2015 = 40;
    public static readonly LsoaDeprivationQuintilesInEngland2015 = 41;
    public static readonly LsoaDeprivationDecilesWithinArea2015 = 48;
    public static readonly LsoaDeprivationQuintilesWithinArea2015 = 50;
    public static readonly SocioeconomicGroup = 59;
}

export class CategoryIds {
    public static readonly Undefined = -1;
    // Deprivation
    public static readonly MostDeprivedDecile = 1;
    public static readonly MostDeprivedQuintile = 1;
    public static readonly LeastDeprivedQuintile = 5;
    // Ethnicity (7 groups)
    public static readonly EthnicityWhite = 1;
    public static readonly EthnicityMixed = 2;
    public static readonly EthnicityAsian = 3;
    public static readonly EthnicityBlack = 4;
    public static readonly EthnicityOther = 5;
}

export class ComparatorIds {
    public static readonly SubNational = 1;
    public static readonly Target = 2;
    public static readonly National = 4;
}

export class ComparatorMethodIds {
    public static readonly SingleOverlappingCIsForOneCiLevel = 1;
    public static readonly SpcForProportions = 5;
    public static readonly SpcForDsr = 6;
    public static readonly SuicidePlan = 14;
    public static readonly Quintiles = 15;
    public static readonly Quartiles = 16;
    public static readonly SingleOverlappingCIsForTwoCiLevels = 17;
}

export class CsvCoreDataType {
    public static readonly National = 0;
    public static readonly Subnational = 1;
    public static readonly Area = 2;
}

export class GroupIds {
    public static readonly Search = 1;
    public static readonly PracticeProfiles_Population = 1200006;
}

/** Highcharts helper */
export class HC {
    public static readonly Credits = { enabled: false };
    public static readonly NoLineMarker = { enabled: false, symbol: 'x' }
}

export class IndicatorIds {
    public static readonly QofListSize = 114;
    public static readonly QofPoints = 295;
    public static readonly QuinaryAgeBands = 337;
    public static readonly LifeExpectancy = 650;
    public static readonly EthnicityEstimates = 1679;
    public static readonly DeprivationScore = 93553;
    public static readonly WouldRecommendPractice = 93438;
}

export class PageType {
    public static readonly None = 0;
    public static readonly Overview = 1;
    public static readonly Map = 2;
    public static readonly Trends = 3;
    public static readonly CompareAreas = 4;
    public static readonly AreaProfiles = 5;
    public static readonly Inequalities = 6;
    public static readonly England = 7;
}

export class ParentDisplay {
    public static readonly NationalAndRegional = 0;
    public static readonly RegionalOnly = 1;
    public static readonly NationalOnly = 2;
};

export class PolarityIds {
    public static readonly NotApplicable = -1;
    public static readonly RAGLowIsGood = 0;
    public static readonly RAGHighIsGood = 1;
    public static readonly BlueOrangeBlue = 99;
}

export class ProfileIds {
    public static readonly SearchResults = 13;
    public static readonly Tobacco = 18;
    public static readonly Phof = 19;
    public static readonly PracticeProfile = 20;
    public static readonly PracticeProfileSupportingIndicators = 21;
    public static readonly HealthProfiles = 26;
    public static readonly HealthProtection = 30;
    public static readonly CommonMentalHealthDisorders = 40;
    public static readonly SevereMentalIllness = 41;
    public static readonly SexualHealth = 45;
    public static readonly Liver = 55;
    public static readonly Dementia = 84;
    public static readonly LAPE = 87;
    public static readonly SuicidePrevention = 91;
    public static readonly MentalHealthJsna = 98;
    public static readonly ChildHealth = 105;
    public static readonly ChildHealthBehaviours = 129;
    public static readonly ChildrenYoungPeoplesWellBeing = 133;
    public static readonly IndicatorsForReview = 153;
}

export class ProfileUrlKeys {
    public static readonly ChildHealthBehaviours = 'child-health-behaviours';
    public static readonly DentalHealth = 'dental-health';
}

export class SexIds {
    public static readonly Male = 1;
    public static readonly Female = 2;
    public static readonly Person = 4;
}

export class Significance {
    public static readonly none = 0;
    public static readonly worse = 1;
    public static readonly same = 2;
    public static readonly better = 3;
    public static readonly worst = 4;
    public static readonly best = 5;
}

export class SortOrder {
    public static readonly LowToHigh = 0;
    public static readonly HighToLow = 1;
}

export class SpineChartMinMaxLabel {
    public static readonly DeriveFromLegendColours = 0;
    public static readonly LowestAndHighest = 1;
    public static readonly WorstAndBest = 2;
    public static readonly WorstLowestAndBestHighest = 3;
}

export class SpineChartMinMaxLabelDescription {
    public static readonly Lowest = 'Lowest';
    public static readonly Highest = 'Highest';
    public static readonly Worst = 'Worst';
    public static readonly Best = 'Best';
    public static readonly WorstLowest = 'Worst/ Lowest';
    public static readonly BestHighest = 'Best/ Highest';
}

export class Tabs {
    public static readonly AreaProfiles = 'area-profiles';
    public static readonly BoxPlots = 'box-plots';
    public static readonly CompareAreas = 'compare-areas';
    public static readonly CompareIndicators = 'compare-indicators';
    public static readonly Definitions = 'definitions';
    public static readonly Download = 'download';
    public static readonly England = 'england';
    public static readonly Inequalities = 'inequalities';
    public static readonly Map = 'map';
    public static readonly Overview = 'overview';
    public static readonly Population = 'population';
    public static readonly Trends = 'trends';
}

export class TrendMarkerValue {
    public static readonly Up = 1;
    public static readonly Down = 2;
    public static readonly NoChange = 3;
    public static readonly CannotCalculate = 4;
};

export class ValueTypeIds {
    public static readonly Proportion = 5;
    public static readonly Count = 7;
}
