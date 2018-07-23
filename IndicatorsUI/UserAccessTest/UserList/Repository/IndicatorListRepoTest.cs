using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.UserAccess;
using IndicatorsUI.UserAccess.UserList.Repository;

namespace IndicatorsUI.UserAccessTest.UserList.Repository
{
    [TestClass]
    public class IndicatorListRepoTest
    {
        /// <summary>
        /// Required to allow EF tests to run on server
        /// </summary>
        public void FixEntityFrameworkProviderServicesProblem()
        {
            // See https://stackoverflow.com/questions/14033193/entity-framework-provider-type-could-not-be-loaded
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        [TestMethod]
        public void TestGetListName()
        {
            var repo = new IndicatorListRepo(new FINGERTIPS_USERSEntities());
            Assert.IsNull(repo.GetListNameByPublicId("a"));
        }
    }
}
