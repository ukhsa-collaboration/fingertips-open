using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupDataBuilderByIndicatorIds : GroupDataBuilderBase
    {
        public int AreaTypeId;
        public IList<int> RestrictSearchProfileIds { get; set; }
        public IList<int> IndicatorIds;

        /// <summary>
        /// Mutually exclusive parameter with AreaCode
        /// </summary>
        public string ParentAreaCode;

        /// <summary>
        /// Mutually exclusive parameter with ParentAreaCode
        /// </summary>
        public string AreaCode;

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
            if (string.IsNullOrEmpty(AreaCode) == false)
            {
                return new List<IArea> { AreaFactory.NewArea(AreasReader, AreaCode) };
            }

            // Get child areas of parent
            if (ParentAreaCode == null)
            {
                ParentAreaCode = GetParentAreaCode();
            }

            var areaTypeId = Groupings.First().AreaTypeId;

            return ReadChildAreas(ParentAreaCode, areaTypeId);
        }
    }
}
