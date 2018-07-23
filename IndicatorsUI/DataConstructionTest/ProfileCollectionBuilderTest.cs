using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DataConstruction;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.DataConstructionTest
{
    [TestClass]
    public class ProfileCollectionBuilderTest
    {
        [TestMethod]
        public void TestGetCollection_NationalProfiles()
        {
            var parameters = new NameValueCollection();
            parameters.Add("Environment","Live");

            var profileCollection = new ProfileCollectionBuilder(ReaderFactory.GetProfileReader(), new AppConfig(parameters))
                .GetCollection(ProfileCollectionIds.NationalProfiles);

            Assert.IsNotNull(profileCollection);
            Assert.IsTrue(profileCollection.ProfileCollectionItems.Any());

            // Assert ExternalUrl has been assigned where profile has own skin
            var url = profileCollection.ProfileCollectionItems.First(x => x.ProfileDetails.HasExclusiveSkin).ExternalUrl;
            Assert.IsFalse(string.IsNullOrEmpty(url));
        }
    }
}
