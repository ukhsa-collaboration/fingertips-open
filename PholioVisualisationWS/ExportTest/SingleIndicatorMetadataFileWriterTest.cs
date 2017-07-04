using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class SingleIndicatorMetadataFileWriterTest
    {
        [TestMethod]
        public void Test_Properties_Are_Written()
        {
            var metadata = GetIndicatorMetadata();

            var content = new SingleIndicatorMetadataFileWriter()
                .GetMetadataFileAsBytes(metadata, new List<IndicatorMetadataTextProperty>());

            var csvContent = Encoding.UTF8.GetString(content);

            Assert.IsTrue(csvContent.Contains("MyUnit"));
            Assert.IsTrue(csvContent.Contains("ValueTypeName"));
            Assert.IsTrue(csvContent.Contains("YearTypeName"));
        }

        private static IndicatorMetadata GetIndicatorMetadata()
        {
            var indicatorMetadata = new IndicatorMetadata
            {
                Unit = new Unit {Label = "MyUnit"},
                ValueType = new PholioObjects.ValueType {Name = "ValueTypeName"},
                YearType = new YearType {Name = "YearTypeName"}
            };

            return indicatorMetadata;
        }
    }
}
