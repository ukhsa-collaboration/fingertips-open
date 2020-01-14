using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using System;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class DateChangeHelperTest
    {
        private Mock<IMonthlyReleaseHelper> _monthlyReleaseHelper;
        private Mock<ICoreDataAuditRepository> _coreDataAuditRepository;
        private Mock<ICoreDataSetRepository> _coreDataSetRepository;

        private DateChangeHelper _dateChangeHelper;
        private readonly TimePeriod _timePeriod = GetNewTimePeriod();

        [TestInitialize]
        public void Initialize()
        {
            _monthlyReleaseHelper = new Mock<IMonthlyReleaseHelper>(MockBehavior.Strict);
            _coreDataAuditRepository = new Mock<ICoreDataAuditRepository>(MockBehavior.Strict);
            _coreDataSetRepository = new Mock<ICoreDataSetRepository>(MockBehavior.Strict);
        }

        [TestMethod]
        public void Test_ShouldNewDataBeHighlighted_False()
        {
            InitDateChangeHelper();
            var metadata = GetIndicatorMetadata(IndicatorIds.DeprivationScoreIMD2010, null, false);

            var dateChange = _dateChangeHelper.GetIndicatorDateChange(metadata, 1, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);

            // Assert: data has not changed recently because no upload info
            AssertDataNotChangedRecently(dateChange);

            VerifyAll();
        }

        [TestMethod]
        public void Test_ShouldNewDataBeHighlighted_True()
        {
            var indicatorId = IndicatorIds.LearningDisabilityQofPrevalence;
            var areaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019;
            var deploymentCount = 12;
            var dateUploaded = new DateTime(2018, 04, 06);
            var dateOfLastRelease = new DateTime(2018, 04, 01);
            var dateOfNextRelease = new DateTime(2018, 05, 02);
            var dateUpdated = new DateTime(2019, 01, 01);

            _coreDataSetRepository.Setup(x => x.GetCoreDataSetChangeLog(indicatorId, areaTypeId))
                .Returns(new CoreDataSetChangeLog
                {
                    IndicatorId = indicatorId,
                    AreaTypeId = areaTypeId,
                    DateUpdated = dateUpdated
                });

            // Arrange: Monthly release helper
            _monthlyReleaseHelper.Setup(x => x.GetReleaseDate(deploymentCount)).Returns(dateOfLastRelease);
            _monthlyReleaseHelper.Setup(x => x.GetFollowingReleaseDate(dateUpdated)).Returns(dateOfNextRelease);

            InitDateChangeHelper();

            var metadata = GetIndicatorMetadata(indicatorId, dateOfLastRelease, true);

            // Act 
            var dateChange = _dateChangeHelper.GetIndicatorDateChange(metadata, deploymentCount, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);

            // Assert: data has changed recently
            AssertDataChangedRecently(dateChange);

            VerifyAll();
        }

        private void InitDateChangeHelper()
        {
            _dateChangeHelper = new DateChangeHelper(_monthlyReleaseHelper.Object,
                _coreDataAuditRepository.Object, _coreDataSetRepository.Object);
        }

        private static void AssertDataNotChangedRecently(IndicatorDateChange dateChange)
        {
            Assert.IsFalse(dateChange.HasDataChangedRecently);
            Assert.IsNull(dateChange.DateOfLastChange);
        }

        private static void AssertDataChangedRecently(IndicatorDateChange dateChange)
        {
            Assert.IsTrue(dateChange.HasDataChangedRecently);
            Assert.IsNotNull(dateChange.DateOfLastChange);
        }

        private void VerifyAll()
        {
            _monthlyReleaseHelper.VerifyAll();
            _coreDataAuditRepository.VerifyAll();
            _coreDataSetRepository.VerifyAll();
        }

        public IndicatorMetadata GetIndicatorMetadata(int indicatorId, DateTime? latestChangeTimestampOverride, bool newDataBeHighLighted)
        {
            return new IndicatorMetadata
            {
                IndicatorId = indicatorId,
                LatestChangeTimestampOverride = latestChangeTimestampOverride,
                ShouldNewDataBeHighlighted = newDataBeHighLighted
            };
        }

        private static TimePeriod GetNewTimePeriod()
        {
            return new TimePeriod
            {
                Year = 2018,
                YearRange = 1,
                Quarter = TimePoint.Undefined,
                Month = TimePoint.Undefined
            };
        }
    }
}
