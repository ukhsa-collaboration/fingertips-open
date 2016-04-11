using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfDataTest
{
    [TestClass]
    public class SpineChartDataBuilderTest
    {
        private IList<SpineChartTableData> spineChartTableDataList;
        private const string AreaCode1 = AreaCodes.CountyUa_Manchester;
        private const string AreaCode2 = AreaCodes.CountyUa_Cumbria;
        private IList<string> areaCodes = new List<string> { AreaCode1, AreaCode2 };
        private IList<string> benchmarkAreaCodes = new[] { AreaCodes.England, AreaCodes.Gor_EastMidlands };

        [TestMethod]
        public void TestGetResponse_DomainTitlesAreSet()
        {
            foreach (var spineChartTableData in Data())
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(spineChartTableData.DomainTitle));
            }
        }

        [TestMethod]
        public void TestGetResponse_GroupIdsAreSet()
        {
            foreach (var spineChartTableData in Data())
            {
                Assert.AreNotEqual(0, spineChartTableData.GroupId);
            }
        }

        [TestMethod]
        public void TestGetResponse_AreRows()
        {
            foreach (var spineChartTableData in Data())
            {
                Assert.AreNotEqual(0, spineChartTableData.IndicatorData.Count);
            }
        }

        [TestMethod]
        public void TestGetResponse_ShortNameIsSet()
        {
            AssertText("ShortName");
        }

        [TestMethod]
        public void TestGetResponse_LongNameIsSet()
        {
            AssertText("LongName");
        }

        [TestMethod]
        public void TestGetResponse_IndicatorIdIsSet()
        {
            foreach (var spineChartTableData in Data())
            {
                foreach (var row in spineChartTableData.IndicatorData)
                {
                    Assert.AreNotEqual(0, row.IndicatorId);
                }
            }
        }

        [TestMethod]
        public void TestGetResponse_PolarityIdIsSet()
        {
            bool isPolaritySet = false;

            foreach (var spineChartTableData in Data())
            {
                isPolaritySet = spineChartTableData.IndicatorData
                    .Any(x => 
                        x.PolarityId == PolarityIds.UseBlues ||
                        x.PolarityId == PolarityIds.RagLowIsGood ||
                        x.PolarityId == PolarityIds.RagHighIsGood);
            }

            Assert.IsTrue(isPolaritySet);
        }

        [TestMethod]
        public void TestGetResponse_SexIdIsSet()
        {
            bool isSet = false;

            foreach (var spineChartTableData in Data())
            {
                isSet = spineChartTableData.IndicatorData
                    .Any(x => x.SexId == SexIds.Persons);
            }

            Assert.IsTrue(isSet);
        }

        [TestMethod]
        public void TestGetResponse_AgeIdIsSet()
        {
            bool isSet = false;

            foreach (var spineChartTableData in Data())
            {
                isSet = spineChartTableData.IndicatorData
                    .Any(x => x.AgeId == AgeIds.AllAges);
            }

            Assert.IsTrue(isSet);
        }

        [TestMethod]
        public void TestGetResponse_PeriodIsSet()
        {
            AssertText("Period");
        }

        /// <summary>
        /// If this test fails then you should add a definition for the 
        /// indicator number field for the indicator in FPM.
        /// </summary>
        [TestMethod]
        public void TestGetResponse_IndicatorNumberSetForPhof()
        {
            AssertText("IndicatorNumber", 
                IndicatorIds.DeprivationScoreIMD2010, /*in PHOF but does not need number*/
                IndicatorIds.DeprivationScoreIMD2015 /*in PHOF but does not need number*/
                );
        }

        [TestMethod]
        public void TestGetResponse_BenchmarkData()
        {
            bool anyValuesFormatted = false;

            foreach (var spineChartTableData in Data())
            {
                foreach (var row in spineChartTableData.IndicatorData)
                {
                    IDictionary<string, CoreDataSet> benchmarkDataMap = row.BenchmarkData;
                    CheckAllBenchmarkAreaCodesAreIncluded(benchmarkDataMap);

                    foreach (var areaCode in benchmarkDataMap.Keys)
                    {
                        var benchmarkData = benchmarkDataMap[areaCode];
                        if (benchmarkData != null && benchmarkData.IsValueValid && benchmarkData.ValueFormatted != null)
                        {
                            anyValuesFormatted = true;
                            break;
                        }
                    }
                }
                if (anyValuesFormatted) break;
            }

            if (anyValuesFormatted == false)
            {
                Assert.Fail("No formatted benchmark data");
            }
        }

        private void CheckAllBenchmarkAreaCodesAreIncluded(IDictionary<string, CoreDataSet> benchmarkData)
        {
            foreach (var benchmarkAreaCode in benchmarkAreaCodes)
            {
                Assert.IsTrue(benchmarkData.ContainsKey(benchmarkAreaCode),
                    "Expected benchmark code not found " + benchmarkAreaCode);
            }
        }

        [TestMethod]
        public void TestGetResponse_Minimum()
        {
            AssertNotAllValuesAreZero("Min");
        }

        [TestMethod]
        public void TestGetResponse_Maximum()
        {
            AssertNotAllValuesAreZero("Max");
        }

        [TestMethod]
        public void TestGetResponse_P25()
        {
            AssertNotAllValuesAreZero("Percentile25");
        }

        [TestMethod]
        public void TestGetResponse_P75()
        {
            AssertNotAllValuesAreZero("Percentile75");
        }

        [TestMethod]
        public void TestGetResponse_MinimumFormatted()
        {
            AssertAtLeastOneTextValueIsNotNull("MinF");
        }

        [TestMethod]
        public void TestGetResponse_MaximumFormatted()
        {
            AssertAtLeastOneTextValueIsNotNull("MaxF");
        }

        [TestMethod]
        public void TestGetResponse_SexIsAppendedToNames()
        {
            const string sexText = "(Male)";

            bool doAnyShortNamesEndInMale = false;
            bool doAnyLongNamesEndInMale = false;

            foreach (var spineChartTableData in Data())
            {
                foreach (var row in spineChartTableData.IndicatorData)
                {
                    if (row.ShortName.EndsWith(sexText))
                    {
                        doAnyShortNamesEndInMale = true;
                    }

                    if (row.LongName.EndsWith(sexText))
                    {
                        doAnyLongNamesEndInMale = true;
                    }
                }
            }

            if (doAnyShortNamesEndInMale == false)
            {
                Assert.Fail("No short names ended with " + sexText);
            }

            if (doAnyLongNamesEndInMale == false)
            {
                Assert.Fail("No long names ended with " + sexText);
            }
        }

        [TestMethod]
        public void TestGetResponse_ChildAreaDataForCountyUAs()
        {
            bool anyValuesSet = false;

            foreach (var spineChartTableData in Data())
            {
                foreach (var row in spineChartTableData.IndicatorData)
                {
                    Assert.IsTrue(row.AreaData.ContainsKey(AreaCode1), "Child area code missing");
                    Assert.IsTrue(row.AreaData.ContainsKey(AreaCode2), "Child area code missing");

                    var coreDataSet = row.AreaData[AreaCode1];
                    if (coreDataSet != null && coreDataSet.IsValueValid)
                    {
                        anyValuesSet = true;
                        break;
                    }
                }
                if (anyValuesSet) break;
            }

            if (anyValuesSet == false)
            {
                Assert.Fail("All child area values were null");
            }
        }

        [TestMethod]
        public void TestGetResponse_ChildAreaDataIsFormatted()
        {
            bool anyValuesFormatted = false;

            foreach (var spineChartTableData in Data())
            {
                foreach (var row in spineChartTableData.IndicatorData)
                {
                    var coreDataSet = row.AreaData[AreaCode1];
                    if (coreDataSet != null && coreDataSet.IsValueValid && coreDataSet.ValueFormatted != null)
                    {
                        anyValuesFormatted = true;
                        break;
                    }
                }
                if (anyValuesFormatted) break;
            }

            if (anyValuesFormatted == false)
            {
                Assert.Fail("No child area values were formatted");
            }
        }

        [TestMethod]
        public void TestGetResponse_SignificanceIsSet()
        {
            bool anySignificancesSet = false;

            foreach (var spineChartTableData in Data())
            {
                foreach (var row in spineChartTableData.IndicatorData)
                {
                    var significance = row.AreaData[AreaCode1].SignificanceAgainstOneBenchmark;
                    if (significance.HasValue &&
                        (significance == (int)Significance.Same ||
                        significance == (int)Significance.Better ||
                        significance == (int)Significance.Worse))
                    {
                        anySignificancesSet = true;
                        break;
                    }
                }
                if (anySignificancesSet) break;
            }

            if (anySignificancesSet == false)
            {
                Assert.Fail("All significances were none");
            }
        }

        [TestMethod]
        public void TestGetResponse_ChildAreaDataForPracticeAndCcg()
        {
            double countPracticeValues = 0;
            double countCcgValues = 0;

            var practiceCode = AreaCodes.Gp_Sawston;
            var ccgCode = AreaCodes.Ccg_CambridgeshirePeterborough;

            var data = new SpineChartTableDataBuilder().GetDomainDataForProfile(ProfileIds.PracticeProfiles,
                    AreaTypeIds.GpPractice, new List<string> { practiceCode, ccgCode }, new List<string> { AreaCodes.England });

            foreach (var spineChartTableData in data)
            {
                foreach (var row in spineChartTableData.IndicatorData)
                {
                    Assert.IsTrue(row.AreaData.ContainsKey(practiceCode), "Practice code is missing");
                    Assert.IsTrue(row.AreaData.ContainsKey(ccgCode), "CCG code is missing");

                    var practiceData = row.AreaData[practiceCode];
                    if (practiceData != null && practiceData.IsValueValid)
                    {
                        countPracticeValues++;

                        var ccgData = row.AreaData[ccgCode];
                        if (ccgData != null && ccgData.IsValueValid)
                        {
                            countCcgValues++;
                        }
                    }

                }
            }

            if (countPracticeValues == 0)
            {
                Assert.Fail("No practice values");
            }

            // No CCG averages for "Indicators for needs assessment"
            if (countCcgValues / countPracticeValues < 0.8)
            {
                Assert.Fail("Unexpectedly low number of CCG values where practice values are available");
            }
        }

        [TestMethod]
        public void TestGetResponse_AllSignificancesAreSetForPhofHealthProtectionIncludingTargets()
        {
            var healthProtectionDomain = Data()[3];

            // Check title so we know we have the correct domain
            Assert.AreEqual("health protection", healthProtectionDomain.DomainTitle.ToLower());

            int successCounts = 0;
            int failCounts = 0;
            foreach (var row in healthProtectionDomain.IndicatorData)
            {
                var coreDataSet = row.AreaData[AreaCode1];
                var sig = coreDataSet.SignificanceAgainstOneBenchmark;
                if (sig.HasValue == false || sig.Value == (int) Significance.None)
                {
                    failCounts++;
                }
                else
                {
                    successCounts++;
                }
            }

            Assert.IsTrue(successCounts > failCounts);
        }

        [TestMethod]
        public void TestGetResponse_DomainsAreInCorrectOrder()
        {
            var profileId = ProfileIds.SexualHealth;
            spineChartTableDataList = new SpineChartTableDataBuilder().GetDomainDataForProfile(
                profileId, AreaTypeIds.CountyAndUnitaryAuthority, areaCodes, benchmarkAreaCodes);

            var groupIds = ReaderFactory.GetProfileReader().GetProfile(profileId).GroupIds;
            var groupingMetadataList = ReaderFactory.GetGroupDataReader().GetGroupMetadata(groupIds);

            for (var i = 0; i < spineChartTableDataList.Count; i++)
            {
                Assert.AreEqual(spineChartTableDataList[i].GroupId, groupingMetadataList[i].Id);
            }
        }


        private void AssertText(string propertyName, params int[] indicatorIdsToIgnore)
        {
            foreach (var spineChartTableData in Data())
            {
                foreach (var row in spineChartTableData.IndicatorData)
                {
                    var indicatorId = row.IndicatorId;
                    if (indicatorIdsToIgnore.Contains(indicatorId) == false)
                    {
                        var text = (string) row.GetType().GetProperty(propertyName).GetValue(row, null);
                        Assert.IsFalse(string.IsNullOrWhiteSpace(text), "No '" + propertyName + "' for " + indicatorId);
                    }
                }
            }
        }

        private void AssertAtLeastOneTextValueIsNotNull(string propertyName)
        {
            bool anyValuesSet = false;

            foreach (var spineChartTableData in Data())
            {
                if (anyValuesSet == false)
                {
                    foreach (var row in spineChartTableData.IndicatorData)
                    {
                        var text = (string)row.GetType().GetProperty(propertyName).GetValue(row, null);
                        if (string.IsNullOrWhiteSpace(text) == false)
                        {
                            anyValuesSet = true;
                            break;
                        }
                    }
                }
            }

            if (anyValuesSet == false)
            {
                Assert.Fail("All " + propertyName + "s were null");
            }
        }

        private void AssertNotAllValuesAreZero(string propertyName)
        {
            bool anyValuesSet = false;

            foreach (var spineChartTableData in Data())
            {
                if (anyValuesSet == false)
                {
                    foreach (var row in spineChartTableData.IndicatorData)
                    {
                        double? d = (double?)row.GetType().GetProperty(propertyName).GetValue(row, null);

                        if (d.HasValue)
                        {
                            anyValuesSet = true;
                            break;
                        }
                    }
                }
            }

            if (anyValuesSet == false)
            {
                Assert.Fail("All " + propertyName + "s were zero");
            }
        }

        private IList<SpineChartTableData> Data()
        {
            // All tests are read only so only need to create once
            while (spineChartTableDataList == null)
            {
                spineChartTableDataList = new SpineChartTableDataBuilder().GetDomainDataForProfile(
                    ProfileIds.Phof, AreaTypeIds.CountyAndUnitaryAuthority, areaCodes, benchmarkAreaCodes);
            }

            return spineChartTableDataList;
        }
    }
}
