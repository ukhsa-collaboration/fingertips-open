using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.MainUI.Helpers;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class ColumnAssignerTest
    {
        private int _itemCount;

        [TestMethod]
        public void TestNoItems()
        {
            ExpectColumns(0);
            ExpectColumns(0, 0);
            ExpectColumns(0, 0, 0);
            ExpectColumns(0, 0, 0, 0);
        }

        [TestMethod]
        public void Test2Columns()
        {
            ExpectColumns(1, 0);
            ExpectColumns(1, 1);
            ExpectColumns(5, 4);
            ExpectColumns(5, 5);
        }

        [TestMethod]
        public void Test3Columns()
        {
            ExpectColumns(3, 3, 3);
            ExpectColumns(3, 3, 2);
            ExpectColumns(3, 3, 1);
        }

        [TestMethod]
        public void Test4Columns()
        {
            ExpectColumns(3, 3, 3, 1);
        }


        [TestMethod]
        public void Test2ColumnsOf8()
        {
            ExpectColumns(8,8);
        }

        private void ExpectColumns(params int[] columnItemCounts)
        {
            int columnCount = columnItemCounts.Length;
            int totalItemCount = columnItemCounts.ToList().Sum();

            var assigner = new ColumnAssigner(totalItemCount, columnCount);

            _itemCount = 0;

            for (int i = 0; i < columnCount; i++)
            {
                AssignItemsToCurrentColumn(assigner);

                var expectedItemsSoFar = columnItemCounts.ToList().Take(i + 1).Sum();
                Assert.AreEqual(expectedItemsSoFar, _itemCount);

                assigner.NewColumn();
            }

            Assert.AreEqual(totalItemCount, _itemCount);
        }


        private void AssignItemsToCurrentColumn(ColumnAssigner assigner)
        {
            while (assigner.IsNextIndexInCurrentColumn)
            {
                _itemCount++;
                var index = assigner.NextIndex;
            }
        }

    }
}
