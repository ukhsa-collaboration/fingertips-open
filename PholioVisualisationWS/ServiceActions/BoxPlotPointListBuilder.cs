using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class BoxPlotPointListBuilder
    {
        private readonly SingleGroupingProvider _singleGroupingProvider;

        public BoxPlotPointListBuilder(SingleGroupingProvider singleGroupingProvider)
        {
            _singleGroupingProvider = singleGroupingProvider;
        }

        public IList<IndicatorStats> GetBoxPlotPoints(GroupingDifferentiator groupingDifferentiator,
            ParentArea parentArea, int profileId, IndicatorMetadata indicatorMetadata)
        {
            var boxPlotPoints = new List<IndicatorStats>();

            var grouping = GetGrouping(groupingDifferentiator, parentArea, profileId);
            var timePeriods = TimePeriodIterator.TimePeriodsFromGrouping(grouping, indicatorMetadata.YearType);

            var indicatorStatsBuilder = new IndicatorStatsBuilder();

            foreach (var timePeriod in timePeriods)
            {
               var indicatorStats = indicatorStatsBuilder.GetIndicatorStats(timePeriod, grouping,
                    indicatorMetadata, parentArea, profileId);
                boxPlotPoints.Add(indicatorStats);
            }

            return boxPlotPoints;
        }

        private Grouping GetGrouping(GroupingDifferentiator groupingDifferentiator, ParentArea parentArea, int profileId)
        {
            Grouping grouping;
            if (profileId == ProfileIds.Undefined || profileId == ProfileIds.Search)
            {
                // Grouping for any profile
                grouping = _singleGroupingProvider
                    .GetGroupingWithLatestDataPointForAnyProfile(groupingDifferentiator, parentArea.ChildAreaTypeId);
            }
            else
            {
                // Grouping for specific profile
                grouping = _singleGroupingProvider.GetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(profileId,
                    parentArea.ChildAreaTypeId, groupingDifferentiator);
            }

            return grouping;
        }
    }
}
