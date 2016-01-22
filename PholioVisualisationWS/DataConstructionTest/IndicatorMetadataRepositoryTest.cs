using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class IndicatorMetadataRepositoryTest
    {
        [TestMethod]
        public void TestReduceDescriptiveMetadata()
        {
            IndicatorMetadata indicatorMetadata =IndicatorMetadataRepository.Instance
                .GetIndicatorMetadata(IndicatorIds.ObesityYear6);

            Assert.IsTrue(
                indicatorMetadata.Descriptive.ContainsKey(IndicatorMetadataTextColumnNames.Definition));

            IndicatorMetadataRepository.Instance.ReduceDescriptiveMetadata(
                new List<IndicatorMetadata> { indicatorMetadata });

            Assert.AreEqual(
                IndicatorMetadataRepository.Instance.TruncatedPropertyNames.Length,
                indicatorMetadata.Descriptive.Count);

            // Assert all expected properties are present
            foreach (var propertyName in IndicatorMetadataRepository.Instance.TruncatedPropertyNames)
            {
                Assert.IsTrue(indicatorMetadata.Descriptive.ContainsKey(propertyName));
            }
        }

        [TestMethod]
        public void TestGetMetadataForSpecifiedGroup()
        {
            IList<Grouping> groupings = ReaderFactory.GetGroupDataReader().GetGroupingsByGroupId(GroupIds.Diabetes_TreatmentTargets);
            var list = IndicatorMetadataRepository.Instance.GetIndicatorMetadata(groupings);
            Assert.IsTrue(list.Count > 0);
        }
    }
}
