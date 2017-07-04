using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class MultipleIndicatorMetadataFileWriterTest
    {
        public const int IndicatorId = 2345;
        private static readonly IndicatorMetadataProvider _indicatorMetadataProvider =
            IndicatorMetadataProvider.Instance;

        private readonly IList<IndicatorMetadataTextProperty> _properties =
            _indicatorMetadataProvider.IndicatorMetadataTextProperties;

        [TestMethod]
        public void TestGetMetadataFileAsBytes()
        {
            var file = new MultipleIndicatorMetadataFileWriter().GetMetadataFileAsBytes(
                _indicatorMetadataProvider.GetIndicatorMetadata(new List<int>
                {
                    IndicatorIds.ExcessWinterDeaths, IndicatorIds.GcseAchievement
                }), _properties);

            Assert.IsTrue(file.Length > 0);
        }

        [TestMethod]
        public void Test_Properties_Are_Written()
        {
            var metadata = GetIndicatorMetadata();

            var content = new MultipleIndicatorMetadataFileWriter()
                .GetMetadataFileAsBytes(new List<IndicatorMetadata> { metadata},
                new List<IndicatorMetadataTextProperty>());

            var csvContent = Encoding.UTF8.GetString(content);

            Assert.IsTrue(csvContent.Contains(IndicatorId.ToString()));
            Assert.IsTrue(csvContent.Contains("MyUnit"));
            Assert.IsTrue(csvContent.Contains("ValueTypeName"));
            Assert.IsTrue(csvContent.Contains("YearTypeName"));
        }

        [TestMethod]
        public void Test_Descriptive_Properties_Are_Written()
        {
            var metadata = GetIndicatorMetadata();

            var content = new MultipleIndicatorMetadataFileWriter()
                .GetMetadataFileAsBytes(new List<IndicatorMetadata> { metadata },
                new List<IndicatorMetadataTextProperty> {new IndicatorMetadataTextProperty()
                {
                    ColumnName = "IndicatorNameColumnName",
                    DisplayName = "IndicatorNameDisplayName"
                } });

            var csvContent = Encoding.UTF8.GetString(content);

            Assert.IsTrue(csvContent.Contains("IndicatorNameDisplayName"));
            Assert.IsTrue(csvContent.Contains("IndicatorNameValue"));
        }

        private static IndicatorMetadata GetIndicatorMetadata()
        {
            var indicatorMetadata = new IndicatorMetadata
            {
                IndicatorId = IndicatorId,
                Unit = new Unit { Label = "MyUnit" },
                ValueType = new PholioObjects.ValueType { Name = "ValueTypeName" },
                YearType = new YearType { Name = "YearTypeName" }
            };

            indicatorMetadata.Descriptive = new Dictionary<string, string>
            {
                {"IndicatorNameColumnName", "IndicatorNameValue"}
            };

            return indicatorMetadata;
        }
    }
}
