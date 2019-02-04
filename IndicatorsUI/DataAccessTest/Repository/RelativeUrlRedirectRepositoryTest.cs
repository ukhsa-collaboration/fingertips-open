using System;
using System.Collections.Generic;
using System.Linq;
using IndicatorsUI.DataAccess.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUI.DataAccessTest.Repository
{
    [TestClass]
    public class RelativeUrlRedirectRepositoryTest
    {
        [TestMethod]
        public void TestGetAllRelativeUrlRedirects()
        {
            var redirects = new RelativeUrlRedirectRepository().GetAllRelativeUrlRedirects();

            Assert.IsTrue(redirects.Any());
            Assert.IsNotNull(redirects.First().FromUrl);
            Assert.IsNotNull(redirects.First().ToUrl);
        }
    }
}
