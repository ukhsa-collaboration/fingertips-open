
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupMetadataBuilder
    {
        public List<int> GroupIds { get; set; }

        private IGroupDataReader groupDataReader;

        public GroupMetadataBuilder(IGroupDataReader groupDataReader)
        {
            this.groupDataReader = groupDataReader;
        }

        public IList<GroupingMetadata> Build()
        {
            if (GroupIds == null || GroupIds.Any() == false)
            {
                return new List<GroupingMetadata>();
            }

            return groupDataReader.GetGroupMetadata(GroupIds.ToList());
        }
    }
}
