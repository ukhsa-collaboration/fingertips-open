
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class AreaLabelsBuilderTest
    {
        [TestMethod]
        public void TestLabels()
        {
            List<Area> areas = new List<Area>();
            for (int i = 1; i <= 10; i++)
            {
                areas.Add(new Area { Code = AreaLabelsBuilder.ShapeAreaCodePrefix + i, Name = "test" + i });
            }
            AreaLabelsBuilder builder = new AreaLabelsBuilder(areas, AreaLabelsBuilder.ShapeAreaCodePrefix);

            // Assert ids and labels match and are complete
            Assert.AreEqual(10, builder.Labels.Count);
            for (int i = 1; i <= 10; i++)
            {
                Assert.AreEqual("test" + i, builder.Labels[i]); ;
            }
        }

        [TestMethod]
        public void TestLabelsForDeprivationDecile()
        {           
            var categories = new List<Category>();
            for (int i = 1; i <= 10; i++)
            {
                categories.Add(new Category {CategoryId = i, CategoryTypeId = i, Name = "test" +i, ShortName = "short-name" +1 });
            }
            AreaLabelsBuilder builder = new AreaLabelsBuilder(categories);

            // Assert ids and labels match and are complete
            Assert.AreEqual(10, builder.Labels.Count);
            for (int i = 1; i <= 10; i++)
            {
                Assert.AreEqual("test" + i, builder.Labels[i]);                
            }
        }
    }
}
