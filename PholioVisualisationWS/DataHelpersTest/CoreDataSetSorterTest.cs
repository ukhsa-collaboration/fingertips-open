using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.DataSorting;

namespace PholioVisualisation.DataSortingTest
{
    [TestClass]
    public class CoreDataSetSorterTest
    {
        [TestMethod]
        public void TestSortByAgeId()
        {
            var unsortedDataList = GetDataWithSpecificAgeIds(2, 3, 1);
            var ages = GetAges(1,2,3);
            var sortedDataList = new CoreDataSetSorter(unsortedDataList).SortByAgeId(ages);

            AssertAgeIdOrder(sortedDataList, 1, 2, 3);
        }

        [TestMethod]
        public void TestSortByAgeIdWhereMultipleDataWithSameAgeId()
        {
            var unsortedDataList = GetDataWithSpecificAgeIds(2, 1, 2);
            var ages = GetAges(1, 2);
            var sortedDataList = new CoreDataSetSorter(unsortedDataList).SortByAgeId(ages);

            AssertAgeIdOrder(sortedDataList, 1, 2, 2);
        }

        [TestMethod]
        public void TestSortBySexId()
        {
            var unsortedDataList = GetDataWithSpecificSexIds(2, 3, 1);
            var sexes = GetSexes(1, 2, 3);
            var sortedDataList = new CoreDataSetSorter(unsortedDataList).SortBySexId(sexes);

            AssertSexIdOrder(sortedDataList, 1, 2, 3);
        }

        [TestMethod]
        public void TestSortByDescendingYear()
        {
            var unsortedDataList = new List<CoreDataSet>
            {
                new CoreDataSet { Year = 2002},
                new CoreDataSet { Year = 2001},
                new CoreDataSet { Year = 2003}
            };
            var sortedDataList = new CoreDataSetSorter(unsortedDataList).SortByDescendingYear().ToList();

            Assert.AreEqual(2003, sortedDataList[0].Year);
            Assert.AreEqual(2001, sortedDataList[2].Year);
        }

        private static List<CoreDataSet> GetDataWithSpecificAgeIds(params int[] ageIds)
        {
            return ageIds.Select(x => new CoreDataSet{AgeId = x}).ToList();
        }

        private static List<Age> GetAges(params int[] ageIds)
        {
            return ageIds.Select(x => new Age { Id = x }).ToList();
        }

        private static List<CoreDataSet> GetDataWithSpecificSexIds(params int[] sexIds)
        {
            return sexIds.Select(x => new CoreDataSet { SexId = x }).ToList();
        }

        private static List<Sex> GetSexes(params int[] sexIds)
        {
            return sexIds.Select(x => new Sex { Id = x }).ToList();
        }

        private static void AssertAgeIdOrder(IList<CoreDataSet> dataList, params int[] ageIds)
        {
            for (int i = 0; i < ageIds.Length; i++)
            {
                Assert.AreEqual(ageIds[i],dataList[i].AgeId);
            }
        }

        private static void AssertSexIdOrder(IList<CoreDataSet> dataList, params int[] sexIds)
        {
            for (int i = 0; i < sexIds.Length; i++)
            {
                Assert.AreEqual(sexIds[i], dataList[i].SexId);
            }
        }
    }
}
