using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using ServicesWeb.Controllers;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class IndicatorMetadataControllerTest
    {
        [TestMethod]
        public void TestGetIndicatorMetadataTextProperties()
        {
            var properties = new IndicatorMetadataController().GetIndicatorMetadataTextProperties();
            Assert.IsTrue(properties.Select(x => x.ColumnName)
                .Contains(IndicatorMetadataTextColumnNames.DataSource));
        }
    }
}
