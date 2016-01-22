using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class CoreDataSetProviderFactoryTest
    {
        [TestMethod]
        public void NewCategoryAreaCoreDataSetProvider()
        {
            var categoryArea = CategoryArea.New(CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority,1);
            Assert.IsTrue(new CoreDataSetProviderFactory().New(categoryArea) is CategoryAreaCoreDataSetProvider);
        }

        [TestMethod]
        public void NewGpDeprivationDecileCoreDataSetProvider()
        {
            var categoryArea = CategoryArea.New(CategoryTypeIds.DeprivationDecileGp2015, 1);
            Assert.IsTrue(new CoreDataSetProviderFactory().New(categoryArea) is GpDeprivationDecileCoreDataSetProvider);
        }

        [TestMethod]
        public void NewCcgCoreDataSetProvider()
        {
            Assert.IsTrue(Provider(AreaTypeIds.Ccg) is CcgCoreDataSetProvider);
        }

        [TestMethod]
        public void NewSimpleCoreDataSetProviderIsDefault()
        {
            Assert.IsTrue(Provider(AreaTypeIds.County) is SimpleCoreDataSetProvider);
            Assert.IsTrue(Provider(AreaTypeIds.Pct) is SimpleCoreDataSetProvider);
            Assert.IsTrue(Provider(AreaTypeIds.GoRegion) is SimpleCoreDataSetProvider);
        }

        [TestMethod]
        public void NewPracticeAggregateCoreDataSetProviderForShape()
        {
            Assert.IsTrue(Provider(AreaTypeIds.Shape) is ShapeCoreDataSetProvider);
        }

        private static CoreDataSetProvider Provider(int id)
        {
            Area area = new Area { AreaTypeId = id };
            var provider = new CoreDataSetProviderFactory().New(area);
            return provider;
        }
    }
}
