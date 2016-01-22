using System.Collections.Generic;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate.Mapping;

namespace ProfileDataTest
{
    [TestClass]
    public class AllowedDataTest
    {
        [TestMethod]
        public void TestAllDataListsAreInitialised()
        {
            var reader = new Mock<ProfilesReader>(MockBehavior.Strict);
            reader.Setup(x => x.GetAllAgeIds()).Returns(new List<int>());
            reader.Setup(x => x.GetAllSexIds()).Returns(new List<int>());
            reader.Setup(x => x.GetAllAreaCodes()).Returns(new List<string>());
            reader.Setup(x => x.GetAllValueNoteIds()).Returns(new List<int>());
            reader.Setup(x => x.GetAllCategories()).Returns(new List<Category>());

            var validData = new AllowedData(reader.Object);

            Assert.IsNotNull(validData.AgeIds);
            Assert.IsNotNull(validData.AreaCodes);
            Assert.IsNotNull(validData.SexIds);
            Assert.IsNotNull(validData.ValueNoteIds);
            Assert.IsNotNull(validData.Categories);

            reader.VerifyAll();
        }

        [TestMethod]
        public void TestAllDataAreLazyLoaded()
        {
            var reader = new Mock<ProfilesReader>(MockBehavior.Strict);
            var validData = new AllowedData(reader.Object);
            reader.VerifyAll();
        }
    }
}