using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class ParentChildAreaRelationshipBuilderTest
    {
        private string parentAreaCode = "Parent";
        private int childAreaId = AreaTypeIds.Pct;

        private List<IArea> filteredAreas;
        private List<IArea> unfilteredAreas;

        [TestInitialize]
        public void RunBeforeEachTest()
        {
            filteredAreas = new List<IArea> {
                new Area { Code = "z" }, 
                new Area { Code = "y" } 
            };
            unfilteredAreas = new List<IArea>(filteredAreas);
            unfilteredAreas.Add(new Area { Code = "a" });
        }

        [TestMethod]
        public void TestBuild()
        {
            var areaListBuilder = new Mock<AreaListBuilder>(MockBehavior.Strict);
            areaListBuilder.Setup(x => x.CreateChildAreaList(parentAreaCode, childAreaId));
            areaListBuilder.Setup(x => x.SortByOrderOrName());
            areaListBuilder.SetupGet(x => x.Areas)
                .Returns(unfilteredAreas);

            var ignoredAreasFilter = new Mock<IgnoredAreasFilter>(MockBehavior.Strict);

            // Get areas
            ParentAreaWithChildAreas relationship =
                new ParentChildAreaRelationshipBuilder(ignoredAreasFilter.Object, areaListBuilder.Object)
                    .GetParentAreaWithChildAreas(new Area { Code = parentAreaCode }, childAreaId, true);

            // Verify
            Assert.AreEqual(3, relationship.Children.Count());
            Assert.AreEqual(parentAreaCode, relationship.Parent.Code);
            areaListBuilder.VerifyAll();
        }

        [TestMethod]
        public void TestBuild_IgnoredAreasMayBeRemoved()
        {
            var areaListBuilder = new Mock<AreaListBuilder>();
            areaListBuilder.SetupGet(x => x.Areas)
                .Returns(filteredAreas);

            var ignoredAreasFilter = new Mock<IgnoredAreasFilter>(MockBehavior.Strict);
            ignoredAreasFilter.Setup(x => x.RemoveAreasIgnoredEverywhere(unfilteredAreas));

            // Get areas
            ParentAreaWithChildAreas relationship =
                new ParentChildAreaRelationshipBuilder(ignoredAreasFilter.Object, areaListBuilder.Object)
                    .GetParentAreaWithChildAreas(new Area { Code = parentAreaCode }, childAreaId, false);

            // Verify
            Assert.AreEqual(2, relationship.Children.Count());
            areaListBuilder.VerifyAll();
        }
    }
}