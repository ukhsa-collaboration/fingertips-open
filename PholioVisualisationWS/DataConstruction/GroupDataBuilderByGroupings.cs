
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupDataBuilderByGroupings : GroupDataBuilderBase
    {
        public string ParentAreaCode;
        public int ChildAreaTypeId;
        public int GroupId;

        protected override void ReadGroupings(IGroupDataReader groupDataReader)
        {
            Groupings = groupDataReader.GetGroupingsByGroupIdAndAreaTypeIdOrderedBySequence(GroupId, ChildAreaTypeId);
        }

        protected override IList<IArea> GetChildAreas()
        {
            IList<IArea> childAreas;

            if (Area.IsAreaListAreaCode(ParentAreaCode))
            {
                childAreas = ReadChildAreas(ParentAreaCode, ChildAreaTypeId);
            }
            else
            {
                childAreas = ReadChildAreas(GetParentAreaCode(), ChildAreaTypeId);
            }

            return childAreas;
        }
    }
}
