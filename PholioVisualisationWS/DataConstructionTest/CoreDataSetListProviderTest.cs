using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class CoreDataSetListProviderTest
    {
        [TestMethod]
        public void TestWhenAreaIsCountry()
        {
            var reader = Reader();
            reader.Setup(x => x.GetCoreDataForAllAreasOfType(It.IsAny<Grouping>(), It.IsAny<TimePeriod>()))
                .Returns(new List<CoreDataSet>());

            var area = new Area
            {
                AreaTypeId = AreaTypeIds.Country
            };

            GetChildAreaData(reader, area);

            reader.VerifyAll();
        }

        [TestMethod]
        public void TestWhenAreaIsNotCountry()
        {
            string areaCode = "a";

            var reader = Reader();
            reader.Setup(x => x.GetCoreDataListForChildrenOfArea(It.IsAny<Grouping>(), It.IsAny<TimePeriod>(), areaCode))
                .Returns(new List<CoreDataSet>());

            var area = new Area
                {
                    Code = areaCode,
                    AreaTypeId = AreaTypeIds.Sha
                };
            GetChildAreaData(reader, area);

            reader.VerifyAll();
        }

        [TestMethod]
        public void TestWhenAreaIsCategoryArea()
        {
            CategoryArea area = CategoryArea.New(CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority,1);

            var reader = Reader();
            reader.Setup(x => x.GetCoreDataListForChildrenOfCategoryArea(
                    It.IsAny<Grouping>(), It.IsAny<TimePeriod>(), area))
                .Returns(new List<CoreDataSet>());

            GetChildAreaData(reader, area);

            reader.VerifyAll();
        }

        private static Mock<GroupDataReader> Reader()
        {
            var reader = new Mock<GroupDataReader>(MockBehavior.Strict);
            return reader;
        }

        private static void GetChildAreaData(Mock<GroupDataReader> reader, IArea area)
        {
            var data = new CoreDataSetListProvider(reader.Object)
                .GetChildAreaData(new Grouping(),area,new TimePeriod());
        }
    }
}
