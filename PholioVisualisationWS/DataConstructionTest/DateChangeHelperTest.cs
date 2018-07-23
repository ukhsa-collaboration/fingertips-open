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

        private DateChangeHelper _dateChangeHelper;
        private int indicatorID = 1;

        [TestInitialize]
        public void Initialize()
        {
            _monthlyReleaseHelper = new Mock<IMonthlyReleaseHelper>(MockBehavior.Strict);
            _coreDataAuditRepository = new Mock<ICoreDataAuditRepository>(MockBehavior.Strict);
        }

        [TestMethod]
        public void Test_ShouldNewDataBeHighlighted_False_Suppresses_Change()
        {
            InitDateChangeHelper();
            var metadata = GetIndicatorMetadata(null);
            metadata.ShouldNewDataBeHighlighted = false;
            var data = _dateChangeHelper.AssignDateChange(metadata, 90);

            // Assert: data has not changed recently because no upload info
            AssertDataNotChangedRecently(data);

            VerifyAll();
        }

        [TestMethod]
        public void TestAssignDateChangeWhereNoUploadInfo()
        {
            _coreDataAuditRepository.Setup(x => x.GetLatestUploadAuditData(indicatorID))
                .Returns((CoreDataUploadAudit)null);

            InitDateChangeHelper();

            var data = _dateChangeHelper.AssignDateChange(GetIndicatorMetadata(null), 0);

            // Assert: data has not changed recently because no upload info
            AssertDataNotChangedRecently(data);

            VerifyAll();
        }

        [TestMethod]
        public void TestAssignDateChangeWhereOverrideDateIsSetGreaterThanReleaseDate()
        {
            var dateUploaded = new DateTime(2018, 03, 01);
            var dateOfLastRelease = new DateTime(2018, 03, 06);
            var latestChangeTimestampOverride = new DateTime(2018, 03, 25); // Future date

            _coreDataAuditRepository.Setup(x => x.GetLatestUploadAuditData(indicatorID))
                .Returns(new CoreDataUploadAudit
                {
                    DateCreated = dateUploaded
                });

            _monthlyReleaseHelper.Setup(x => x.GetFollowingReleaseDate(dateUploaded)).Returns(dateOfLastRelease);
            SetDateTimeNow();

            InitDateChangeHelper();

            var indicatorMetadata = GetIndicatorMetadata(latestChangeTimestampOverride);

            var data = _dateChangeHelper.AssignDateChange(indicatorMetadata, 0);

            // Assert: Date in future has changed
            Assert.IsTrue(data.HasDataChangedRecently);
            Assert.AreEqual(data.DateOfLastChange, latestChangeTimestampOverride);

            VerifyAll();
        }

        [TestMethod]
        public void TestAssignDateChangeWhereOverrideDateIsSetLessThanReleaseDate()
        {
            var dateUploaded = new DateTime(2018, 03, 01);
            var dateOfLastRelease = new DateTime(2018, 03, 06);
            var latestChangeTimestampOverride = new DateTime(2018, 2, 25);

            _coreDataAuditRepository.Setup(x => x.GetLatestUploadAuditData(indicatorID))
                .Returns(new CoreDataUploadAudit
                {
                    DateCreated = dateUploaded
                });


            _monthlyReleaseHelper.Setup(x => x.GetFollowingReleaseDate(dateUploaded)).Returns(dateOfLastRelease);
            SetDateTimeNow();

            InitDateChangeHelper();

            var indicatorMetadata = GetIndicatorMetadata(latestChangeTimestampOverride);

            var data = _dateChangeHelper.AssignDateChange(indicatorMetadata, 0);

            // Assert
            Assert.IsFalse(data.HasDataChangedRecently);
            Assert.AreEqual(data.DateOfLastChange, dateOfLastRelease);

            VerifyAll();
        }

        [TestMethod]
        public void TestAssignDateChangeWhereOverrideDateIsSetLessThanReleaseDateWithNewDateTimeSpanInDays30()
        {
            var dateUploaded = new DateTime(2018, 03, 01);
            var dateOfLastRelease = new DateTime(2018, 03, 06);
            var latestChangeTimestampOverride = new DateTime(2018, 2, 25);

            _coreDataAuditRepository.Setup(x => x.GetLatestUploadAuditData(indicatorID))
                .Returns(new CoreDataUploadAudit
                {
                    DateCreated = dateUploaded
                });

            _monthlyReleaseHelper.Setup(x => x.GetFollowingReleaseDate(dateUploaded)).Returns(dateOfLastRelease);
            SetDateTimeNow();

            InitDateChangeHelper();

            var indicatorMetadata = GetIndicatorMetadata(latestChangeTimestampOverride);

            var data = _dateChangeHelper.AssignDateChange(indicatorMetadata, 30);

            // Assert
            Assert.IsTrue(data.HasDataChangedRecently);
            Assert.AreEqual(data.DateOfLastChange, dateOfLastRelease);

            VerifyAll();
        }

        private void SetDateTimeNow()
        {
            _monthlyReleaseHelper.Setup(x => x.GetDateTimeNow()).Returns(new DateTime(2018, 03, 10));
        }

        private void InitDateChangeHelper()
        {
            _dateChangeHelper = new DateChangeHelper(_monthlyReleaseHelper.Object,
                _coreDataAuditRepository.Object);
        }

        private static void AssertDataNotChangedRecently(IndicatorDateChange data)
        {
            Assert.IsFalse(data.HasDataChangedRecently);
            Assert.IsNull(data.DateOfLastChange);
        }

        private void VerifyAll()
        {
            _monthlyReleaseHelper.VerifyAll();
            _coreDataAuditRepository.VerifyAll();
        }

        public IndicatorMetadata GetIndicatorMetadata(DateTime? latestChangeTimestampOverride)
        {
            return new IndicatorMetadata
            {
                IndicatorId = indicatorID,
                LatestChangeTimestampOverride = latestChangeTimestampOverride,
                ShouldNewDataBeHighlighted = true
            };
        }
    }
}