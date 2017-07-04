﻿using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class HealthProfilesData
    {
        public string Population { get; set; }

        public string LifeExpectancyGapMale { get; set; }
        public string LifeExpectancyGapFemale { get; set; }

        public IList<XyPoint> LifeExpectancyMaleByDeprivationDecile { get; set; }
        public IList<XyPoint> LifeExpectancyMaleLineOfBestFit { get; set; }

        // Page 2: life expectancy gap charts at bottom of page
        public IList<XyPoint> LifeExpectancyFemaleByDeprivationDecile { get; set; }
        public IList<XyPoint> LifeExpectancyFemaleLineOfBestFit { get; set; }

        // Page 2: map at top of page
        public Dictionary<string, int> LsoaQuintiles { get; set; }

        // Page 2: column charts at top of page
        public IList<CoreDataSet> DeprivationQuintilesPopulationLocal { get; set; }
        public IList<CoreDataSet> DeprivationQuintilesPopulationEngland { get; set; }

        public IList<int> EarlyDeathYearRange { get; set; }

        public LocalAndEnglandChartDataWithDeprivation EarlyDeathAllCausesMale { get; set; }
        public LocalAndEnglandChartDataWithDeprivation EarlyDeathAllCausesFemale { get; set; }
        public LocalAndEnglandChartData EarlyDeathCvd { get; set; }
        public LocalAndEnglandChartData EarlyDeathCancer { get; set; }

        public CoreDataSet EmergencyAdmissionsLocal { get; set; }
        public CoreDataSet EmergencyAdmissionsEngland { get; set; }

        public Dictionary<string, CoreDataSet> EmergencyAdmissionsLocalByEthnicity { get; set; }
        public Dictionary<string, CoreDataSet> EmergencyAdmissionsEnglandByEthnicity { get; set; }

        public string DependencyRatio { get; set; }

        public string PercentageEthnicMinoritesMale { get; set; }
        public string PercentageEthnicMinoritesFemale { get; set; }
        public string PercentageEthnicMinoritesCombined { get; set; }

        public string PopulationPercentageMale { get; set; }
        public string PopulationPercentageFemale { get; set; }

        public string PopulationCountMale { get; set; }
        public string PopulationCountFemale { get; set; }
        public string PopulationCountCombined { get; set; }

        public string ProjectedPopulationMale { get; set; }
        public string ProjectedPopulationFemale { get; set; }
        public string ProjectedPopulationCombined { get; set; }
    }
}