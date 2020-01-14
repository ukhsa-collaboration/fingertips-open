
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class RegionToChildAreasHashBuilderTest
    {
        public const string ParentAreaCode = AreaCodes.Gor_EastOfEngland;

        public const int ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019;

        [TestMethod]
        public void TestBuildWithProfile()
        {
            var profile = ReaderFactory.GetProfileReader()
                .GetProfile(ProfileIds.Phof);
            var hash = new RegionToChildAreasHashBuilder()
                .Build(profile, new List<string> {ParentAreaCode}, ChildAreaTypeId);
            CheckDictionary(hash);
        }

        private void CheckDictionary(Dictionary<ParentArea, IList<IArea>> hash)
        {
            // Subnational key present
            Assert.IsTrue(hash.Keys.Any(x => x.AreaCode == ParentAreaCode));

            var childAreas = hash.First(x => x.Key.ChildAreaTypeId == ChildAreaTypeId).Value;

            // Contains cambridge
            Assert.AreEqual(1,childAreas.Count(x => x.Code == AreaCodes.CountyUa_Cambridgeshire));
        }

    }
}
