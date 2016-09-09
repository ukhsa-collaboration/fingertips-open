using System.Collections.Generic;
using Profiles.DomainObjects;

namespace Profiles.DataConstruction
{
    public class ProfileCollectionListBuilder
    {
        private IProfileCollectionBuilder _profileCollectionBuilder;

        public ProfileCollectionListBuilder(IProfileCollectionBuilder profileCollectionBuilder)
        {
            _profileCollectionBuilder = profileCollectionBuilder;
        }

        public List<ProfileCollection> GetProfileCollections(string urlKey, List<int> profileCollectionIds)
        {
            var profileCollections = new List<ProfileCollection>();
            foreach (int id in profileCollectionIds)
            {
                var profileCollection = _profileCollectionBuilder.GetCollection(id);

                if (profileCollection != null)
                {
                    profileCollection.UrlKey = urlKey;
                    profileCollection.AssignParentToCollectionItems();
                    profileCollections.Add(profileCollection);
                }
            }
            return profileCollections;
        }

    }
}