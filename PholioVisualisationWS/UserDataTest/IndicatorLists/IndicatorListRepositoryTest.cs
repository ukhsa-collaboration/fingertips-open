using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.UserData.IndicatorLists;

namespace PholioVisualisation.UserDataTest.IndicatorLists
{
    [TestClass]
    public class IndicatorListRepositoryTest
    {
        private const string PublicId = "-1";

        /// <summary>
        /// Required to allow EF tests to run on server
        /// </summary>
        public void FixEntityFrameworkProviderServicesProblem()
        {
            // See https://stackoverflow.com/questions/14033193/entity-framework-provider-type-could-not-be-loaded
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        [TestMethod]
        public void Test_IndicatorLists_Can_Be_Retrieved()
        {
            var repo = new IndicatorListRepository();
            Assert.IsNotNull(repo.IndicatorLists);
        }

        [TestMethod]
        public void Test_GetIndicatorList_That_Does_Not_Exist()
        {
            var repo = new IndicatorListRepository();
            Assert.IsNull(repo.GetIndicatorList(PublicId));
        }

        [TestMethod]
        public void Test_GetIndicatorIdsInIndicatorList_For_IndicatorList_That_Does_Not_Exist()
        {
            var repo = new IndicatorListRepository();
            Assert.IsFalse(repo.GetIndicatorIdsInIndicatorList(PublicId).Any());
        }

    }
}
