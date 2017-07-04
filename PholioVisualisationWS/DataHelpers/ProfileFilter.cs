using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataSorting
{
    public class ProfileFilter
    {
        private static readonly List<int> SystemProfileIds = new List<int>
        {
            ProfileIds.Search,
            ProfileIds.Archived,
            ProfileIds.Unassigned
        };

        /// <summary>
        /// Removes system profiles: search, archived indicators and unassigned indicators.
        /// </summary>
        public static IList<int> RemoveSystemProfileIds(IList<int> profileIds)
        {
            return profileIds
                .Where(x => SystemProfileIds.Contains(x) == false)
                .ToList();
        }

        /// <summary>
        /// Removes system profiles: search, archived indicators and unassigned indicators.
        /// </summary>
        public static IList<ProfileConfig> RemoveSystemProfiles(IList<ProfileConfig> profiles)
        {
            return profiles
                .Where(x => SystemProfileIds.Contains(x.ProfileId) == false)
                .ToList();
        }
    }
}