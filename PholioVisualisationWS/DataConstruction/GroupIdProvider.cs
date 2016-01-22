using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public interface IGroupIdProvider
    {
        IList<int> GetGroupIds(int profileId);
    }

    public class GroupIdProvider : IGroupIdProvider
    {
        private IProfileReader profilesReader;

        /// <summary>
        /// To enable creation of Mock objects.
        /// </summary>
        protected GroupIdProvider() { }

        public GroupIdProvider(IProfileReader profilesReader)
        {
            this.profilesReader = profilesReader;
        }

        public virtual IList<int> GetGroupIds(int profileId)
        {
            if (IsSearchProfile(profileId) || IsProfileUndefined(profileId))
            {
                return profilesReader.GetGroupIdsFromAllProfiles();
            }

            return GetGroupIdsOfSpecificProfile(profileId);
        }

        private IList<int> GetGroupIdsOfSpecificProfile(int profileId)
        {
            var profile = profilesReader.GetProfile(profileId);
            if (profile != null)
            {
                // Ensure group IDs are in correct order
                return ReaderFactory.GetGroupDataReader()
                    .GetGroupMetadata(profile.GroupIds)
                    .Select(x => x.Id)
                    .ToList();
            }
            return new List<int>();
        }

        private static bool IsSearchProfile(int id)
        {
            return id == ProfileIds.Search;
        }

        private static bool IsProfileUndefined(int id)
        {
            return id == ProfileIds.Undefined;
        }
    }
}
