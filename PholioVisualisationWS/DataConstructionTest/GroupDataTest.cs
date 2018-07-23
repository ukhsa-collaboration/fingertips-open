
using System;
using System.Collections.Specialized;
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
    public class GroupDataTest
    {
        [TestMethod]
        public void TestInitIndicatorMetadata()
        {
            List<Grouping> grouping = new List<Grouping>{
                new Grouping { IndicatorId = IndicatorIds.LifeExpectancyAtBirth},
                new Grouping { IndicatorId = IndicatorIds.Aged0To4Years}
            };

            GroupData data = new GroupData();
            data.InitIndicatorMetadata(IndicatorMetadataProvider.Instance.GetIndicatorMetadataCollection(grouping));

            Assert.AreEqual(2, data.IndicatorMetadata.Count);
            Assert.IsNotNull(data.GetIndicatorMetadataById(IndicatorIds.LifeExpectancyAtBirth));
            Assert.IsNull(data.GetIndicatorMetadataById(IndicatorIds.AdultDrugMisuse));
        }

    }
}
