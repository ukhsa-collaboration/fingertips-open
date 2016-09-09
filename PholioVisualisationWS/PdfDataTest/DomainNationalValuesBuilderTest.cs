using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfDataTest
{
    /// <summary>
    /// Much of DomainNationalValuesBuilder is covered by SpineChartDataBuilderTest
    /// </summary>
    [TestClass]
    public class DomainNationalValuesBuilderTest
    {
        private static IList<DomainNationalValues> domainNationalValuesList;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            domainNationalValuesList = new DomainNationalValuesBuilder()
                .GetDomainDataForProfile(ProfileIds.Phof,
                AreaTypeIds.CountyAndUnitaryAuthority, new List<string>());
        }

        [TestMethod]
        public void TestAreaValues()
        {
            bool areAnyAreaValuesSet = false;

            var domainNationalValues = domainNationalValuesList.First();
            foreach (var groupRootNationalValues in domainNationalValues.IndicatorData)
            {
                if (groupRootNationalValues.AreaValues.Count > 0)
                {
                    areAnyAreaValuesSet = true;
                    break;
                }
            }

            if (areAnyAreaValuesSet == false)
            {
                Assert.Fail("No area values set");
            }
        }

        [TestMethod]
        public void TestSignificancesAreSet()
        {
            var domainNationalValues = domainNationalValuesList.First();
            foreach (var groupRootNationalValues in domainNationalValues.IndicatorData)
            {
                var dataList = groupRootNationalValues.AreaValues.Values.ToList();
                foreach (var coreDataSet in dataList)
                {
                    Assert.IsTrue(coreDataSet.SignificanceAgainstOneBenchmark.HasValue);
                }
            }
        }

        [TestMethod]
        public void TestGetResponse_MetadataIsSet()
        {
            // Do not want to repeat identical tests in SpineChartDataBuilderTest
            AssertText("ShortName");
        }

        private void AssertText(string propertyName)
        {
            foreach (var spineChartTableData in domainNationalValuesList)
            {
                foreach (var row in spineChartTableData.IndicatorData)
                {
                    var text = (string)row.GetType().GetProperty(propertyName).GetValue(row, null);
                    Assert.IsFalse(string.IsNullOrWhiteSpace(text));
                }
            }
        }
    }
}
