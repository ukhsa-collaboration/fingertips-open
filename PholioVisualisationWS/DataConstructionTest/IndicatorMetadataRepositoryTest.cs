using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class IndicatorMetadataRepositoryTest
    {
        [TestMethod]
        public void TestReduceDescriptiveMetadata()
        {
            IndicatorMetadata indicatorMetadata =IndicatorMetadataProvider.Instance
                .GetIndicatorMetadata(IndicatorIds.ObesityYear6);

            Assert.IsTrue(
                indicatorMetadata.Descriptive.ContainsKey(IndicatorMetadataTextColumnNames.Definition));

            IndicatorMetadataProvider.Instance.ReduceDescriptiveMetadata(
                new List<IndicatorMetadata> { indicatorMetadata });

            Assert.AreEqual(
                IndicatorMetadataProvider.Instance.TruncatedPropertyNames.Length,
                indicatorMetadata.Descriptive.Count);

            // Assert all expected properties are present
            foreach (var propertyName in IndicatorMetadataProvider.Instance.TruncatedPropertyNames)
            {
                Assert.IsTrue(indicatorMetadata.Descriptive.ContainsKey(propertyName));
            }
        }

        [TestMethod]
        public void TestGetMetadataForSpecifiedGroup()
        {
            IList<Grouping> groupings = ReaderFactory.GetGroupDataReader().GetGroupingsByGroupId(GroupIds.SexualAndReproductiveHealth);
            var list = IndicatorMetadataProvider.Instance.GetIndicatorMetadata(groupings, 
                IndicatorMetadataTextOptions.OverrideGenericWithProfileSpecific);
            Assert.IsTrue(list.Count > 0);
        }
    }
}
