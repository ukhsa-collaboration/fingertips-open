﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class LimitsBuilderTest
    {
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();

        [TestMethod]
        public void TestLimitsForRange()
        {
            var grouping = PrematureMortalityGrouping();
            var limitsWithComparators = GetLimits(grouping, false);
            var limitsWithoutComparators = GetLimits(grouping, true);

            // Check limits
            Assert.IsTrue(limitsWithComparators.Max > 0);
            Assert.IsTrue(limitsWithComparators.Min > 0);
            Assert.IsTrue(limitsWithComparators.Max > limitsWithComparators.Min);

            // Check limits
            Assert.IsTrue(limitsWithoutComparators.Max > 0);
            Assert.IsTrue(limitsWithoutComparators.Min > 0);
            Assert.IsTrue(limitsWithoutComparators.Max > limitsWithoutComparators.Min);

            // Max including comparators is within same order of magnitude
            Assert.IsFalse(limitsWithComparators.Max > limitsWithoutComparators.Max * 10);
        }

        [TestMethod]
        public void TestLimitsForCountValueType()
        {
            var grouping = CountGrouping();
            var limitsWithComparators = GetLimits(grouping, false);
            var limitsWithoutComparators = GetLimits(grouping, true);

            // Including comparators widens limits
            Assert.IsTrue(limitsWithComparators.Max > limitsWithoutComparators.Max);
            Assert.IsTrue(limitsWithComparators.Min < limitsWithoutComparators.Min);

            // Max including comparators is much bigger
            Assert.IsTrue(limitsWithComparators.Max > limitsWithoutComparators.Max * 10);
        }

        private Limits GetLimits(Grouping grouping,  bool excludeComparators)
        {
            var areaCodes = new List<string>
            {
                AreaCodes.CountyUa_Cambridgeshire,
                AreaCodes.CountyUa_Bedford
            };

            var comparatorMap = new ComparatorMap();
            comparatorMap.Add(new Comparator
            {
                Area = AreaFactory.NewArea(areasReader, AreaCodes.Gor_EastOfEngland),
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                ComparatorId = ComparatorIds.Subnational
            });
            comparatorMap.Add(new Comparator
            {
                Area = AreaFactory.NewArea(areasReader, AreaCodes.England),
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                ComparatorId = ComparatorIds.England
            });

            var limitsBuilder = new LimitsBuilder()
            {
                ExcludeComparators = excludeComparators
            };
            return limitsBuilder.GetLimits(areaCodes, grouping, comparatorMap);
        }

        private Grouping CountGrouping()
        {
            return new Grouping
            {
                AgeId = AgeIds.Over18,
                SexId = SexIds.Persons,
                IndicatorId = IndicatorIds.NumberInTreatmentAtSpecialistDrugMisuseServices,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                DataPointYear = 2014,
                YearRange = 1,
                BaselineYear = 2014
            };
        }

        private Grouping PrematureMortalityGrouping()
        {
            return new Grouping
            {
                AgeId = AgeIds.Under75,
                SexId = SexIds.Persons,
                IndicatorId = IndicatorIds.OverallPrematureDeaths,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                DataPointYear = 2011,
                YearRange = 3,
                BaselineYear = 2011
            };
        }
    }
}
