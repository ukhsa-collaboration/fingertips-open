using FingertipsUploadService.ProfileData.Entities.JobError;
using FingertipsUploadService.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace FingertipsUploadService.ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingJobErrorRepository
    {
        private UploadJobErrorRepository _uploadJobErrorRepository;
        private Guid _guid;

        [TestInitialize]
        public void Init()
        {
            _uploadJobErrorRepository = new UploadJobErrorRepository();
            _guid = Guid.NewGuid();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _uploadJobErrorRepository.DeleteLog(_guid);
        }

        [TestMethod]
        public void TestLogJobError()
        {
            var error = new UploadJobError
            {
                JobGuid = _guid,
                ErrorType = UploadJobErrorType.WorkSheetValidationError,
                ErrorText = "wrong name"
            };

            _uploadJobErrorRepository.Log(error);

            var errorFromDb = _uploadJobErrorRepository.FindJobErrorsByJobGuid(_guid).FirstOrDefault();
            Assert.AreEqual(error.JobGuid, errorFromDb.JobGuid);
            Assert.AreEqual(error.ErrorType, errorFromDb.ErrorType);
            Assert.AreEqual(error.ErrorText, errorFromDb.ErrorText);
        }
    }
}
