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
        public void TestGetValueNotes()
        {
            byte[] data = DataControllerEndPointTest.GetData("value_notes");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetTrendMarkers()
        {
            byte[] data = DataControllerEndPointTest.GetData("recent_trends");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetSexes()
        {
            byte[] data = DataControllerEndPointTest.GetData("sexes");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAges()
        {
            byte[] data = DataControllerEndPointTest.GetData("ages");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAge()
        {
            byte[] data = DataControllerEndPointTest.GetData("age?id=" + AgeIds.From0To4);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetCategoryTypes()
        {
            byte[] data = DataControllerEndPointTest.GetData("category_types");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetPolarities()
        {
            byte[] data = DataControllerEndPointTest.GetData("polarities");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetConfidenceIntervalMethods()
        {
            byte[] data = DataControllerEndPointTest.GetData("confidence_interval_methods");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetConfidenceIntervalMethod()
        {
            byte[] data = DataControllerEndPointTest.GetData("confidence_interval_method?id=" + ConfidenceIntervalMethodIds.Byars);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetComparatorMethods()
        {
            byte[] data = DataControllerEndPointTest.GetData("comparator_methods");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetComparatorMethod()
        {
            byte[] data = DataControllerEndPointTest.GetData("comparator_method?id=" +
                ComparatorMethodIds.SingleOverlappingCIs);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetComparators()
        {
            byte[] data = DataControllerEndPointTest.GetData("comparators");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetComparator()
        {
            byte[] data = DataControllerEndPointTest.GetData("comparator?id=" +
                ComparatorIds.Subnational);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetComparatorSignificances()
        {
            byte[] data = DataControllerEndPointTest.GetData("comparator_significances?polarity_id=" +
                PolarityIds.BlueOrangeBlue);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetCategories()
        {
            byte[] data = DataControllerEndPointTest.GetData("categories?" +
                "category_type_id=" + CategoryTypeIds.EthnicGroups5);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetUnits()
        {
            byte[] data = DataControllerEndPointTest.GetData("units");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetYearTypes()
        {
            byte[] data = DataControllerEndPointTest.GetData("year_types");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetYearType()
        {
            byte[] data = DataControllerEndPointTest.GetData("year_type?id=" + YearTypeIds.Academic);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetValueTypes()
        {
            byte[] data = DataControllerEndPointTest.GetData("value_types");
            TestHelper.IsData(data);
        }
    }
}
