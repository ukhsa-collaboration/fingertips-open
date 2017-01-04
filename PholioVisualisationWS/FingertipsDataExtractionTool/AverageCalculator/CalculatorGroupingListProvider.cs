using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace FingertipsDataExtractionTool.AverageCalculator
{
    public class CalculatorGroupingListProvider
    {
        private IGroupDataReader _groupReader;

        public CalculatorGroupingListProvider(IGroupDataReader groupReader)
        {
            _groupReader = groupReader;
        }

        /// <summary>
        /// Gets groupings that represent every available combination of indicator ID, age ID, sex ID and area type ID.
        /// </summary>
        public virtual IList<Grouping> GetGroupings()
        {
            var groupingList = new List<Grouping>();

            var indicators = _groupReader.GetAllIndicators();

            foreach (var indicator in indicators)
            {
                var groupedGroupings = _groupReader.GetGroupingsByIndicatorId(indicator).GroupBy(x => new
                {
                    x.AreaTypeId, x.Sex, x.Age
                });

                foreach (var group in groupedGroupings)
                {
                    // Add grouping with the widest available range of time periods
                    var latest = group.OrderByDescending(x => x.DataPointYear).First();
                    var earliest = group.OrderBy(x => x.BaselineYear).First();
                    latest.BaselineYear = earliest.BaselineYear;
                    groupingList.Add(latest);
                }
            }

            return groupingList;
        }
    }
}
