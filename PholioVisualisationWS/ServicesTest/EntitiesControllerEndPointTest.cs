using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class EntitiesControllerEndPointTest
    {
        [TestMethod]
        public void TestGetNearestNeighbourTypes()
        {
            byte[] data = GetData("nearest_neighbour_types");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetNearestNeighbourType()
        {
            byte[] data = GetData("nearest_neighbour_type?neighbour_type_id=1");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetValueNotes()
        {
            byte[] data = GetData("value_notes");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetTrendMarkers()
        {
            byte[] data = GetData("recent_trends");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetSexes()
        {
            byte[] data = GetData("sexes");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAges()
        {
            byte[] data = GetData("ages");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAge()
        {
            byte[] data = GetData("age?id=" + AgeIds.From0To4);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetCategoryTypes()
        {
            byte[] data = GetData("category_types");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetPolarities()
        {
            byte[] data = GetData("polarities");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetConfidenceIntervalMethods()
        {
            byte[] data = GetData("confidence_interval_methods");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetConfidenceIntervalMethod()
        {
            byte[] data = GetData("confidence_interval_method?id=" + ConfidenceIntervalMethodIds.Byars);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetComparatorMethods()
        {
            byte[] data = GetData("comparator_methods");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetComparatorMethod()
        {
            byte[] data = GetData("comparator_method?id=" +
                ComparatorMethodIds.SingleOverlappingCIsForOneCiLevel);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetComparators()
        {
            byte[] data = GetData("comparators");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetComparator()
        {
            byte[] data = GetData("comparator?id=" +
                ComparatorIds.Subnational);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetComparatorSignificances()
        {
            byte[] data = GetData("comparator_significances?polarity_id=" +
                PolarityIds.BlueOrangeBlue);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetCategories()
        {
            byte[] data = GetData("categories?" +
                "category_type_id=" + CategoryTypeIds.EthnicGroups5);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetUnits()
        {
            byte[] data = GetData("units");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetYearTypes()
        {
            byte[] data = GetData("year_types");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetYearType()
        {
            byte[] data = GetData("year_type?id=" + YearTypeIds.Academic);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetValueTypes()
        {
            byte[] data = GetData("value_types");
            TestHelper.IsData(data);
        }

        public byte[] GetData(string path)
        {
            return DataControllerEndPointTest.GetData(path);
        }
    
    }
}
