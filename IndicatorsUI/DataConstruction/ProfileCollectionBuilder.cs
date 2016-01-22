using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace Profiles.DataConstruction
{
    public class ProfileCollectionBuilder
    {
        private ProfileReader profileReader;
        private AppConfig config;

        public ProfileCollectionBuilder(ProfileReader profileReader, AppConfig config)
        {
            this.profileReader = profileReader;
            this.config = config;
        }

        public ProfileCollection GetCollection(int profileCollectionId)
        {
            var profileCollection = profileReader.GetProfileCollection(profileCollectionId);
            profileCollection.ProfileCollectionItems = profileReader.GetProfileCollectionItems(profileCollection.Id);
            AddProfileDetails(profileCollection);
            return profileCollection;
        }

        private void AddProfileDetails(ProfileCollection profileCollection)
        {
            var isLive = config.IsEnvironmentLive;

            foreach (var item in profileCollection.ProfileCollectionItems)
            {
                var profileDetails = new ProfileDetailsBuilder(item.ProfileId).Build();
                item.ProfileDetails = profileDetails;
                if (profileDetails.HasExclusiveSkin)
                {
                    var skin = profileReader.GetSkinFromId(profileDetails.SkinId);
                    item.ExternalUrl = isLive ?
                        skin.LiveHost :
                        skin.TestHost;
                }
            }
        }
    }
}
