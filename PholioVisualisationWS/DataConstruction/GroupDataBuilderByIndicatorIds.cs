using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupDataBuilderByIndicatorIds : GroupDataBuilderBase
    {
        public IList<int> IndicatorIds;
        public string ParentAreaCode;
        public int AreaTypeId;

        public List<int> RestrictSearchProfileIds { get; set; }

        protected override void ReadGroupings(IGroupDataReader groupDataReader)
        {
            var profileIds = RestrictSearchProfileIds != null && RestrictSearchProfileIds.Any() ?
                RestrictSearchProfileIds :
                new List<int> { ProfileId };

            var profileReader = ReaderFactory.GetProfileReader();

            Groupings = new GroupingListProvider(groupDataReader, profileReader)
                .GetGroupings(profileIds, IndicatorIds, AreaTypeId);
        }

        protected override IList<IArea> GetChildAreas()
        {
            if (ParentAreaCode == null)
            {
                ParentAreaCode = GetParentAreaCode();
            }

            var areaTypeId = Groupings.First().AreaTypeId;

            return ReadChildAreas(ParentAreaCode, areaTypeId);
        }
    }
}
