﻿using System;
﻿using System.Linq;
﻿using System.Text.RegularExpressions;
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfDataTest
{
    [TestClass]
    public class HealthProfilesSupportingInformationBuilderTest
    {
        private static HealthProfilesSupportingInformation _supportingInformation;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
                _supportingInformation = new HealthProfilesSupportingInformationBuilder(
                    AreaCodes.CountyUa_Manchester).Build();
        }

        [TestMethod]
        public void TestBuild_ObjectReturned()
        {
            Assert.IsNotNull(_supportingInformation);
        }
        
        [TestMethod]
        public void TestBuild_PopulationAssigned()
        {
            var population = Data().Population;

            // Number between 1,000 and 10,000,000
            Assert.IsTrue(population.Length > 4 && population.Length < 8);
        }
        
        [TestMethod]
        public void TestLifeExpectancyGapMaleAssigned()
        {
            AssertLifeExpectancyGapIsValid(Data().LifeExpectancyGapMale);
        }

        [TestMethod]
        public void TestLifeExpectancyGapFemaleAssigned()
        {
            AssertLifeExpectancyGapIsValid(Data().LifeExpectancyGapFemale);
        }

        [TestMethod]
        public void TestLifeExpectancyMaleByDecilesAssigned()
        {
            Assert.AreEqual(10, Data().LifeExpectancyMaleByDeprivationDecile.Count);
        }

        [TestMethod]
        public void TestLifeExpectancyFemaleByDecilesAssigned()
        {
            Assert.AreEqual(10, Data().LifeExpectancyFemaleByDeprivationDecile.Count);
        }

        [TestMethod]
        public void TestLifeExpectancyMaleLineOfBestFitAssigned()
        {
            Assert.IsNull(new HealthProfilesData().LifeExpectancyMaleLineOfBestFit, "Should be null by default");

            Assert.IsNotNull(Data().LifeExpectancyMaleLineOfBestFit);
        }

        [TestMethod]
        public void TestLifeExpectancyFemaleLineOfBestFitAssigned()
        {
            Assert.IsNull(new HealthProfilesData().LifeExpectancyFemaleLineOfBestFit, "Should be null by default");

            Assert.IsNotNull(Data().LifeExpectancyFemaleLineOfBestFit);
        }

        [TestMethod]
        public void TestLsoaQuintilesAssigned()
        {
            Assert.IsNotNull(Data().LsoaQuintilesWithinLocalArea);
        }

        [TestMethod]
        public void TestDeprivationQuintilesPopulationAssigned()
        {
            var data = Data();
            Assert.IsTrue(data.DeprivationQuintilesPopulationLocal.Any());
            Assert.IsTrue(data.DeprivationQuintilesPopulationEngland.Any());
        }

        [TestMethod]
        public void TestEmergencyAdmissionsLocalAssigned()
        {
            Assert.IsNotNull(Data().EmergencyAdmissionsLocalByEthnicity);
            Assert.IsNotNull(Data().EmergencyAdmissionsEnglandByEthnicity);
            Assert.IsNotNull(Data().EmergencyAdmissionsLocal);
            Assert.IsNotNull(Data().EmergencyAdmissionsEngland);
        }

        [TestMethod]
        public void TestPopulationPercentageAssigned()
        {
            Assert.IsNotNull(Data().PopulationPercentageFemale);
            Assert.IsNotNull(Data().PopulationPercentageMale);
        }

        [TestMethod]
        public void TestDependencyRatio()
        {
            Assert.IsNotNull(Data().DependencyRatio);
        }

        [TestMethod]
        public void TestPercentageEthnicMinorites()
        {
            Assert.IsNotNull(Data().PercentageEthnicMinoritesCombined);
            Assert.IsNotNull(Data().PercentageEthnicMinoritesMale);
            Assert.IsNotNull(Data().PercentageEthnicMinoritesFemale);
        }

        [TestMethod]
        public void TestPopulations()
        {
            Assert.IsNotNull(Data().PopulationCountFemale);
            Assert.IsNotNull(Data().PopulationCountCombined);
            Assert.IsNotNull(Data().PopulationCountMale);
        }

        [TestMethod]
        public void TestProjectedPopulationChange()
        {
            Assert.IsNotNull(Data().ProjectedPopulationMale);
            Assert.IsNotNull(Data().ProjectedPopulationFemale);
            Assert.IsNotNull(Data().ProjectedPopulationCombined);
        }

        [TestMethod]
        public void TestBuild_PopulationCommarised()
        {
            var population = Data().Population;
            Assert.IsTrue(population.Contains(","));
        }

        [TestMethod]
        public void TestBuild_KeyMessagesAssigned()
        {
            Assert.AreEqual(5, _supportingInformation.HealthProfilesContent.KeyMessages.Count);
        }

        /// <summary>
        /// This test needs to pass for the Health Profiles PDFs to be
        /// generated. It is excluded from Jenkins because it fails often.
        /// </summary>
        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestAllEarlyDeathTrendContainSameNumberOfPoints()
        {
            var data = Data();

            var yearRange = data.EarlyDeathYearRange.Count;

            // All cause female
            Assert.AreEqual(yearRange, data.EarlyDeathAllCausesFemale.England.Count);
            Assert.AreEqual(yearRange, data.EarlyDeathAllCausesFemale.Local.Count);
            Assert.AreEqual(yearRange, data.EarlyDeathAllCausesFemale.LocalLeastDeprived.Count);
            Assert.AreEqual(yearRange, data.EarlyDeathAllCausesFemale.LocalMostDeprived.Count);

            // All cause male
            Assert.AreEqual(yearRange, data.EarlyDeathAllCausesMale.England.Count);
            Assert.AreEqual(yearRange, data.EarlyDeathAllCausesMale.Local.Count);
            Assert.AreEqual(yearRange, data.EarlyDeathAllCausesMale.LocalLeastDeprived.Count);
            Assert.AreEqual(yearRange, data.EarlyDeathAllCausesMale.LocalMostDeprived.Count);

            // Cancer
            Assert.AreEqual(yearRange, data.EarlyDeathCancer.England.Count);
            Assert.AreEqual(yearRange, data.EarlyDeathCancer.Local.Count);

            // CVD
            Assert.AreEqual(yearRange, data.EarlyDeathCvd.England.Count);
            Assert.AreEqual(yearRange, data.EarlyDeathCvd.Local.Count);
        }

        [TestMethod]
        public void TestLifeExpectancyValuesFormattedCorrectlyForLowerTierLAs()
        {
            var data = new HealthProfilesSupportingInformationBuilder(AreaCodes.La_Wychavon).Build();
            var keyMessage2 = data.HealthProfilesContent.KeyMessages[1];
            var regex = new Regex(@"\d{3}");

            Assert.IsFalse(string.IsNullOrEmpty(keyMessage2), "Key message not defined");
            Assert.IsFalse(regex.IsMatch(keyMessage2), keyMessage2);
        }

        [TestMethod]
        public void TestLimitsAreDefinedForCvd()
        {
            ValidateLimitsAndStepForSpecificCauses(Data().EarlyDeathCvd);
        }

        [TestMethod]
        public void TestLimitsAreDefinedForCancer()
        {
            ValidateLimitsAndStepForSpecificCauses(Data().EarlyDeathCancer);
        }

        [TestMethod]
        public void TestLimitsAreDefinedForEarlyDeathAllCausesMale()
        {
            ValidateLimitsAndStepForAllCauses(Data().EarlyDeathAllCausesMale);
        }

        [TestMethod]
        public void TestLimitsAreDefinedForEarlyDeathAllCausesFemale()
        {
            ValidateLimitsAndStepForAllCauses(Data().EarlyDeathAllCausesFemale);
        }

        private HealthProfilesData Data()
        {
            return _supportingInformation.HealthProfilesData;
        }

        private static void ValidateLimitsAndStepForSpecificCauses(LocalAndEnglandChartData data)
        {
            var limits = data.YAxis;
            Assert.IsNotNull(limits);
            Assert.AreEqual(0, limits.Min);
            Assert.AreEqual(250, limits.Max);
            Assert.IsNotNull(limits.Step);
        }

        private void AssertLifeExpectancyGapIsValid(string gap)
        {
            Assert.IsFalse(string.IsNullOrEmpty(gap));
            Assert.IsTrue(gap.Contains("."));

            // e.g. "1.3" or "12.4"
            Assert.IsTrue(gap.Length > 2);
            Assert.IsTrue(gap.Length < 5);
        }

        private static void ValidateLimitsAndStepForAllCauses(LocalAndEnglandChartData data)
        {
            var limits = data.YAxis;
            Assert.IsNotNull(limits);
            Assert.AreEqual(0, limits.Min);
            Assert.IsTrue(limits.Max < 2000);
            Assert.IsNotNull(limits.Step);
        }
    }
}
