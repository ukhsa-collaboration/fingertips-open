using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest.Repositories
{
    [TestClass]
    public class CoreDataSetRepositoryTest
    {
        private CoreDataSetRepository _coreDataSetRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _coreDataSetRepository = new CoreDataSetRepository();
        }

        [TestMethod]
        public void Test_Save_Null_Object_Ignored()
        {
            _coreDataSetRepository.Save(null);
        }

        [TestMethod]
        public void Test_Can_Save_New_Object()
        {
            var timePeriod = new TimePeriod()
            {
                Year = 1980,
                YearRange = 4,
                Quarter = TimePoint.Undefined,
                Month = TimePoint.Undefined
            };

            var grouping = new Grouping
            {
                IndicatorId = IndicatorIds.ExcessWinterDeaths,
                AgeId = AgeIds.AllAges,
                SexId = SexIds.Persons,
                YearRange = timePeriod.YearRange,
            };

            var coreData = new CoreDataSet()
            {
                IndicatorId = grouping.IndicatorId,
                Year = timePeriod.Year,
                YearRange = timePeriod.YearRange,
                Quarter = timePeriod.Quarter,
                Month = timePeriod.Month,
                AreaCode = AreaCodes.England,
                AgeId = grouping.AgeId,
                SexId = grouping.SexId,
                CategoryTypeId = CategoryTypeIds.Undefined,
                CategoryId = CategoryIds.Undefined
            };

            // Arrange: Delete any existing data
            var coreDataFromDb = GetCoreDataFromDb(grouping, timePeriod);
            _coreDataSetRepository.Delete(coreDataFromDb);

            // Act: Save new data
            _coreDataSetRepository.Save(coreData);

            // Assert: Saved data can be read from DB
            coreDataFromDb = GetCoreDataFromDb(grouping, timePeriod);
            Assert.IsNotNull(coreDataFromDb);
        }

        [TestMethod]
        public void TestGetLastestTimePeriodOfCoreData()
        {
            var yearRange = 1;
            var timePeriod = _coreDataSetRepository.GetLastestTimePeriodOfCoreData(
                IndicatorIds.Aged0To4Years, yearRange);

            Assert.IsTrue(timePeriod.Year > 2010);
            Assert.AreEqual(yearRange, timePeriod.YearRange);
        }

        [TestMethod]
        public void TestGetLastestTimePeriodOfCoreData_Returns_Null_Where_No_Data()
        {
            var yearRange = 33;
            var timePeriod = _coreDataSetRepository.GetLastestTimePeriodOfCoreData(
                IndicatorIds.LifeExpectancyAt65, yearRange);

            Assert.IsNull(timePeriod);
        }

        private static CoreDataSet GetCoreDataFromDb(Grouping grouping, TimePeriod timePeriod)
        {
            var coreDataFromDb = ReaderFactory.GetGroupDataReader().GetCoreData(grouping,
                timePeriod, AreaCodes.England);
            return coreDataFromDb.FirstOrDefault();
        }
    }
}
