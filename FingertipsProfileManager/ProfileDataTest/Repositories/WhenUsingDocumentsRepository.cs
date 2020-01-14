using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.ProfileDataTest.Repositories
{
    [TestClass]
    public class WhenUsingDocumentsRepository
    {
        private DocumentsRepository _documentsRepository;

        [TestInitialize]
        public void Init()
        {
            _documentsRepository = new DocumentsRepository();
        }

        [TestMethod]
        public void TestGetDocumentContent()
        {
            var content = _documentsRepository.GetDocumentContent(DocumentIds.UserGuide);
            Assert.IsNotNull(content);
        }
    }
}