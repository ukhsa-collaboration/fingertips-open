using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class PostcodeProviderTest
    {
        [TestMethod]
        public void TestGetNextPostcodeBatchCalled26Times()
        {
            var reader = new Mock<AreasReader>(MockBehavior.Strict);
            reader.Setup(x => x.GetAllPostCodeParentAreasStartingWithLetter(It.IsAny<string>()))
                .Returns(new List<PostcodeParentAreas>());

            var provider = new PostcodeProvider(reader.Object);
            while (provider.AreMorePostcodes)
            {
                var postcodes = provider.GetNextPostcodeBatch();
            }

            reader.Verify(x => x.GetAllPostCodeParentAreasStartingWithLetter(It.IsAny<string>()), Times.Exactly(26));
        }
    }
}
