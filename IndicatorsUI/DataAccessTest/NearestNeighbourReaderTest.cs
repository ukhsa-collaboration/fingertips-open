using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DataAccess;
using Profiles.DomainObjects;
using System.Collections.Generic;
using System.Linq;

namespace IndicatorsUI.DataAccessTest
{
    [TestClass]
    public class NearestNeighbourReaderTest
    {
        private NearestNeighbourReader _reader;

        [TestInitialize]
        public void Init()
        {
            _reader = ReaderFactory.GetNearestNeighbourReader();
        }

        [TestMethod]
        public void TestGetProfileNearestNeighbourAreaTypeMappingWhereDefaultUsed()
        {
            var result = _reader.GetProfileNearestNeighbourAreaTypeMapping(ProfileIds.Phof);
            Assert.IsNotNull(result);
            Assert.AreEqual(ProfileIds.Undefined, result[0].ProfileId);
        }

        [TestMethod]
        public void TestGetProfileNearestNeighbourAreaTypeMappingWithDiabetes()
        {
            var result = _reader.GetProfileNearestNeighbourAreaTypeMapping(ProfileIds.ChildHealth);
            var match = result.Any(x => x.ProfileId == ProfileIds.ChildHealth);
            Assert.IsTrue(match);
        }

        [TestMethod]
        public void TestGetNearestNeighbourAreaType()
        {
            var result = _reader.GetNearestNeighbourAreaType(new List<int> { NeighbourTypes.CIPFA }).First();
            Assert.IsNotNull(result);
        }
    }
}
