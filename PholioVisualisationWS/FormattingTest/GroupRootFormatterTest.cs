
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.FormattingTest
{
    [TestClass]
    public class GroupRootFormatterTest
    {
        [TestMethod]
        public void TestTimePeriodsAreFormatted()
        {
            GroupRoot groupRoot = new GroupRoot();
            groupRoot.Grouping.Add(new Grouping { DataPointYear = 2005, YearRange = 1, DataPointQuarter = -1 });
            IndicatorMetadata metadata = new IndicatorMetadata { YearTypeId = 1 };

            Limits limits = new Limits { Min = 0, Max = 50 };
            var formatter = new NumericFormatterFactory(null).NewWithLimits(metadata, limits);
            new GroupRootFormatter().Format(groupRoot, metadata, new DataPointTimePeriodFormatter(), formatter);

            Assert.AreEqual("2005", groupRoot.Grouping[0].TimePeriodText);
        }
    }
}
