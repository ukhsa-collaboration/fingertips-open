using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class AuditProviderTest
    {
        [TestMethod]
        public void GetLatestAuditDataTest()
        {
            var provider = new AuditProvider();
            var data = provider.GetLatestAuditData(IndicatorIds.LifeExpectancyAtBirth);
            Assert.IsNotNull(data);
        }
    }
}
