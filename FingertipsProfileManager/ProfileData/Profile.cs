using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Entities.Profile;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.ProfileData
{
    public class Profile
    {
        public string Name { get; set; }
        public string UrlKey { get; set; }
        public int Id { get; set; }

        /// <summary>
        /// The indicator names for the selected domain only.
        /// </summary>
        public IList<GroupingPlusName> IndicatorNames { get; set; }
        public IList<GroupingMetadata> GroupingMetadatas { get; set; }
        public IList<AreaType> AreaTypes { get; set; }

        // User selection attributes should not belong to the profile
        public int SelectedAreaType { get; set; }
        public int SelectedDomain { get; set; }
        public bool AreIndicatorsToBeListed { get; set; }
        public bool IsProfileViewable { get; set; }
        public List<string> ContactUserIds { get; set; }
        public bool AreIndicatorNamesDisplayedWithNumbers { get; set; }

        public GroupingMetadata GetSelectedGroupingMetadata(int selectedDomainNumber)
        {
            return GroupingMetadatas.Where(x => x.Sequence == selectedDomainNumber).FirstOrDefault();
        }
    }
}
