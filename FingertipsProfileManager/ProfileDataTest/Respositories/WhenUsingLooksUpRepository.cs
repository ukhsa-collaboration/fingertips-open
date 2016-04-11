using System.Linq;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingLooksUpRepository
    {

        private LookUpsRepository _lookUpsRepository;

        [TestInitialize]
        public void Init()
        {
            _lookUpsRepository = new LookUpsRepository();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _lookUpsRepository.Dispose();
        }

        [TestMethod]
        public void GetKeyColours_Returns_Result()
        {
            var keyColours = _lookUpsRepository.GetKeyColours();
            Assert.IsTrue(keyColours.Any());
        }

        [TestMethod]
        public void GetSkins_Returns_Result()
        {
            var skins = _lookUpsRepository.GetSkins();
            Assert.IsTrue(skins.Any());
        }

        [TestMethod]
        public void GetAreas_Returns_Result()
        {
            var result = _lookUpsRepository.GetAreaTypes();

            Assert.IsTrue(result != null && result.Any());
        }

      
        [TestMethod]
        public void GetSexes_Returns_Result()
        {
            var result = _lookUpsRepository.GetSexes();

            Assert.IsTrue(result != null && result.Any());
        }

        [TestMethod]
        public void GetAges_Returns_Result()
        {
            var result = _lookUpsRepository.GetAges();

            Assert.IsTrue(result != null && result.Any());
        }

        [TestMethod]
        public void GetComparators_Returns_Result()
        {
            var result = _lookUpsRepository.GetComparators();

            Assert.IsTrue(result != null && result.Any());
        }

        [TestMethod]
        public void GetYearTypes_Returns_Result()
        {
            var result = _lookUpsRepository.GetYearTypes();

            Assert.IsTrue(result != null && result.Any());
        }

        [TestMethod]
        public void GetIndicatorValueTypes_Returns_Result()
        {
            var result = _lookUpsRepository.GetIndicatorValueTypes();

            Assert.IsTrue(result != null && result.Any());
        }

        [TestMethod]
        public void GetConfidenceIntervalMethods_Returns_Result()
        {
            var result = _lookUpsRepository.GetConfidenceIntervalMethods();

            Assert.IsTrue(result != null && result.Any());
        }

        [TestMethod]
        public void GetUnits_Returns_Result()
        {
            var result = _lookUpsRepository.GetUnits();

            Assert.IsTrue(result != null && result.Any());
        }
        
        [TestMethod]
        public void GetDenominatorTypes_Returns_Result()
        {
            var result = _lookUpsRepository.GetDenominatorTypes();

            Assert.IsTrue(result != null && result.Any());
        }
        
        [TestMethod]
        public void TestGetAllCategoryTypes()
        {
            var result = _lookUpsRepository.GetCategoryTypes();
            Assert.IsNotNull(result != null && result.Count() >0);
        }

        
    }
}
