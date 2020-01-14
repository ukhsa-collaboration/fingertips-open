
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class QuinaryPopulationCalculatorTest
    {

        [TestMethod]
        public void Test_Calculate_England_Population_For_Gp()
        {
            var calculator = new QuinaryPopulationCalculator(ReaderFactory.GetGroupDataReader(), ReaderFactory.GetAreasReader());

            var population = calculator.CalculateParentPopulationValues(AreaCodes.England, AreaTypeIds.GpPractice,
                IndicatorIds.QuinaryPopulations, SexIds.Male, new TimePeriod
                {
                    Month = -1,
                    Quarter = -1,
                    Year = 2018,
                    YearRange = 1
                });

            Assert.IsTrue(population.ChildAreaCount > 6000 && population.ChildAreaCount < 9000);
            Assert.AreEqual(20, population.Values.Count, "Expect quinary bands to 95+");
        }

        [TestMethod]
        public void Test_Calculate_Deprivation_Decile_Population_For_County_Ua()
        {
            var calculator = new QuinaryPopulationCalculator(ReaderFactory.GetGroupDataReader(), ReaderFactory.GetAreasReader());

            var code = CategoryArea.CreateAreaCode(CategoryTypeIds.DeprivationDecileCountyAndUA2015, 5);
            
            var population = calculator.CalculateParentPopulationValues(code, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,
                IndicatorIds.ResidentPopulation, SexIds.Male, new TimePeriod
                {
                    Month = -1,
                    Quarter = -1,
                    Year = 2016,
                    YearRange = 1
                });

            Assert.AreEqual(15, population.ChildAreaCount);
            Assert.AreEqual(19, population.Values.Count, "Expect quinary bands to 90+");
        }

    }
}
