using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DomainObjects;

namespace IndicatorsUI.DomainObjectsTest
{
    [TestClass]
    public class ProfileCollectionTest
    {
        [TestMethod]
        public void TestContainsProfileWithIdIsFalse()
        {
            var collection = new ProfileCollection();
            collection.ProfileCollectionItems = new List<ProfileCollectionItem>
            {
                new ProfileCollectionItem{ProfileId = 1}
            };
            Assert.IsFalse(collection.ContainsProfileWithId(2));
        }

        [TestMethod]
        public void TestContainsProfileWithIdIsTrue()
        {
            var collection = new ProfileCollection();
            collection.ProfileCollectionItems = new List<ProfileCollectionItem>
            {
                new ProfileCollectionItem{ProfileId = 2}
            };
            Assert.IsTrue(collection.ContainsProfileWithId(2));
        }
    }
}
