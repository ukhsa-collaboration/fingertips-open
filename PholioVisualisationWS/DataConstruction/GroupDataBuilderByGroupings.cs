
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupDataBuilderByGroupings : GroupDataBuilderBase
    {
        public int ChildAreaTypeId;
        public int GroupId;

        protected override void ReadGroupings(IGroupDataReader groupDataReader)
        {
            Groupings = groupDataReader.GetGroupingsByGroupIdAndAreaTypeIdOrderedBySequence(GroupId, ChildAreaTypeId);
        }

        protected override IList<IArea> GetChildAreas()
        {
           return ReadChildAreas(GetParentAreaCode(), ChildAreaTypeId);
        }
    }
}
