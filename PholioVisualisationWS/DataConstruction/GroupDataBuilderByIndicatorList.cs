using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.UserData.IndicatorLists;

namespace PholioVisualisation.DataConstruction
{
    public class GroupDataBuilderByIndicatorList : GroupDataBuilderBase
    {
        public int ChildAreaTypeId;
        public string IndicatorListPublicId;
        private IIndicatorListRepository _indicatorListRepository;
        private SingleGroupingProvider _singleGroupingProvider;

        public GroupDataBuilderByIndicatorList(IIndicatorListRepository indicatorListRepository, 
            SingleGroupingProvider singleGroupingProvider) : base()
        {
            _indicatorListRepository = indicatorListRepository;
            _singleGroupingProvider = singleGroupingProvider;
        }

        protected override void ReadGroupings(IGroupDataReader groupDataReader)
        {
            var list = _indicatorListRepository.GetIndicatorList(IndicatorListPublicId);

            // Find groupings
            List<Grouping> groupings = new List<Grouping>();
            foreach (var listItem in list.IndicatorListItems)
            {
                var grouping = _singleGroupingProvider.GetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(
                    ChildAreaTypeId, listItem.IndicatorId, listItem.SexId, listItem.AgeId);
                if (grouping != null)
                {
                    // Need grouping for each comparator ID
                    var grouping2 = new Grouping(grouping);
                    grouping.ComparatorId = ComparatorIds.Subnational;
                    grouping2.ComparatorId = ComparatorIds.England;

                    groupings.Add(grouping);
                    groupings.Add(grouping2);
                }
            }
            Groupings = groupings;
        }

        protected override IList<IArea> GetChildAreas()
        {
            return ReadChildAreas(GetParentAreaCode(), ChildAreaTypeId);
        }
    }
}