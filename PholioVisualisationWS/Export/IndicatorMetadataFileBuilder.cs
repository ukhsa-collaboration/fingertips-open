using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class IndicatorMetadataFileBuilder
    {
        private IndicatorMetadataProvider _indicatorMetadataProvider;
        private GroupingListProvider _groupingListProvider;

        public IndicatorMetadataFileBuilder(IndicatorMetadataProvider indicatorMetadataProvider,
            GroupingListProvider groupingListProvider)
        {
            _indicatorMetadataProvider = indicatorMetadataProvider;
            _groupingListProvider = groupingListProvider;
        }

        public byte[] GetFileForGroups(IList<int> groupIds)
        {
            var groupings = new List<Grouping>();

            foreach (var groupId in groupIds)
            {
                groupings.AddRange(GetGroupingsForGroup(groupId));
            }

            return GetFileForMultipleIndicators(groupings);
        }

        public byte[] GetFileForSpecifiedIndicators(IList<int> indicatorIds, int? profileId)
        {
            var groupings = GetGroupingsFromIndicatorIds(indicatorIds, profileId);
            return GetFileForMultipleIndicators(groupings);
        }

        private List<Grouping> GetGroupingsForGroup(int group_id)
        {
            var groupings = _groupingListProvider
                .GetGroupingsByGroup(group_id)
                .OrderBy(x => x.Sequence)
                .ToList();
            return groupings;
        }

        private IList<Grouping> GetGroupingsFromIndicatorIds(IList<int> indicatorIds, int? profileId)
        {
            IList<Grouping> groupings;
            if (profileId.HasValue)
            {
                groupings = _groupingListProvider.GetGroupings(new List<int> { profileId.Value },
                    indicatorIds);
            }
            else
            {
                groupings = _groupingListProvider.GetGroupings(indicatorIds);
            }
            return groupings;
        }

        private byte[] GetFileForMultipleIndicators(IList<Grouping> groupings)
        {
            if (groupings.Any() == false) return null;

            var metadataList = _indicatorMetadataProvider.GetIndicatorMetadata(groupings,
            IndicatorMetadataTextOptions.OverrideGenericWithProfileSpecific);

            // Sort metadata according to grouping order
            var orderedMetadata = new List<IndicatorMetadata>();
            foreach (var indicatorId in groupings.Select(x => x.IndicatorId).Distinct())
            {
                orderedMetadata.Add(metadataList.First(x => x.IndicatorId == indicatorId));
            }

            return new MultipleIndicatorMetadataFileWriter().GetMetadataFileAsBytes(orderedMetadata,
                GetIndicatorMetadataTextProperties());
        }

        private IList<IndicatorMetadataTextProperty> GetIndicatorMetadataTextProperties()
        {
            var properties = _indicatorMetadataProvider.IndicatorMetadataTextProperties;
            return properties;
        }
    }
}