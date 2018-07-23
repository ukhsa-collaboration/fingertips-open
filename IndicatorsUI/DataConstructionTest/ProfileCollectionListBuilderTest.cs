using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IndicatorsUI.DataConstruction;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.DataConstructionTest
{
    [TestClass]
    public class ProfileCollectionListBuilderTest
    {
        private Mock<IProfileCollectionBuilder> _builder;

        public const string UrlKey = "key";
        public const int ProfileCollectionId = 1;

        [TestInitialize]
        public void TestInitialize()
        {
            _builder = new Mock<IProfileCollectionBuilder>();
        }

        [TestMethod]
        public void Test_Ids_That_Are_Not_Valid_Are_Ignored()
        {
            // Arrange
            _builder.Setup(x => x.GetCollection(ProfileCollectionId))
                .Returns<ProfileCollection>(null);

            // Act 
            var collections = new ProfileCollectionListBuilder(_builder.Object)
                .GetProfileCollections(UrlKey, new List<int> { ProfileCollectionId });

            // Assert
            Assert.AreEqual(0, collections.Count);
        }

        [TestMethod]
        public void Test_Profile_Collections_Are_Initialised_Correctly()
        {
            // Arrange
            var profileCollection = new ProfileCollection
            {
                ProfileCollectionItems = new List<ProfileCollectionItem>
                {
                    new ProfileCollectionItem()
                }
            };

            _builder.Setup(x => x.GetCollection(ProfileCollectionId))
                .Returns(profileCollection);

            // Act 
            var collections = new ProfileCollectionListBuilder(_builder.Object)
                .GetProfileCollections(UrlKey, new List<int> { ProfileCollectionId });

            // Assert: collection
            var collection = collections.First();
            Assert.AreEqual(1, collections.Count);
            Assert.AreEqual(UrlKey, collection.UrlKey, "UrlKey should have been assigned");

            // Assert: collection items
            var collectionItems = collection.ProfileCollectionItems;
            Assert.AreEqual(1, collectionItems.Count());
            Assert.AreEqual(collection, collectionItems.First().ParentCollection, "Parent collection should be assigned");
        }
    }
}
