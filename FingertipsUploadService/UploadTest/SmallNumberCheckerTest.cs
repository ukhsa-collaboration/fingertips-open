using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Profile;
using FingertipsUploadService.ProfileData.Repositories;
using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data;
using System.Linq;

namespace FingertipsUploadService.UploadTest
{
    [TestClass]
    public class SmallNumberCheckerTest
    {
        private const int IndicatorId = 2;
        private const int DisclosureControlId = 1;
        private const int RowId = 3;
        private const string AreaCode = "a";

        private Mock<IDisclosureControlRepository> _disclosureControlRepository;
        private Mock<ProfilesReader> _profilesreader;
        private Mock<AreaTypeRepository> _areaTypeRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _disclosureControlRepository = new Mock<IDisclosureControlRepository>(MockBehavior.Strict);
            _disclosureControlRepository.Setup(x => x.GetDisclosureControlById(DisclosureControlId))
                .Returns(new DisclosureControl { Formula = "($ >0) AND ($ <=5)" });

            _profilesreader = new Mock<ProfilesReader>(MockBehavior.Strict);

            _areaTypeRepository = new Mock<AreaTypeRepository>(MockBehavior.Strict);
            _areaTypeRepository.Setup(x => x.ShouldWarnAboutSmallNumbersForArea(AreaCode))
                .Returns(true);
        }

        [TestMethod]
        public void Test_Small_Number_Is_Flagged()
        {
            // Arrange: data
            var row = DataRow();
            row[UploadColumnNames.Count] = 1;
            var batchUpload = new BatchUpload();

            // Arrange: dependencies
            SetupProfilesReaderWithDisclosureId(DisclosureControlId);

            // Act
            SmallNumberChecker().Check(row, RowId, batchUpload);

            // Assert
            Assert.AreEqual(1, batchUpload.SmallNumberWarnings.Count);
            VerifyMocks();
        }

        [TestMethod]
        public void Test_Large_Number_Is_Ignored()
        {
            // Arrange: data
            var row = DataRow();
            row[UploadColumnNames.Count] = 10;
            var batchUpload = new BatchUpload();

            // Arrange: dependencies
            SetupProfilesReaderWithDisclosureId(DisclosureControlId);

            // Act
            SmallNumberChecker().Check(row, RowId, batchUpload);

            // Assert
            Assert.IsFalse(batchUpload.SmallNumberWarnings.Any());
            VerifyMocks();
        }

        [TestMethod]
        public void Test_Row_Ignored_Where_No_Disclosure_Required_For_Indicator()
        {
            // Arrange: data
            var row = DataRow();
            row[UploadColumnNames.Count] = 1;
            var batchUpload = new BatchUpload();

            // Arrange: dependencies
            _disclosureControlRepository = new Mock<IDisclosureControlRepository>(MockBehavior.Strict);
            SetupProfilesReaderWithDisclosureId(DisclosureControlIds.NoCheckRequired);
            _areaTypeRepository = new Mock<AreaTypeRepository>(MockBehavior.Strict);

            // Act
            new SmallNumberChecker(_profilesreader.Object, _disclosureControlRepository.Object,
               _areaTypeRepository.Object)
                .Check(row, RowId, batchUpload);

            // Assert
            Assert.IsFalse(batchUpload.SmallNumberWarnings.Any());
            VerifyMocks();
        }

        [TestMethod]
        public void IntegrationTest_Small_Number_Is_Flagged()
        {
            // Arrange: data
            var row = DataRow();
            row[UploadColumnNames.Count] = 1;
            var batchUpload = new BatchUpload();

            // Arrange: dependencies
            SetupProfilesReaderWithDisclosureId(DisclosureControlIds.GreaterThan0LessThanOrEqualto5);

            // Act
            new SmallNumberChecker(_profilesreader.Object, new DisclosureControlRepository(), _areaTypeRepository.Object)
                .Check(row, RowId, batchUpload);

            // Assert
            Assert.AreEqual(1, batchUpload.SmallNumberWarnings.Count);
        }

        [TestMethod]
        public void IntegrationTest_Large_Number_Is_Flagged()
        {
            // Arrange: data
            var row = DataRow();
            row[UploadColumnNames.Count] = 10;
            var batchUpload = new BatchUpload();

            // Arrange: dependencies
            SetupProfilesReaderWithDisclosureId(DisclosureControlIds.GreaterThan0LessThanOrEqualto5);

            // Act
            new SmallNumberChecker(_profilesreader.Object, new DisclosureControlRepository(), _areaTypeRepository.Object)
                .Check(row, RowId, batchUpload);

            // Assert
            Assert.IsFalse(batchUpload.SmallNumberWarnings.Any());
        }

        private static DataRow DataRow()
        {
            var row = UploadBatchRowParserTest.GetTestDataRow();
            row[UploadColumnNames.IndicatorId] = IndicatorId;
            return row;
        }

        private void SetupProfilesReaderWithDisclosureId(int d)
        {
            _profilesreader.Setup(x => x.GetIndicatorMetadata(IndicatorId))
                .Returns(new IndicatorMetadata { DisclosureControlId = d });
        }

        private SmallNumberChecker SmallNumberChecker()
        {
            var checker = new SmallNumberChecker(_profilesreader.Object,
                _disclosureControlRepository.Object, _areaTypeRepository.Object);
            return checker;
        }

        private void VerifyMocks()
        {
            _disclosureControlRepository.VerifyAll();
            _profilesreader.VerifyAll();
            _areaTypeRepository.VerifyAll();
        }
    }
}
