using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.MainUI.Common;

namespace IndicatorsUITest
{
    [TestClass]
    public class RowAssignerTest
    {
        [TestMethod]
        public void Test_2Columns()
        {
            var rowAssigner = new RowAssigner(2);

            // Row 1 Column 1
            Assert.IsTrue(rowAssigner.IsItemFirstOfRow);
            Assert.IsFalse(rowAssigner.IsItemLastOfRow);
            rowAssigner.ItemAdded();

            // Row 1 Column 2
            Assert.IsFalse(rowAssigner.IsItemFirstOfRow);
            Assert.IsTrue(rowAssigner.IsItemLastOfRow);
            rowAssigner.ItemAdded();

            // Row 2 Column 1
            Assert.IsTrue(rowAssigner.IsItemFirstOfRow);
            Assert.IsFalse(rowAssigner.IsItemLastOfRow);
            rowAssigner.ItemAdded();

            // Row 2 Column 2
            Assert.IsFalse(rowAssigner.IsItemFirstOfRow);
            Assert.IsTrue(rowAssigner.IsItemLastOfRow);
            rowAssigner.ItemAdded();
        }

        [TestMethod]
        public void Test_3Columns()
        {
            var rowAssigner = new RowAssigner(3);

            // Row 1 Column 1
            Assert.IsTrue(rowAssigner.IsItemFirstOfRow);
            Assert.IsFalse(rowAssigner.IsItemLastOfRow);
            rowAssigner.ItemAdded();

            // Row 1 Column 2
            Assert.IsFalse(rowAssigner.IsItemFirstOfRow);
            Assert.IsFalse(rowAssigner.IsItemLastOfRow);
            rowAssigner.ItemAdded();

            // Row 1 Column 3
            Assert.IsFalse(rowAssigner.IsItemFirstOfRow);
            Assert.IsTrue(rowAssigner.IsItemLastOfRow);
            rowAssigner.ItemAdded();

            // Row 2 Column 1
            Assert.IsTrue(rowAssigner.IsItemFirstOfRow);
            Assert.IsFalse(rowAssigner.IsItemLastOfRow);
            rowAssigner.ItemAdded();

            // Row 2 Column 2
            Assert.IsFalse(rowAssigner.IsItemFirstOfRow);
            Assert.IsFalse(rowAssigner.IsItemLastOfRow);
            rowAssigner.ItemAdded();

            // Row 2 Column 3
            Assert.IsFalse(rowAssigner.IsItemFirstOfRow);
            Assert.IsTrue(rowAssigner.IsItemLastOfRow);
            rowAssigner.ItemAdded();
        }
    }
}
