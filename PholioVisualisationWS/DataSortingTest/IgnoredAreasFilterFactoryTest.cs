using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataSortingTest
{
    [TestClass]
    public class IgnoredAreasFilterFactoryTest
    {
        [TestMethod]
        public void TestNew()
        {
            var filter = IgnoredAreasFilterFactory.New(ProfileIds.Phof);
            Assert.IsNotNull(filter);
        }
    }
}
