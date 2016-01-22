using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class HealthProfilesData
    {
        public string LifeExpectancyGapMale { get; set; }
        public string LifeExpectancyGapFemale { get; set; }

        public IList<XyPoint> LifeExpectancyMaleByDeprivationDecile { get; set; }
        public IList<XyPoint> LifeExpectancyMaleLineOfBestFit { get; set; }

        public IList<XyPoint> LifeExpectancyFemaleByDeprivationDecile { get; set; }
        public IList<XyPoint> LifeExpectancyFemaleLineOfBestFit { get; set; }

        public Dictionary<string, int> LsoaQuintiles { get; set; }

        public string Population { get; set; }

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
   
    }
}