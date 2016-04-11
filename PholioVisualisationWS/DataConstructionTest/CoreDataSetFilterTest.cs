using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class CoreDataSetFilterTest
    {

        [TestMethod]
        public void TestSelectWhereValueIsValid()
        {
            var data = new[] { 
                new CoreDataSet{Value = 1},
                new CoreDataSet{Value = 1}
            };
            var filteredData = new CoreDataSetFilter(data).SelectWhereValueIsValid();

            Assert.AreEqual(2, filteredData.Count());
        }

        [TestMethod]
        public void TestSelectWhereValueIsValid_InvalidValueRemoved()
        {
            var validValue = 1;
            
            var data = new[] { 
                new CoreDataSet{Value = validValue},
                new CoreDataSet{Value = ValueData.NullValue}
            };
            var filteredData = new CoreDataSetFilter(data).SelectWhereValueIsValid();

            Assert.AreEqual(1, filteredData.Count());
            Assert.AreEqual(validValue, filteredData.First().Value);
        }

        [TestMethod]
        public void TestSelectDistinctSexIds()
        {
            var data = new[] { 
                new CoreDataSet{SexId = 2},
                new CoreDataSet{SexId = 2}
            };
            var ids = new CoreDataSetFilter(data).SelectDistinctSexIds();

            // Check IDs
            Assert.AreEqual(1, ids.Count());
            Assert.AreEqual(2, ids.First());
        }

        [TestMethod]
        public void TestSelectDistinctAgeIds()
        {
            var data = new[] { 
                new CoreDataSet{AgeId = 2},
                new CoreDataSet{AgeId = 2}
            };
            var ids = new CoreDataSetFilter(data).SelectDistinctAgeIds();

            // Check IDs
            Assert.AreEqual(1, ids.Count());
            Assert.AreEqual(2, ids.First());
        }

        [TestMethod]
        public void TestSelectWhereCountIsValid()
        {
            var data = new[] { 
                new CoreDataSet{Count = 1},
                new CoreDataSet{Count = 1}
            };
            var filteredData = new CoreDataSetFilter(data).SelectWhereCountIsValid();

            Assert.AreEqual(2, filteredData.Count());
        }

        [TestMethod]
        public void TestSelectWhereCountIsValid_InvalidCountRemoved()
        {
            var data = new[] { 
                new CoreDataSet{Count = null},
                 new CoreDataSet{Count = -1},
                new CoreDataSet{Count = 1}
            };
            var filteredData = new CoreDataSetFilter(data).SelectWhereCountIsValid();

            Assert.AreEqual(1, filteredData.Count());
            Assert.AreEqual(1, filteredData.First().Count);
        }

        [TestMethod]
        public void TestSelectWhereCountAndDenominatorAreValid()
        {
            var data = new[] { 
                new CoreDataSet{Count = 1,Denominator = 2},
                new CoreDataSet{Count = 1,Denominator = 2}
            };
            var filteredData = new CoreDataSetFilter(data).SelectWhereCountAndDenominatorAreValid();

            Assert.AreEqual(2, filteredData.Count());
        }

        [TestMethod]
        public void TestSelectWhereCountAndDenominatorAreValid_InvalidCountRemoved()
        {
            var data = new[] { 
                new CoreDataSet{Count = null,Denominator = 2},
                 new CoreDataSet{Count = -1, Denominator = 2},
                new CoreDataSet{Count = 1,Denominator = 2}
            };
            var filteredData = new CoreDataSetFilter(data).SelectWhereCountAndDenominatorAreValid();

            Assert.AreEqual(1, filteredData.Count());
            Assert.AreEqual(1, filteredData.First().Count);
        }

        [TestMethod]
        public void TestSelectWhereCountAndDenominatorAreValid_InvalidDenominatorRemoved()
        {
            var data = new[] { 
                new CoreDataSet{Count = 1,Denominator = -1},
                 new CoreDataSet{Count = 1, Denominator = 2},
            };
            var filteredData = new CoreDataSetFilter(data).SelectWhereCountAndDenominatorAreValid();

            Assert.AreEqual(1, filteredData.Count());
            Assert.AreEqual(2, filteredData.First().Denominator);
        }

        [TestMethod]
        public void TestSelectWithAreaCode()
        {
            var codes = new[] { "a", "b" };
            var filteredData = new CoreDataSetFilter(TestData()).SelectWithAreaCode(codes);

            Assert.AreEqual(2, filteredData.Count());
            Assert.IsNotNull(filteredData.First(x => x.AreaCode == "a"));
            Assert.IsNotNull(filteredData.First(x => x.AreaCode == "b"));
        }

        [TestMethod]
        public void TestSelectWithAreaCodeIgnoresEmptyListOfAreaCodes()
        {
            var filteredData = new CoreDataSetFilter(TestData()).SelectWithAreaCode(new string[] { });
            Assert.AreEqual(3, filteredData.Count());
        }

        [TestMethod]
        public void TestSelectWithAreaCodeIgnoresNullListOfAreaCodes()
        {
            var filteredData = new CoreDataSetFilter(TestData()).SelectWithAreaCode(null);
            Assert.AreEqual(3, filteredData.Count());
        }

        [TestMethod]
        public void TestFilterCoreDataSet()
        {
            var codes = new[] { "a", "b" };
            var filteredData = new CoreDataSetFilter(TestData()).RemoveWithAreaCode(codes);

            Assert.AreEqual(1, filteredData.Count());
            Assert.AreEqual("c", filteredData.First().AreaCode);
        }

        [TestMethod]
        public void TestRemoveWithAreaCodeIgnoresEmptyListOfAreaCodes()
        {
            var filteredData = new CoreDataSetFilter(TestData()).RemoveWithAreaCode(new string[] { });
            Assert.AreEqual(3, filteredData.Count());
        }

        [TestMethod]
        public void TestRemoveWithAreaCodeIgnoresNullListOfAreaCodes()
        {
            var filteredData = new CoreDataSetFilter(TestData()).RemoveWithAreaCode(null);
            Assert.AreEqual(3, filteredData.Count());
        }

        [TestMethod]
        public void TestRemoveWithAreaCode_EmptyListInConstructor()
        {
            var filteredData = new CoreDataSetFilter(new List<CoreDataSet>()).RemoveWithAreaCode(new List<string>());
            Assert.AreEqual(0, filteredData.Count());
        }

        [TestMethod]
        public void TestRemoveWithAreaCode_NullListInConstructor()
        {
            var filteredData = new CoreDataSetFilter(null).RemoveWithAreaCode(new List<string>());
            Assert.AreEqual(0, filteredData.Count());
        }

        [TestMethod]
        public void TestFilterCoreDataSetCaseInsensitive()
        {
            // WRT ignored codes
            var filteredData = new CoreDataSetFilter(TestData()).RemoveWithAreaCode(new[] { "A", "B" });

            Assert.AreEqual(1, filteredData.Count());
            Assert.AreEqual("c", filteredData.First().AreaCode);

            // WRT coredataset area codes
            var data = TestData();
            foreach (var coreDataSet in data)
            {
                coreDataSet.AreaCode = coreDataSet.AreaCode.ToUpper();
            }
            filteredData = new CoreDataSetFilter(data).RemoveWithAreaCode(new[] { "a", "b" });

            Assert.AreEqual(1, filteredData.Count());
            Assert.AreEqual("C", filteredData.First().AreaCode);
        }

        public IEnumerable<CoreDataSet> TestData()
        {
            return new[]
                {
                    new CoreDataSet{AreaCode = "a"},
                    new CoreDataSet{AreaCode = "b"},
                    new CoreDataSet{AreaCode = "c"}
                };
        }

    }
}
