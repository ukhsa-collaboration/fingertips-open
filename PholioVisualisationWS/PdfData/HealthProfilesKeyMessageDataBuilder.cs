using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.KeyMessages;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class HealthProfilesKeyMessageDataBuilder
    {
        private CoreDataSetProvider benchmarkDataProvider;
        private CoreDataSetProvider coreDataSetProvider;
        internal KeyMessageData keyMessageData = new KeyMessageData();
        private PholioReader pholioReader = ReaderFactory.GetPholioReader();
        private HealthProfilesGroupRootSelector groupRootSelector;
        private IndicatorMetadataCollection indicatorMetadataCollection;
        private IArea _area;
        private IndicatorComparerFactory _indicatorComparerFactory;

        public HealthProfilesKeyMessageDataBuilder(IArea area, CoreDataSetProvider coreDataSetProvider,
            CoreDataSetProvider benchmarkDataProvider, IndicatorMetadataCollection indicatorMetadataCollection,
            HealthProfilesGroupRootSelector groupRootSelector)
        {
            _area = area;
            keyMessageData.Area = area;
            this.coreDataSetProvider = coreDataSetProvider;
            this.benchmarkDataProvider = benchmarkDataProvider;
            this.groupRootSelector = groupRootSelector;
            this.indicatorMetadataCollection = indicatorMetadataCollection;

            _indicatorComparerFactory = new IndicatorComparerFactory {PholioReader = pholioReader};
        }

        public KeyMessageData BuildData()
        {
            AssignKeyMessageData();
            return keyMessageData;
        }

        private void AssignKeyMessageData()
        {
            // Key Message 1
            AssignSpineChartSig();
            AssignDeprivationData();
            AssignChildrenInPoverty();
            AssignLifeExpectancyAtBirth();

            // Key Message 2
            AssignDataForKeyMessage2();

            // Key Message 3           
            AssignYear6Obesity();
            AssignUnder18AlcoholSpecificStays();
            AssignTeenagePregnancySig();
            AssignGCSEAchievementSig();
            AssignBreastfeedingSig();
            AssignSmokingAtTimeOfDeliverySig();

            // Key Message 4
            AssignAdultAlcoholAdmissions();
            AssignAdultSelfHarmAdmissions();
            AssignAdultSmokingRelatedDeaths();
            AssignAdultExcessWeightSig();
            AssignAdultSmokingPrevalenceSig();
            AssignAdultPhysicalActivitySig();
            AssignAdultHipFracturesSig();
            AssignAdultSTISig();
            AssignAdultKilledAndSeriouslyInjuredOnRoadsSig();
            AssignAdultIncidenceOfTBSig();

            AssignAdultStatutoryHomelessnessSig();
            AssignAdultViolentCrimeSig();
            AssignAdultLongTermUnemploymentSig();
            AssignAdultExcessWinterDeathsSig();
            AssignAdultUnder75MortalityRateCvdSig();
            AssignAdultUnder75MortalityRateCancerSig();
        }

        private CoreDataSet GetCoreDataSet(Grouping grouping, out Significance significance)
        {
            IndicatorMetadata metadata = indicatorMetadataCollection.GetIndicatorMetadataById(grouping.IndicatorId);
            CoreDataSet coreDataSet = coreDataSetProvider.GetData(grouping, TimePeriod.GetDataPoint(grouping), metadata);
            CoreDataSet benchmarkData = benchmarkDataProvider.GetData(grouping, TimePeriod.GetDataPoint(grouping),
                metadata);
            significance = _indicatorComparerFactory.New(grouping).Compare(coreDataSet, benchmarkData, metadata);

            return coreDataSet;
        }

        private Significance GetSignificance(Grouping grouping)
        {
            Significance significance;
            GetCoreDataSet(grouping, out significance);
            return significance;
        }

        #region Key Message 1

        #region 1.1

        private void AssignSpineChartSig()
        {
            var roots = groupRootSelector.MainGroupRoots;
            var significances = roots.Select(groupRoot => GetSignificance(groupRoot.FirstGrouping)).ToList();
            keyMessageData.SpineChartSignificances = new SignificanceCounter(significances);
        }

        #endregion

        #region 1.2

        /// <summary>
        /// Assign significance 
        /// </summary>
        internal void AssignDeprivationData()
        {
            // Define parameters
            Grouping grouping = groupRootSelector.DeprivationScoreIMD2015.FirstGrouping;
            var timePeriod = TimePeriod.GetDataPoint(grouping);

            // Get data
            var groupDataReader = ReaderFactory.GetGroupDataReader();
            CoreDataSet coreDataSet = groupDataReader.GetCoreData(grouping, timePeriod, _area.Code)
                .FirstOrDefault();
            keyMessageData.Deprivation = coreDataSet;
        }

        internal void AssignChildrenInPoverty()
        {
            Grouping grouping = groupRootSelector.ChildrenInLowIncomeFamilies.FirstGrouping;
            IndicatorMetadata metadata = indicatorMetadataCollection.GetIndicatorMetadataById(grouping.IndicatorId);
            CoreDataSet coreDataSet = coreDataSetProvider.GetData(grouping, TimePeriod.GetDataPoint(grouping), metadata);

            keyMessageData.ChildrenInLowIncomeFamilies = coreDataSet;
        }

        #endregion

        #region 1.3

        private void AssignLifeExpectancyAtBirth()
        {
            keyMessageData.FemaleLifeExpectancyAtBirth =
                GetSignificance(groupRootSelector.LifeExpectancyAtBirthFemale.FirstGrouping);
            keyMessageData.MaleLifeExpectancyAtBirth =
                GetSignificance(groupRootSelector.LifeExpectancyAtBirthMale.FirstGrouping);
        }

        #endregion

        #endregion

        #region Key Message 2

        private void AssignDataForKeyMessage2()
        {
            Grouping maleGrouping = groupRootSelector.SlopeIndexOfInequalityForLifeExpectancyMale.FirstGrouping;
            Grouping femaleGrouping = groupRootSelector.SlopeIndexOfInequalityForLifeExpectancyFemale.FirstGrouping;

            IndicatorMetadata metadata =
                indicatorMetadataCollection.GetIndicatorMetadataById(maleGrouping.IndicatorId);

            // Female
            CoreDataSet data = coreDataSetProvider.GetData(femaleGrouping, TimePeriod.GetDataPoint(femaleGrouping), metadata);
            if (data != null && data.IsValueValid)
            {
                keyMessageData.IsFemaleSlopeIndexOfInequalitySignificant =
                    IsSlopeIndexOfInequalityForLifeExpectancySignificant(data);
                keyMessageData.FemaleSlopeIndexOfInequalityForLifeExpectancy =
                    NumericFormatter.Format1DP(data.Value);
            }

            // Male
            data = coreDataSetProvider.GetData(maleGrouping, TimePeriod.GetDataPoint(maleGrouping), metadata);
            if (data != null && data.IsValueValid)
            {
                keyMessageData.IsMaleSlopeIndexOfInequalitySignificant =
                    IsSlopeIndexOfInequalityForLifeExpectancySignificant(data);
                keyMessageData.MaleSlopeIndexOfInequalityForLifeExpectancy =
                    NumericFormatter.Format1DP(data.Value);
            }

        }

        private bool IsSlopeIndexOfInequalityForLifeExpectancySignificant(CoreDataSet data)
        {
            return data.LowerCI > 0 || data.UpperCI < 0;
        }

        #endregion

        #region Key Message 3

        #region 3.1

        private void AssignYear6Obesity()
        {
            Grouping grouping = groupRootSelector.ObesityYear6.FirstGrouping;
            Significance significance;
            CoreDataSet coreDataSet = GetCoreDataSet(grouping, out significance);

            keyMessageData.ObesityYear6Significance = significance;

            if (coreDataSet != null)
            {
                if (coreDataSet.IsValueValid)
                {
                    keyMessageData.ObesityYear6Percentage = NumericFormatter.Format1DP(coreDataSet.Value);
                }

                if (coreDataSet.IsCountValid)
                {
                    keyMessageData.ObesityYear6Count = NumberCommariser.Commarise0DP(coreDataSet.CountPerYear);
                }
            }
        }

        #endregion

        #region 3.2

        private void AssignUnder18AlcoholSpecificStays()
        {
            Grouping grouping = groupRootSelector.Under18AlcoholSpecificStays.FirstGrouping;
            Significance significance;
            CoreDataSet coreDataSet = GetCoreDataSet(grouping, out significance);

            keyMessageData.Under18AlcoholSpecificSignificance = significance;

            if (coreDataSet != null)
            {
                if (coreDataSet.IsValueValid)
                {
                    keyMessageData.Under18AlcoholSpecificHospitalStays = coreDataSet.Value;
                }

                if (coreDataSet.IsCountValid)
                {
                    keyMessageData.Under18AlcoholSpecificHospitalStaysPerYear =
                        NumberCommariser.Commarise0DP(coreDataSet.CountPerYear);
                }
            }
        }

        #endregion

        #region 3.3 & 3.4

        private void AssignTeenagePregnancySig()
        {
            Grouping grouping = groupRootSelector.TeenagePregnancy.FirstGrouping;
            Significance significance;
            GetCoreDataSet(grouping, out significance);
            keyMessageData.TeenagePregnancySig = significance;
        }

        private void AssignGCSEAchievementSig()
        {
            Grouping grouping = groupRootSelector.GCSEAchievement.FirstGrouping;
            Significance significance;
            GetCoreDataSet(grouping, out significance);
            keyMessageData.GcseAchievementSig = significance;
        }

        private void AssignBreastfeedingSig()
        {
            Grouping grouping = groupRootSelector.BreastfeedingInitiation.FirstGrouping;
            Significance significance;
            GetCoreDataSet(grouping, out significance);
            keyMessageData.BreastfeedingInitiationSig = significance;
        }

        private void AssignSmokingAtTimeOfDeliverySig()
        {
            Grouping grouping = groupRootSelector.SmokingAtTimeOfDelivery.FirstGrouping;
            Significance significance;
            GetCoreDataSet(grouping, out significance);
            keyMessageData.SmokingAtTimeOfDeliverySig = significance;
        }

        #endregion

        #endregion

        #region Key Message 4

        #region 4.2

        private void AssignAdultAlcoholAdmissions()
        {
            Grouping grouping = groupRootSelector.AdultAlcoholAdmissions.FirstGrouping;
            Significance significance;
            CoreDataSet coreDataSet = GetCoreDataSet(grouping, out significance);

            keyMessageData.AdultAlcoholAdmissionsSignificance = significance;

            if (coreDataSet != null)
            {
                if (coreDataSet.IsValueValid)
                {
                    keyMessageData.AdultAlcoholAdmissions = coreDataSet.Value;
                }

                if (coreDataSet.IsCountValid)
                {
                    keyMessageData.AdultAlcoholAdmissionsPerYear = coreDataSet.CountPerYear;
                }
            }
        }

        #endregion

        #region 4.3

        private void AssignAdultSelfHarmAdmissions()
        {
            Grouping grouping = groupRootSelector.AdultSelfHarmAdmissions.FirstGrouping;
            Significance significance;
            CoreDataSet coreDataSet = GetCoreDataSet(grouping, out significance);

            keyMessageData.AdultSelfHarmAdmissionsSignificance = significance;

            if (coreDataSet != null)
            {
                if (coreDataSet.IsValueValid)
                {
                    keyMessageData.AdultSelfHarmAdmissions = NumberCommariser.Commarise0DP(coreDataSet.Value);
                }

                if (coreDataSet.IsCountValid)
                {
                    keyMessageData.AdultSelfHarmAdmissionsPerYear =
                        NumberCommariser.Commarise0DP(coreDataSet.CountPerYear);
                }
            }
        }

        #endregion

        #region 4.4

        private void AssignAdultSmokingRelatedDeaths()
        {
            Grouping grouping = groupRootSelector.AdultSmokingRelatedDeaths.FirstGrouping;
            Significance significance;
            CoreDataSet coreDataSet = GetCoreDataSet(grouping, out significance);

            keyMessageData.AdultSmokingRelatedDeathsSignificance = significance;

            if (coreDataSet != null)
            {
                if (coreDataSet.IsValueValid)
                {
                    keyMessageData.AdultSmokingRelatedDeaths = coreDataSet.Value;
                }

                if (coreDataSet.IsCountValid)
                {
                    keyMessageData.AdultSmokingRelatedDeathsPerYear = coreDataSet.CountPerYear;
                }
            }
        }

        #endregion

        #region 4.5

        private void AssignAdultExcessWeightSig()
        {
            Grouping grouping = groupRootSelector.AdultExcessWeight.FirstGrouping;
            Significance significance;
            GetCoreDataSet(grouping, out significance);
            keyMessageData.AdultExcessWeightSignificance = significance;
        }

        private void AssignAdultSmokingPrevalenceSig()
        {
            Grouping grouping = groupRootSelector.AdultSmokingPrevalence.FirstGrouping;
            Significance significance;
            GetCoreDataSet(grouping, out significance);
            keyMessageData.AdultSmokingPrevalenceSignificance = significance;
        }

        private void AssignAdultPhysicalActivitySig()
        {
            Grouping grouping = groupRootSelector.AdultPhysicalActivity.FirstGrouping;
            Significance significance;
            GetCoreDataSet(grouping, out significance);
            keyMessageData.AdultPhysicalActivitySignificance = significance;
        }

        #endregion

        #region 4.6

        private void AssignAdultHipFracturesSig()
        {
            Grouping grouping = groupRootSelector.AdultHipFractures.FirstGrouping;
            Significance significance;
            GetCoreDataSet(grouping, out significance);
            keyMessageData.AdultHipFracturesSignificance = significance;
        }

        private void AssignAdultSTISig()
        {
            Grouping grouping = groupRootSelector.AdultSTI.FirstGrouping;
            Significance significance;
            GetCoreDataSet(grouping, out significance);
            keyMessageData.AdultSTISignificance = significance;
        }

        private void AssignAdultKilledAndSeriouslyInjuredOnRoadsSig()
        {
            Grouping grouping = groupRootSelector.AdultKilledAndSeriouslyInjuredOnRoads.FirstGrouping;
            Significance significance;
            GetCoreDataSet(grouping, out significance);
            keyMessageData.AdultKilledAndSeriouslyInjuredOnRoadsSignificance = significance;
        }

        private void AssignAdultIncidenceOfTBSig()
        {
            Grouping grouping = groupRootSelector.AdultIncidenceOfTB.FirstGrouping;
            Significance significance;
            GetCoreDataSet(grouping, out significance);
            keyMessageData.AdultIncidenceOfTBSignificance = significance;
        }

        #endregion

        #region 4.7

        public void AssignAdultStatutoryHomelessnessSig()
        {
            keyMessageData.AdultStatutoryHomelessnessSig =
                GetSignificanceOnly(groupRootSelector.AdultStatutoryHomelessness);
        }

        public void AssignAdultViolentCrimeSig()
        {
            keyMessageData.AdultViolentCrimeSig = GetSignificanceOnly(groupRootSelector.AdultViolentCrime);
        }

        public void AssignAdultLongTermUnemploymentSig()
        {
            keyMessageData.AdultLongTermUnemploymentSig =
                GetSignificanceOnly(groupRootSelector.AdultLongTermUnemployment);
        }

        public void AssignAdultExcessWinterDeathsSig()
        {
            keyMessageData.AdultExcessWinterDeathsSig = GetSignificanceOnly(groupRootSelector.AdultExcessWinterDeaths);
        }

        public void AssignAdultUnder75MortalityRateCvdSig()
        {
            keyMessageData.AdultUnder75MortalityRateCvdSig =
                GetSignificanceOnly(groupRootSelector.AdultUnder75MortalityRateCvd);
        }

        public void AssignAdultUnder75MortalityRateCancerSig()
        {
            keyMessageData.AdultUnder75MortalityRateCancerSig =
                GetSignificanceOnly(groupRootSelector.AdultUnder75MortalityRateCancer);
        }

        private Significance GetSignificanceOnly(GroupRoot groupRoot)
        {
            Grouping grouping = groupRoot.GetNationalGrouping();
            Significance significance;
            GetCoreDataSet(grouping, out significance);
            return significance;
        }

        #endregion

        #endregion
    }
}