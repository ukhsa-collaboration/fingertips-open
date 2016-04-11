using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class PracticePerformanceIndicatorValuesTest
    {
        [TestMethod]
        public void TestPracticePerformanceIndicatorValues()
        {
            PracticePerformanceIndicatorValues values = new PracticePerformanceIndicatorValues(ReaderFactory.GetGroupDataReader(),
                "C85026", 0);

            var val = values.IndicatorToValue[PracticePerformanceIndicatorValues.Qof];
            Assert.IsTrue(val != null && val.IsValueValid);

            val = values.IndicatorToValue[PracticePerformanceIndicatorValues.PatientsThatWouldRecommendPractice];
            Assert.IsTrue(val != null && val.IsValueValid);

            val = values.IndicatorToValue[PracticePerformanceIndicatorValues.LifeExpectancyMale];
            Assert.IsTrue(val != null && val.IsValueValid);

            val = values.IndicatorToValue[PracticePerformanceIndicatorValues.LifeExpectancyFemale];
            Assert.IsTrue(val != null && val.IsValueValid);
        }

        [TestMethod]
        public void TestPracticePerformanceIndicatorValuesDataPointYearOvershoot()
        {
                PracticePerformanceIndicatorValues values =
                    new PracticePerformanceIndicatorValues(ReaderFactory.GetGroupDataReader(), "C85026", 100);
                Assert.AreEqual(0, values.IndicatorToValue.Count);
        }
    }
}
