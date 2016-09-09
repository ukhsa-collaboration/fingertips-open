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
    public class AreaTypesWithParentAreaTypesBuilderTest
    {
        [TestMethod]
        public void TestChildAreaTypeIdToParentArea()
        {
            var areaTypes = GetAreaTypes(ParentAreaGroups());

            AssertParentAreasAreDefined(areaTypes);
        }

        [TestMethod]
        public void TestChildAreaTypeIdToParentAreaOrdersBySequence()
        {
            var parentAreaGroups = ParentAreaGroupsForSorting();
            var areaTypes = GetAreaTypes(parentAreaGroups);
            var parentAreaTypes = areaTypes.First().ParentAreaTypes;

            var orderedGroups = parentAreaGroups.OrderBy(x => x.Sequence).ToList();
            for (var i = 0; i < parentAreaTypes.Count; i++)
            {
                Assert.AreEqual(orderedGroups[i].ParentAreaTypeId, parentAreaTypes[i].Id);
            }
        }

        [TestMethod]
        public void TestChildAreaTypeIdToParentArea_WithCategoryTypeId()
        {
            var parentAreaGroups = new List<ParentAreaGroup>
            {
                ParentAreaGroup1(),
                ParentAreaGroupWithCategoryType()
            };

            var areaTypes = GetAreaTypes(parentAreaGroups);

            AssertParentAreasAreDefined(areaTypes);
        }

        private static IList<AreaType> GetAreaTypes(List<ParentAreaGroup> parentAreaGroups)
        {
            var reader = ReaderFactory.GetAreasReader();
            var builder = new AreaTypesWithParentAreaTypesBuilder(parentAreaGroups, reader);
            var areaTypes = builder.ChildAreaTypesWithParentAreaTypes;
            return areaTypes;
        }

        private static void AssertParentAreasAreDefined(IList<AreaType> areaTypes)
        {
            foreach (var areaType in areaTypes)
            {
                Assert.IsTrue(areaType.ParentAreaTypes.Any());
            }
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
                ParentAreaTypeId = AreaTypeIds.GoRegion,
                Sequence = 1
            };
        }

        private static ParentAreaGroup ParentAreaGroupWithCategoryType()
        {
            return new ParentAreaGroup
            {
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                CategoryTypeId = CategoryTypeIds.DeprivationDecileCountyAndUA2010,
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
