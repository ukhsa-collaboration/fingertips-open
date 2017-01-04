using System.Collections.Generic;
using System.Linq;

namespace Profiles.DomainObjects
{
    public class ProfileCollection
    {
        public int Id { get; set; }
        public string CollectionName { get; set; }
        public string CollectionSkinTitle { get; set; }

        public string UrlKey { get; set; }

        public bool IsCollectionFrontPage
        {
            get { return string.IsNullOrEmpty(UrlKey) == false; }
        }

        public bool IsNationalProfilesCollection
        {
            get { return Id == ProfileCollectionIds.NationalProfiles; }
        }

        public bool ContainsProfileWithId(int profileId)
        {
            return ProfileCollectionItems
                .Select(x => x.ProfileId)
                .Contains(profileId);
        }

        public IEnumerable<ProfileCollectionItem> ProfileCollectionItems { get; set; }

        public IList<ProfileCollectionItem> ProfileCollectionItemsSortedByName()
        {
            if (ProfileCollectionItems == null)
            {
                return new List<ProfileCollectionItem>();
            }

            return ProfileCollectionItems
                .OrderBy(x => x.ProfileDetails.Title)
                .ToList();
        }

        public void AssignParentToCollectionItems()
        {
            foreach (var item in ProfileCollectionItems)
            {
                item.ParentCollection = this;
            }
        }

        
    }
}