﻿using System;
﻿using System.Text.RegularExpressions;
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfDataTest
{
    [TestClass]
    public class HealthProfilesSupportingInformationBuilderTest
    {
        public const string AreaCode = AreaCodes.CountyUa_Manchester;

        [TestMethod]
        public void TestBuild_ObjectReturned()
        {
            Assert.IsNotNull(new HealthProfilesSupportingInformationBuilder(AreaCode).Build());
        }

        [TestMethod]
        public void TestBuild_PopulationAssigned()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            var population = info.HealthProfilesData.Population;

            // Number between 1,000 and 10,000,000
            Assert.IsTrue(population.Length > 4 && population.Length < 8);
        }
        
        [TestMethod]
        public void TestLifeExpectancyGapMaleAssigned()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            AssertLifeExpectancyGapIsValid(info.HealthProfilesData.LifeExpectancyGapMale);
        }

        [TestMethod]
        public void TestLifeExpectancyGapFemaleAssigned()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            AssertLifeExpectancyGapIsValid(info.HealthProfilesData.LifeExpectancyGapFemale);
        }

        public void AssertLifeExpectancyGapIsValid(string gap)
        {
            Assert.IsFalse(string.IsNullOrEmpty(gap));
            Assert.IsTrue(gap.Contains("."));

            // e.g. "1.3" or "12.4"
            Assert.IsTrue(gap.Length > 2);
            Assert.IsTrue(gap.Length < 5);
        }

        [TestMethod]
        public void TestLifeExpectancyMaleByDecilesAssigned()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            Assert.AreEqual(10, info.HealthProfilesData.LifeExpectancyMaleByDeprivationDecile.Count);
        }

        [TestMethod]
        public void TestLifeExpectancyFemaleByDecilesAssigned()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            Assert.AreEqual(10, info.HealthProfilesData.LifeExpectancyFemaleByDeprivationDecile.Count);
        }

        [TestMethod]
        public void TestLifeExpectancyMaleLineOfBestFitAssigned()
        {
            Assert.IsNull(new HealthProfilesData().LifeExpectancyMaleLineOfBestFit, "Should be null by default");

            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            Assert.IsNotNull(info.HealthProfilesData.LifeExpectancyMaleLineOfBestFit);
        }

        [TestMethod]
        public void TestLifeExpectancyFemaleLineOfBestFitAssigned()
        {
            Assert.IsNull(new HealthProfilesData().LifeExpectancyFemaleLineOfBestFit, "Should be null by default");

            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            Assert.IsNotNull(info.HealthProfilesData.LifeExpectancyFemaleLineOfBestFit);
        }

        [TestMethod]
        public void TestLsoaQuintilesAssigned()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            Assert.IsNotNull(info.HealthProfilesData.LsoaQuintiles);
        }

        [TestMethod]
        public void TestDeprivationQuintilesPopulationAssigned()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            Assert.IsNotNull(info.HealthProfilesData.DeprivationQuintilesPopulationLocal);
            Assert.IsNotNull(info.HealthProfilesData.DeprivationQuintilesPopulationEngland);
        }

        [TestMethod]
        public void TestEmergencyAdmissionsLocalAssigned()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            Assert.IsNotNull(info.HealthProfilesData.EmergencyAdmissionsLocalByEthnicity);
            Assert.IsNotNull(info.HealthProfilesData.EmergencyAdmissionsEnglandByEthnicity);
            Assert.IsNotNull(info.HealthProfilesData.EmergencyAdmissionsLocal);
            Assert.IsNotNull(info.HealthProfilesData.EmergencyAdmissionsEngland);
        }

        [TestMethod]
        public void TestBuild_PopulationCommarised()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            var population = info.HealthProfilesData.Population;
            Assert.IsTrue(population.Contains(","));
        }

        [TestMethod]
        public void TestBuild_KeyMessagesAssigned()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            Assert.AreEqual(5, info.HealthProfilesContent.KeyMessages.Count);
        }

        /// <summary>
        /// This test needs to pass for the Health Profiles PDFs to be
        /// generated. It is excluded from Jenkins because it fails often.
        /// </summary>
        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestAllEarlyDeathTrendContainSameNumberOfPoints()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            var data = info.HealthProfilesData;

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
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            ValidateLimitsAndStepForSpecificCauses(info.HealthProfilesData.EarlyDeathCvd);
        }

        [TestMethod]
        public void TestLimitsAreDefinedForCancer()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            ValidateLimitsAndStepForSpecificCauses(info.HealthProfilesData.EarlyDeathCancer);
        }

        [TestMethod]
        public void TestLimitsAreDefinedForEarlyDeathAllCausesMale()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            ValidateLimitsAndStepForAllCauses(info.HealthProfilesData.EarlyDeathAllCausesMale);
        }

        [TestMethod]
        public void TestLimitsAreDefinedForEarlyDeathAllCausesFemale()
        {
            var info = new HealthProfilesSupportingInformationBuilder(AreaCode).Build();
            ValidateLimitsAndStepForAllCauses(info.HealthProfilesData.EarlyDeathAllCausesFemale);
        }

        private static void ValidateLimitsAndStepForSpecificCauses(LocalAndEnglandChartData data)
        {
            var limits = data.YAxis;
            Assert.IsNotNull(limits);
            Assert.AreEqual(0, limits.Min);
            Assert.AreEqual(250, limits.Max);
            Assert.IsNotNull(limits.Step);
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
