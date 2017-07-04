using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.ProfileDataTest
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
