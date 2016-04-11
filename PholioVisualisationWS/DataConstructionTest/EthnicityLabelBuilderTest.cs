
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class EthnicityLabelBuilderTest
    {
        [TestMethod]
        public void TestLabelWithOneEthnicity()
        {
            var dataList = GetCategoryData();
            SetValueForCategory(dataList, CategoryIds.EthnicityMixed, 30);
            Contains(new EthnicityLabelBuilder(dataList, GetCategories()), "30.0% mixed");

            dataList = GetCategoryData();
            SetValueForCategory(dataList, CategoryIds.EthnicityBlack, 2.2);
            Contains(new EthnicityLabelBuilder(dataList, GetCategories()), "2.2% black");

            dataList = GetCategoryData();
            SetValueForCategory(dataList, CategoryIds.EthnicityAsian, 10);
            Contains(new EthnicityLabelBuilder(dataList, GetCategories()), "10.0% asian");
            
        }

        [TestMethod]
        public void TestLabelWithOtherNonWhiteEthnicGroups()
        {
            var dataList = GetCategoryData();
            SetValueForCategory(dataList, CategoryIds.EthnicityMixed, 30);
            SetValueForCategory(dataList, CategoryIds.EthnicityBlack, 0.8);
            SetValueForCategory(dataList, CategoryIds.EthnicityWhite, 68.4);

            Assert.AreEqual("30.0% mixed, 1.6% other non-white ethnic groups",
                new EthnicityLabelBuilder(dataList, GetCategories()).Label);
        }

        [TestMethod]
        public void TestLabelWithVerySmallValue()
        {
            var dataList = GetCategoryData();
            SetValueForCategory(dataList, CategoryIds.EthnicityMixed, 30);
            SetValueForCategory(dataList, CategoryIds.EthnicityBlack, 0.04);
            SetValueForCategory(dataList, CategoryIds.EthnicityWhite, 69.96);

            // Do not want to see "0.0% other non-white ethnic groups"
            Assert.AreEqual("30.0% mixed", 
                new EthnicityLabelBuilder(dataList, GetCategories()).Label);
        }

        [TestMethod]
        public void TestLabelWithTwoEthnicities()
        {
            var dataList = GetCategoryData();
            SetValueForCategory(dataList, CategoryIds.EthnicityMixed, 30);
            SetValueForCategory(dataList, CategoryIds.EthnicityBlack, 20);
            SetValueForCategory(dataList, CategoryIds.EthnicityWhite, 50);
            Contains(new EthnicityLabelBuilder(dataList, GetCategories()), "30.0% mixed, 20.0% black");
        }

        private static void Contains(EthnicityLabelBuilder builder, string expected)
        {
            Assert.IsTrue(builder.Label.Contains(expected));
        }

        public void SetValueForCategory(IList<CoreDataSet> dataList, int categoryId, double val)
        {
            dataList.First(x => x.CategoryId == categoryId).Value = val;
        }

        public IList<CoreDataSet> GetCategoryData()
        {
            var categories = GetCategories();
            var dataList = new List<CoreDataSet>();
            foreach (var category in categories)
            {
                dataList.Add(
                    new CoreDataSet { CategoryId = category.CategoryId}
                    );
            }
            return dataList;
        }

        public IList<Category> GetCategories()
        {
            return ReaderFactory.GetAreasReader().GetCategories(
                EthnicityLabelBuilder.EthnicityCategoryTypeId);
        }
    }
}
