using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest.Repositories
{
    [TestClass]
    public class FpmDocumentRepositoryTest
    {
        private FpmDocumentRepository _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new FpmDocumentRepository();
        }

        [TestMethod]
        public void TestPublishDocument()
        {
            _repository.PublishDocument(Publish());
        }

        private FpmDocument Publish()
        {
            return new FpmDocument
            {
                Id = 3,
                ProfileId = 51,
                FileName = "Diabetes_Hospital_Data.xlsx",
                FileData = Encoding.UTF8.GetBytes("This is a test for document publish"),
                UploadedBy = "Test Publish",
                UploadedOn = DateTime.Now
            };
        }
    }
}
