using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class ProfileIdListProviderTest
    {
        [TestMethod]
        public void Test_Integration_GetSearchableProfileTds()
        {
            var ids = new ProfileIdListProvider(ReaderFactory.GetProfileReader())
                .GetSearchableProfileIds();
            Assert.IsTrue(ids.Any());
        }
    }
}