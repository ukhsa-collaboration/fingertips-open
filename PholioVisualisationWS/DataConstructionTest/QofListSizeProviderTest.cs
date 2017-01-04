using System;
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
    public class QofListSizeProviderTest
    {
        const int GroupId = 1200006;
        YearType calendarYearType = new YearType
        {
            Id = YearTypeIds.Calendar
        };

        [TestMethod]
        public void TestCountryValues()
        {
            var areasReader = ReaderFactory.GetAreasReader();
            var groupDataReader = ReaderFactory.GetGroupDataReader();

            // Country
            var size = new QofListSizeProvider(groupDataReader, 
                areasReader.GetAreaFromCode(AreaCodes.England), GroupId, 0, calendarYearType).Value;
            Assert.IsTrue(size > 5000 && size < 10000);
        }

        [TestMethod]
        public void TestPracticeValues()
        {
            var areasReader = ReaderFactory.GetAreasReader();
            var groupDataReader = ReaderFactory.GetGroupDataReader();

            // Practice
            var size = new QofListSizeProvider(groupDataReader,
                areasReader.GetAreaFromCode(AreaCodes.Gp_MonkfieldCambourne), GroupId, 0, calendarYearType).Value;
            Assert.IsTrue(size > 5000 && size < 15000);
        }

        [TestMethod]
        public void TestCcgValue()
        {
            var areasReader = ReaderFactory.GetAreasReader();
            var groupDataReader = ReaderFactory.GetGroupDataReader();

            var size = new QofListSizeProvider(groupDataReader,
                areasReader.GetAreaFromCode(AreaCodes.Ccg_Barnet), GroupId, 0, calendarYearType).Value;
            Assert.IsTrue(size > 5000 && size < 10000);
        }

        [TestMethod]
        public void TestDataPointOffset()
        {
            var areasReader = ReaderFactory.GetAreasReader();
            var groupDataReader = ReaderFactory.GetGroupDataReader();

            // Country
            double lastYearSize = -123456;
            for (int dataPointOffset = 0; dataPointOffset < 100; dataPointOffset++)
            {
                var size = new QofListSizeProvider(groupDataReader,
                    areasReader.GetAreaFromCode(AreaCodes.England), GroupId, dataPointOffset, calendarYearType).Value;
                if (size.HasValue == false)
                {
                    if (dataPointOffset == 0)
                    {
                        Assert.Fail("No QOF data found");
                    }
                    break;
                }
                Assert.AreNotEqual(size, lastYearSize);
                lastYearSize = size.Value;
            }
        }
    }
}
