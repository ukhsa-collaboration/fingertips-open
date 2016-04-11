using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest
{
    /// <summary>
    /// Tests on the contents of the database.
    /// </summary>
    [TestClass]
    public class PholioTest
    {
        private IAreasReader reader;
        private int areaTypeId;

        /// <summary>
        /// If this test fails compare the areas on the test site with those on the live site
        /// as it may indicate that the AreaMappings are not correct.
        /// </summary>
        [TestMethod]
        public void TestAreaMappingsGorToUpperTierLAs()
        {
            areaTypeId = AreaTypeIds.CountyAndUnitaryAuthority;

            reader = ReaderFactory.GetAreasReader();
            ExpectChildren(AreaCodes.Gor_EastMidlands, 9);
            ExpectChildren(AreaCodes.Gor_EastOfEngland, 11);
            ExpectChildren(AreaCodes.Gor_London, 33);
            ExpectChildren(AreaCodes.Gor_NorthEast, 12);
            ExpectChildren(AreaCodes.Gor_NorthWest, 23);
            ExpectChildren(AreaCodes.Gor_SouthEast, 19);
            ExpectChildren(AreaCodes.Gor_SouthWest, 16);
            ExpectChildren(AreaCodes.Gor_WestMidlands, 14);
            ExpectChildren(AreaCodes.Gor_YorkshireHumber, 15);

        }

        private void ExpectChildren(string parentCode, int count)
        {
            Assert.AreEqual(count, reader.GetChildAreaCount(parentCode, areaTypeId));
        }
    }
}
