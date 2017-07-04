﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class LongerLivesAreaDetailsBuilderTest
    {
        /// <summary>
        /// All indicator significances are quintile based.
        /// </summary>
        [TestMethod]
        public void TestGetAreaDetails_DiabetesPrevalenceAndRiskForEngland()
        {
            var areaCode = AreaCodes.Ccg_Barnet;

            var areaDetails = new LongerLivesAreaDetailsBuilder()
                .GetAreaDetails(ProfileIds.Diabetes, GroupIds.Diabetes_PrevalenceAndRisk, AreaTypeIds.Ccg, areaCode);

            var significances = areaDetails.Significances[AreaCodes.England];
            foreach (var significance in significances)
            {
                Assert.AreNotEqual((int)Significance.None, significance, "Significance should not be none");
            }
        }
        /// <summary>
        /// Suicide Related Risk Factors
        /// </summary>
        [TestMethod]
        public void TestGetAreaDetails_Suicide()
        {
            var areaCode = AreaCodes.England;
            var areaDetails = new LongerLivesAreaDetailsBuilder()
                .GetAreaDetails(ProfileIds.Suicide, GroupIds.Suicide_RelatedRiskFactors, AreaTypeIds.CountyAndUnitaryAuthority, areaCode);

            Assert.IsNotNull(areaDetails);
        }

        //profile_id=51&group_id=1938132700&area_code=cat-2-9&child_area_type_id=102
        [TestMethod]
        public void TestGetAreaDetails_DiabetesPrevalenceAndRiskForEnglandDecile()
        {

            var areaDetails = new LongerLivesAreaDetailsBuilder()
                .GetAreaDetails(ProfileIds.Diabetes, GroupIds.Diabetes_TreatmentTargets,
                AreaTypeIds.CountyAndUnitaryAuthority, "cat-2-9");

            Assert.IsNotNull(areaDetails);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void TestGetAreaDetails_ExceptionThrownForGpPractice()
        {
            var areaCode = AreaCodes.Gp_Addingham;

            var areaDetails = new LongerLivesAreaDetailsBuilder()
                .GetAreaDetails(ProfileIds.LongerLivesDiabetesSupportingIndicators, GroupIds.DiabetesSupportingIndicators_ValuesAtTopOfPage, AreaTypeIds.GpPractice, areaCode);
        }

        [TestMethod]
        public void TestGetAreaDetails_CountyUA_England()
        {
            var areaCode = AreaCodes.England;
            var areaDetails = LongerLivesAreaDetails(areaCode, AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.AreEqual(areaCode, areaDetails.Area.Code);
            CheckBenchmarkDetails(areaDetails);
        }

        [TestMethod]
        public void TestGetAreaDetails_CountyUA_Manchester()
        {
            var areaCode = AreaCodes.CountyUa_Manchester;
            var areaDetails = LongerLivesAreaDetails(areaCode, AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.AreEqual(areaCode, areaDetails.Area.Code);
            Assert.IsNotNull(areaDetails.Decile, "Decile is not defined");
            Assert.IsNotNull(areaDetails.Significances);
            Assert.IsNotNull(areaDetails.Benchmarks);
        }

        /// <summary>
        /// Data may be missing for isles of scilly so useful to test with.
        /// </summary>
        [TestMethod]
        public void TestGetAreaDetails_CountyUA_IslesOfScilly()
        {
            var areaCode = AreaCodes.CountyUa_IslesOfScilly;
            var areaDetails = LongerLivesAreaDetails(areaCode, AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.AreEqual(areaCode, areaDetails.Area.Code);
            Assert.IsNotNull(areaDetails.Decile, "Decile is not defined");
            Assert.IsNotNull(areaDetails.Significances);
            Assert.IsNotNull(areaDetails.Benchmarks);
        }

        [TestMethod]
        public void TestGetAreaDetails_Ccg()
        {
            var areaCode = AreaCodes.Ccg_Barnet;
            var areaDetails = LongerLivesAreaDetails(areaCode, AreaTypeIds.Ccg);
            Assert.AreEqual(areaCode, areaDetails.Area.Code);
            Assert.IsNull(areaDetails.Decile, "Decile should not be defined");
            Assert.IsNotNull(areaDetails.Significances);
            Assert.AreEqual(2, areaDetails.Benchmarks.Count,
                "Both England and ONS clsuter should be available as benchmarks");
        }

        [TestMethod]
        public void TestGetAreaDetails_DistrictUA_SouthCambridgeshire()
        {
            var areaCode = AreaCodes.DistrictUa_SouthCambridgeshire;
            var areaDetails = LongerLivesAreaDetails(areaCode, AreaTypeIds.DistrictAndUnitaryAuthority);
            Assert.AreEqual(areaCode, areaDetails.Area.Code);
            Assert.IsNotNull(areaDetails.Decile, "Decile is not defined");
            Assert.IsNotNull(areaDetails.Significances);
            Assert.IsNotNull(areaDetails.Benchmarks);
        }

        [TestMethod]
        public void TestGetAreaDetails_DistrictUA_Mansfield()
        {
            var areaCode = AreaCodes.DistrictUa_Mansfield;
            var areaDetails = LongerLivesAreaDetails(areaCode, AreaTypeIds.DistrictAndUnitaryAuthority);
            Assert.AreEqual(areaCode, areaDetails.Area.Code);
            Assert.IsNotNull(areaDetails.Decile, "Decile is not defined");
            Assert.IsNotNull(areaDetails.Significances);
            Assert.IsNotNull(areaDetails.Benchmarks);
        }

        [TestMethod]
        public void TestGetAreaDetails_DeprivationDecile()
        {
            var areaCode = CategoryArea.CreateAreaCode(
                CategoryTypeIds.DeprivationDecileCountyAndUA2010, 5);
            var areaDetails = LongerLivesAreaDetails(areaCode, AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.AreEqual(areaCode, areaDetails.Area.Code);
            CheckBenchmarkDetails(areaDetails);
        }

        [TestMethod]
        public void TestGetAreaDetails_DistrictUA_ONSCluster()
        {
            var areaCode = AreaCodes.OnsGroup_ProsperingSouthernEngland;
            var areaDetails = LongerLivesAreaDetails(areaCode, AreaTypeIds.DistrictAndUnitaryAuthority);
            Assert.AreEqual(areaCode, areaDetails.Area.Code);
            Assert.IsNull(areaDetails.Benchmarks);
        }

        [TestMethod]
        public void TestGetAreaDetails_DistrictUA_England()
        {
            var areaCode = AreaCodes.England;
            var areaDetails = LongerLivesAreaDetails(areaCode, AreaTypeIds.DistrictAndUnitaryAuthority);
            Assert.AreEqual(areaCode, areaDetails.Area.Code);
            Assert.IsNull(areaDetails.Benchmarks);
        }

        [TestMethod]
        public void TestGetAreaDetails_HypertensionDetectionForEngland()
        {
            AssertHypertensionDomain(GroupIds.Hypertension_Detection);
        }

        private static void AssertHypertensionDomain(int groupId)
        {
            var areaCode = AreaCodes.England;
            var areaDetails = new LongerLivesAreaDetailsBuilder()
                .GetAreaDetails(ProfileIds.HyperTension, groupId,
                    AreaTypeIds.Ccg, areaCode);

            Assert.AreEqual(areaCode, areaDetails.Area.Code);
        }

        private static void CheckBenchmarkDetails(LongerLivesAreaDetails areaDetails)
        {
            Assert.IsNull(areaDetails.Decile);
            Assert.IsNull(areaDetails.Significances);
            Assert.IsNull(areaDetails.Benchmarks);
        }

        private static LongerLivesAreaDetails LongerLivesAreaDetails(string areaCode, int areaTypeId)
        {
            var areaDetails = new LongerLivesAreaDetailsBuilder()
                .GetAreaDetails(ProfileIds.LongerLives, GroupIds.LongerLives, areaTypeId, areaCode);
            return areaDetails;
        }
    }
}