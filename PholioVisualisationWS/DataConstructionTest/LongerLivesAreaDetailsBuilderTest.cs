using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class LongerLivesAreaDetailsBuilderTest
    {
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

        [TestMethod]
        public void TestGetAreaDetails_Public_Health_Dashboard_Nearest_Neighbours()
        {
            var areaDetails = new LongerLivesAreaDetailsBuilder()
                .GetAreaDetails(ProfileIds.PublicHealthDashboardLongerLives, 
                GroupIds.PublicHealthDashboardLongerLives_SummaryRank,
                AreaTypeIds.CountyAndUnitaryAuthority, 
                NearestNeighbourArea.CreateAreaCode(NearestNeighbourTypeIds.Cipfa, AreaCodes.CountyUa_Cambridgeshire)
                );

            // Assert: details defined
            Assert.IsNotNull(areaDetails);

            // Assert: area ranks defined
            var areaRankGroupings = areaDetails.Ranks[AreaCodes.England];
            Assert.IsNotNull(areaRankGroupings[0].AreaRank, "AreaRank is not defined");
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
            var areaDetails = LongerLivesAreaDetails(areaCode, AreaTypeIds.CcgsPreApr2017);
            Assert.AreEqual(areaCode, areaDetails.Area.Code);
            Assert.IsNotNull(areaDetails.Decile, "Decile should be defined");
            Assert.IsNotNull(areaDetails.Significances);
            Assert.AreEqual(2, areaDetails.Benchmarks.Count);
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


        private static void CheckBenchmarkDetails(LongerLivesAreaDetails areaDetails)
        {
            Assert.IsNull(areaDetails.Decile);
            Assert.IsNull(areaDetails.Significances);
            Assert.IsNull(areaDetails.Benchmarks);
        }

        private static LongerLivesAreaDetails LongerLivesAreaDetails(string areaCode, int areaTypeId)
        {
            var areaDetails = new LongerLivesAreaDetailsBuilder()
                .GetAreaDetails(ProfileIds.LongerLives, GroupIds.PublicHealthDashboardLongerLives_SummaryRank, areaTypeId, areaCode);
            return areaDetails;
        }
    }
}
