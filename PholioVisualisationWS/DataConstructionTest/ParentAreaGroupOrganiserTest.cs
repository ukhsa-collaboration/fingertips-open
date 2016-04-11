using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class ParentAreaGroupOrganiserTest
    {
        [TestMethod]
        public void TestChildAreaTypeIdToParentArea()
        {
            var reader = ReaderFactory.GetAreasReader();
            var organiser = new ParentAreaGroupOrganiser(ParentAreaGroups(), reader);

            var map = organiser.ChildAreaTypeIdToParentArea;
            Assert.AreEqual(2, map.Count);
            Assert.AreEqual(2, map[AreaTypeIds.CountyAndUnitaryAuthority].Count);
            Assert.AreEqual(1, map[AreaTypeIds.Ccg].Count);
        }

        [TestMethod]
        public void TestChildAreaTypeIdToParentAreaOrdersBySequence()
        {
            var reader = ReaderFactory.GetAreasReader();
            var parentAreaGroups = ParentAreaGroupsForSorting();
            var organiser = new ParentAreaGroupOrganiser(parentAreaGroups, reader);

            var map = organiser.ChildAreaTypeIdToParentArea;
            var areaTypes = map[AreaTypeIds.CountyAndUnitaryAuthority];

            var orderedGroups = parentAreaGroups.OrderBy(x => x.Sequence).ToList();
            for (var i = 0; i < areaTypes.Count; i++)
            {
                Assert.AreEqual(orderedGroups[i].ParentAreaTypeId, areaTypes[i].Id);
            }
        }

        [TestMethod]
        public void TestChildAreaTypeIdToParentArea_WithCategoryTypeId()
        {
            var reader = ReaderFactory.GetAreasReader();

            var parentAreaGroups = new List<ParentAreaGroup>
            {
                ParentAreaGroup1(),
                ParentAreaGroupWithCategoryType()
            };

            var organiser = new ParentAreaGroupOrganiser(parentAreaGroups, reader);
            var map = organiser.ChildAreaTypeIdToParentArea;
            Assert.AreEqual(2, map[AreaTypeIds.CountyAndUnitaryAuthority].Count);
        }

        private static List<ParentAreaGroup> ParentAreaGroupsForSorting()
        {
            var parentAreaGroups = new List<ParentAreaGroup>
            {
                new ParentAreaGroup
                {
                    ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                    ParentAreaTypeId = AreaTypeIds.OnsClusterGroup2001,
                    Sequence = 2
                },
                ParentAreaGroup1(),
                ParentAreaGroup3()
            };
            return parentAreaGroups;
        }

        private static ParentAreaGroup ParentAreaGroup1()
        {
            return new ParentAreaGroup
            {
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                ParentAreaTypeId = AreaTypeIds.OnsClusterGroup2001,
                Sequence = 1
            };
        }

        private static ParentAreaGroup ParentAreaGroupWithCategoryType()
        {
            return new ParentAreaGroup
            {
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                CategoryTypeId = CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority,
                Sequence = 2
            };
        }

        private static ParentAreaGroup ParentAreaGroup3()
        {
            return new ParentAreaGroup
            {
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                ParentAreaTypeId = AreaTypeIds.GoRegion,
                Sequence = 3
            };
        }

        private static List<ParentAreaGroup> ParentAreaGroups()
        {
            var parentAreaGroups = new List<ParentAreaGroup>
            {
                new ParentAreaGroup
                {
                    ChildAreaTypeId = AreaTypeIds.Ccg,
                    ParentAreaTypeId = AreaTypeIds.GoRegion,
                },
                ParentAreaGroup1(),
                ParentAreaGroup3()
            };
            return parentAreaGroups;
        }
    }
}
