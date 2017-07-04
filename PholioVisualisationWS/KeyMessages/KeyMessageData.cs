using System.Collections;
using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessages
{
    public class KeyMessageData
    {
        public IArea Area { get; set; }        
        
        /**
         * Key message 1
         */

        // 1.0
        public SignificanceCounter SpineChartSignificances { get; set; }

        // 1.2
        public CoreDataSet Deprivation { get; set; }
        public CoreDataSet ChildrenInLowIncomeFamilies { get; set; }
        // 1.3
        public Significance MaleLifeExpectancyAtBirth { get; set; }
        public Significance FemaleLifeExpectancyAtBirth { get; set; }

        /**
         * Key message 2
         */

        public bool IsMaleSlopeIndexOfInequalitySignificant { get; set; }
        public bool IsFemaleSlopeIndexOfInequalitySignificant { get; set; }
        public string MaleSlopeIndexOfInequalityForLifeExpectancy { get; set; }
        public string FemaleSlopeIndexOfInequalityForLifeExpectancy { get; set; }

        /**
         * Key message 3
         */
        
        // 3.1
        public Significance ObesityYear6Significance { get; set; }
        public string ObesityYear6Percentage { get; set; }
        public string ObesityYear6Count { get; set; }
        // 3.2
        public double? Under18AlcoholSpecificHospitalStays { get; set; }
        public Significance Under18AlcoholSpecificSignificance { get; set; }
        public string Under18AlcoholSpecificHospitalStaysPerYear { get; set; }
        // 3.3 - 3.4
        public Significance TeenagePregnancySig { get; set; }
        public Significance GcseAchievementSig { get; set; }
        public Significance BreastfeedingInitiationSig { get; set; }
        public Significance SmokingAtTimeOfDeliverySig { get; set; }

        /**
         * Key message 4
         */

        // 4.1
        public string ObesityAdultsYear { get; set; }
        public string ObesityAdultsPercentage { get; set; }
        public string ObesityAdultsCount { get; set; }
        public Significance ObesityAdultsSignificance { get; set; }
        // 4.2
        public double? AdultAlcoholAdmissions { get; set; }
        public double? AdultAlcoholAdmissionsPerYear { get; set; }
        public Significance AdultAlcoholAdmissionsSignificance { get; set; }
        // 4.3
        public string AdultSelfHarmAdmissions { get; set; }
        public string AdultSelfHarmAdmissionsPerYear { get; set; }
        public Significance AdultSelfHarmAdmissionsSignificance { get; set; }
        // 4.4
        public double? AdultSmokingRelatedDeaths { get; set; }
        public double? AdultSmokingRelatedDeathsPerYear { get; set; }
        // 4.5 
        public Significance AdultSmokingRelatedDeathsSignificance { get; set; }
        public Significance AdultExcessWeightSignificance { get; set; }        
        public Significance AdultSmokingPrevalenceSignificance { get; set; }
        public Significance AdultPhysicalActivitySignificance { get; set; }
        // 4.6
        public Significance AdultHipFracturesSignificance { get; set; }
        public Significance AdultSTISignificance { get; set; }
        public Significance AdultKilledAndSeriouslyInjuredOnRoadsSignificance { get; set; }
        public Significance AdultIncidenceOfTBSignificance { get; set; }
        // 4.7
        public Significance AdultStatutoryHomelessnessSig { get; set; }
        public Significance AdultViolentCrimeSig { get; set; }
        public Significance AdultLongTermUnemploymentSig { get; set; }
        public Significance AdultExcessWinterDeathsSig { get; set; }        
        public Significance AdultUnder75MortalityRateCvdSig { get; set; }
        public Significance AdultUnder75MortalityRateCancerSig { get; set; }
    }
}
