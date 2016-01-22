using FingertipsUploadService.ProfileData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProfileDataTest
{
    [TestClass]
    public class TimePeriodTest
    {
        public const int Undefined = TimePeriod.Undefined;

        [TestMethod]
        public void TestIsLaterThanForAnnual()
        {
            var earlier = Period(2001, Undefined, Undefined);
            var later = Period(2002, Undefined, Undefined);

            Assert.IsTrue(later.IsLaterThan(earlier));
        }

        [TestMethod]
        public void TestIsLaterThanForQuarterlySameYear()
        {
            var earlier = Period(2001, 1, Undefined);
            var later = Period(2001, 2, Undefined);

            Assert.IsTrue(later.IsLaterThan(earlier));
        }

        [TestMethod]
        public void TestIsLaterThanForQuarterlyDifferentYear()
        {
            var earlier = Period(2000, 1, Undefined);
            var later = Period(2001, 1, Undefined);

            Assert.IsTrue(later.IsLaterThan(earlier));
        }

        [TestMethod]
        public void TestIsLaterThanForMonthlySameYear()
        {
            var earlier = Period(2001, Undefined, 1);
            var later = Period(2001, Undefined, 2);

            Assert.IsTrue(later.IsLaterThan(earlier));
        }

        [TestMethod]
        public void TestIsLaterThanForMonthDifferentYear()
        {
            var earlier = Period(2000, Undefined, 1);
            var later = Period(2001, Undefined, 1);

            Assert.IsTrue(later.IsLaterThan(earlier));
        }

        public TimePeriod Period(int year, int quarter, int month)
        {
            return new TimePeriod
            {
                Year = year,
                Quarter = quarter,
                Month = month
            };
        }
    }
}
