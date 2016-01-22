using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class AreaRankBuilder
    {
        public IGroupDataReader GroupDataReader { get; set; }
        public IAreasReader AreasReader { get; set; }
        public IArea Area { get; set; }

        public AreaRankGrouping BuildRank(Grouping grouping, IndicatorMetadata indicatorMetadata,
            TimePeriod timePeriod, IEnumerable<CoreDataSet> dataList)
        {
            var validDataList = RankDataList.ValidDataList(dataList, grouping.PolarityId);

            if (validDataList.Count == 0)
            {
                return null;
            }

            var min = validDataList.First();
            var minArea = AreasReader.GetAreaFromCode(min.AreaCode);

            var max = validDataList.Last();
            var maxArea = AreasReader.GetAreaFromCode(max.AreaCode);

            CoreDataSet areaData;
            int? rank;
            if (Area.IsCountry)
            {
                areaData = GroupDataReader.GetCoreData(grouping, timePeriod, Area.Code).FirstOrDefault();
                rank = null;
            }
            else if (Area is CategoryArea)
            {
                // Deprivation decile
                var categoryArea = (CategoryArea)Area;
                areaData = GroupDataReader.GetCoreDataForCategoryArea(grouping, timePeriod, categoryArea);
                rank = null;
            }
            else
            {
                areaData = AreaHelper.GetDataForAreaFromDataList(Area.Code, validDataList);
                rank = validDataList.IndexOf(areaData) + 1;
            }

            // Format data
            var formatter = NumericFormatterFactory.New(indicatorMetadata, GroupDataReader);
            formatter.Format(min);
            formatter.Format(max);
            formatter.Format(areaData);

            // Reduce JSON footprint
            var dataProcessor = new ValueDataProcessor(null);
            dataProcessor.Truncate(areaData);
            dataProcessor.Truncate(min);
            dataProcessor.Truncate(max);

            AreaRank areaRank = null;
            if (areaData != null)
            {                
                areaRank = new AreaRank
                {
                    // define Area when required
                    Value = areaData.Value,
                    ValueFormatted = areaData.ValueFormatted,
                    Rank = rank,
                    Count = areaData.Count.Value,
                    Denom = areaData.Denominator
                };
            }

            return new AreaRankGrouping
            {
                TimePeriodText = new TimePeriodTextFormatter(indicatorMetadata).Format(timePeriod),
                AreaRank = areaRank,
                Min = new AreaRank
                {
                    // define Count when required
                    Area = minArea,
                    Value = min.Value,
                    ValueFormatted = min.ValueFormatted,
                    Rank = 1,
                    Count = min.Count.Value
                },
                Max = new AreaRank
                {
                    // define Count when required
                    Area = maxArea,
                    Value = max.Value,
                    ValueFormatted = max.ValueFormatted,
                    Rank = validDataList.Count(),
                    Count = max.Count.Value
                }
            };
        }
    }
}
