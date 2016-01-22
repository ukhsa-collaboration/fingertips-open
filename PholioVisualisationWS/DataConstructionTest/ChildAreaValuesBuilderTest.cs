using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class ChildAreaValuesBuilderTest
    {
        [TestMethod]
        public void TestAllCountyAndUAsForEngland()
        {
            var grouping = GroupingForEngland();

            ChildAreaValuesBuilder builder = new ChildAreaValuesBuilder(
                new IndicatorComparerFactory { PholioReader = ReaderFactory.GetPholioReader() },
                ReaderFactory.GetGroupDataReader(),
                ReaderFactory.GetAreasReader(),
                ReaderFactory.GetProfileReader())
            {
                AreaTypeId = grouping.AreaTypeId,
                DataPointOffset = 0,
                ParentAreaCode = AreaCodes.England,
                ComparatorId = grouping.ComparatorId
            };

            BuildAndVerifyData(builder, grouping);
        }

        [TestMethod]
        public void TestTargetComparisonIsMade()
        {
            Grouping grouping = new Grouping
            {
                AgeId = AgeIds.Plus15,
                SexId = SexIds.Persons,
                IndicatorId = IndicatorIds.HIVLateDiagnosis, // an indicator with a target
                GroupId = GroupIds.SexualAndReproductiveHealth,
                ComparatorId = ComparatorIds.England,
                ComparatorMethodId = ComparatorMethodId.SingleOverlappingCIs,
                AreaTypeId = AreaTypeIds.DistrictAndUnitaryAuthority,
                YearRange = 3,
                DataPointYear = 2011
            };

            ChildAreaValuesBuilder builder = new ChildAreaValuesBuilder(
                new IndicatorComparerFactory { PholioReader = ReaderFactory.GetPholioReader() },
                ReaderFactory.GetGroupDataReader(),
                ReaderFactory.GetAreasReader(),
                ReaderFactory.GetProfileReader())
            {
                AreaTypeId = grouping.AreaTypeId,
                DataPointOffset = 0,
                ParentAreaCode = AreaCodes.England,
                ComparatorId = grouping.ComparatorId
            };

            var list = builder.Build(grouping);

            // Target significance is calculated
            Assert.AreNotEqual(Significance.None,
                list.First().Significance[ComparatorIds.Target]);
        }

        private static Grouping GroupingForEngland()
        {
            Grouping grouping = new Grouping
            {
                AgeId = AgeIds.Under75,
                SexId = SexIds.Persons,
                IndicatorId = IndicatorIds.OverallPrematureDeaths,
                GroupId = GroupIds.LongerLives,
                ComparatorId = ComparatorIds.England,
                ComparatorMethodId = (int) ComparatorMethodId.SingleOverlappingCIs,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                YearRange = 3,
                DataPointYear = 2010
            };
            return grouping;
        }

        [TestMethod]
        public void TestAllCountyUAsInDeprivationDecile()
        {
            Grouping grouping = new Grouping
            {
                AgeId = AgeIds.Under75,
                SexId = SexIds.Persons,
                IndicatorId = IndicatorIds.OverallPrematureDeaths,
                GroupId = GroupIds.LongerLives,
                ComparatorId = ComparatorIds.Subnational,
                ComparatorMethodId = ComparatorMethodId.SingleOverlappingCIs,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                YearRange = 3,
                DataPointYear = 2010
            };

            ChildAreaValuesBuilder builder = new ChildAreaValuesBuilder(
                new IndicatorComparerFactory { PholioReader = ReaderFactory.GetPholioReader() },
                ReaderFactory.GetGroupDataReader(),
                ReaderFactory.GetAreasReader(),
                ReaderFactory.GetProfileReader())
            {
                AreaTypeId = grouping.AreaTypeId,
                DataPointOffset = 0,
                ParentAreaCode = CategoryArea.CreateAreaCode(
                    CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority, 1),
                ComparatorId = grouping.ComparatorId
            };

            BuildAndVerifyData(builder, grouping);
        }

        [TestMethod]
        public void TestAllCountyUAsWithQuintilesComparison()
        {
            Grouping grouping = new Grouping
            {
                AgeId = AgeIds.Under75,
                SexId = SexIds.Persons,
                IndicatorId = IndicatorIds.OverallPrematureDeaths,
                GroupId = GroupIds.LongerLives,
                ComparatorId = ComparatorIds.Subnational,
                ComparatorMethodId = ComparatorMethodId.Quintiles,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                YearRange = 3,
                DataPointYear = 2010
            };

            ChildAreaValuesBuilder builder = new ChildAreaValuesBuilder(
                new IndicatorComparerFactory { PholioReader = ReaderFactory.GetPholioReader() },
                ReaderFactory.GetGroupDataReader(),
                ReaderFactory.GetAreasReader(),
                ReaderFactory.GetProfileReader())
            {
                AreaTypeId = grouping.AreaTypeId,
                DataPointOffset = 0,
                ParentAreaCode = AreaCodes.Gor_EastMidlands,
                ComparatorId = grouping.ComparatorId
            };

            var list = builder.Build(grouping);

            Assert.IsTrue(list.Any());

            var categoriesFound = new [] {false,false,false,false,false};
            foreach (var coreDataSet in list)
            {
                var sig = coreDataSet.Significance[grouping.ComparatorId];
                categoriesFound[sig - 1] = true;
                Assert.AreNotEqual(Significance.None,sig);
            }

            // Check at least one of each category was found
            foreach (var b in categoriesFound)
            {
                Assert.IsTrue(b);
            }
        }

        [TestMethod]
        public void TestEmptyListReturnedWhenNoData()
        {
            var grouping = GroupingForEngland();

            grouping.DataPointYear = 1900;

            ChildAreaValuesBuilder builder = new ChildAreaValuesBuilder(
                new IndicatorComparerFactory { PholioReader = ReaderFactory.GetPholioReader() },
                ReaderFactory.GetGroupDataReader(),
                ReaderFactory.GetAreasReader(),
                ReaderFactory.GetProfileReader())
            {
                AreaTypeId = grouping.AreaTypeId,
                DataPointOffset = 0,
                ParentAreaCode = AreaCodes.England,
                ComparatorId = grouping.ComparatorId
            };

            var list = builder.Build(grouping);

            Assert.IsFalse(list.Any());
        }


        private static void BuildAndVerifyData(ChildAreaValuesBuilder builder, Grouping grouping)
        {
            var list = builder.Build(grouping);

            Assert.IsTrue(list.Any());

            // Significance is calculated
            Assert.AreNotEqual(Significance.None,
                list.First().Significance[grouping.ComparatorId]);
        }
    }
}
