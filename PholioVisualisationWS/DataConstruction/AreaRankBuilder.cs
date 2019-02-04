using PholioVisualisation.DataAccess;
using PholioVisualisation.DataSorting;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataConstruction
{
    public class AreaRankBuilder
    {
        private IGroupDataReader _groupDataReader;
        private IAreasReader _areasReader;
        private IPholioLabelReader _pholioLabelReader;
        private INumericFormatterFactory _numericFormatterFactory;

        public AreaRankBuilder(IGroupDataReader groupDataReader, IAreasReader areasReader,
            IPholioLabelReader pholioLabelReader, INumericFormatterFactory numericFormatterFactory)
        {
            _groupDataReader = groupDataReader;
            _areasReader = areasReader;
            _pholioLabelReader = pholioLabelReader;
            _numericFormatterFactory = numericFormatterFactory;
        }

        public AreaRankGrouping BuildRank(IArea area, Grouping grouping, IndicatorMetadata indicatorMetadata,
            TimePeriod timePeriod, IList<CoreDataSet> dataList)
        {
            var polarityId = grouping.PolarityId;
            if (grouping.IndicatorId == IndicatorIds.TotalPrescribedLarc)
            {
                // LARC is BOB even though high is good
                polarityId = PolarityIds.RagHighIsGood;
            }

            var validDataList = RankDataList.ValidDataList(dataList, polarityId);

            if (validDataList.Count == 0)
            {
                return null;
            }

            // Define limits
            var min = validDataList.First();
            var max = validDataList.Last();

            // Define rank and data
            CoreDataSet areaData;
            int? rank;
            if (area.IsCountry || area.IsOnsClusterGroup)
            {
                areaData = _groupDataReader.GetCoreData(grouping, timePeriod, area.Code).FirstOrDefault();
                rank = null;
            }
            else if (area is CategoryArea)
            {
                // Deprivation decile
                var categoryArea = (CategoryArea)area;
                areaData = _groupDataReader.GetCoreDataForCategoryArea(grouping, timePeriod, categoryArea);
                rank = null;
            }
            else if (area is NearestNeighbourArea)
            {
                // Nearest neighbour
                areaData = CoreDataSet.GetNullObject(area.Code); // Average not calculated for neighbours
                rank = null;
            }
            else
            {
                // Define rank for child area
                areaData = new CoreDataSetFilter(validDataList).SelectWithAreaCode(area.Code);
                if (areaData == null)
                {
                    // Data not valid
                    areaData = new CoreDataSetFilter(dataList).SelectWithAreaCode(area.Code);
                    if (areaData == null)
                    {
                        // No data available
                        areaData = CoreDataSet.GetNullObject(area.Code);
                    }
                    rank = null;
                }
                else
                {
                    rank = GetRank(validDataList, areaData);
                }
            }

            FormatAndTruncateData(indicatorMetadata, new List<CoreDataSet> { areaData, min, max });

            AreaRank areaRank = null;
            if (areaData != null)
            {
                areaRank = new AreaRank
                {
                    Area = area,
                    Value = areaData.Value,
                    ValueFormatted = areaData.ValueFormatted,
                    Rank = rank,
                    Count = areaData.Count.Value,
                    Denom = areaData.Denominator,
                    ValueNote = GetValueNote(areaData)
                };
            }

            // Define areas
            var areas = _areasReader.GetAreasFromCodes(new List<string> { min.AreaCode, max.AreaCode });
            var minArea = areas.First(x => x.Code == min.AreaCode);
            var maxArea = areas.First(x => x.Code == max.AreaCode);

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
                    Rank = validDataList.Count,
                    Count = max.Count.Value
                },
                IID = grouping.IndicatorId,
                Sex = grouping.Sex,
                Age = grouping.Age,
                PolarityId = grouping.PolarityId
            };
        }

        private void FormatAndTruncateData(IndicatorMetadata indicatorMetadata, List<CoreDataSet> dataList)
        {
            var formatter = _numericFormatterFactory.New(indicatorMetadata);
            var dataProcessor = new ValueDataProcessor(null);

            foreach (var data in dataList)
            {
                formatter.Format(data);
                dataProcessor.Truncate(data);
            }
        }

        public static int? GetRank(IList<CoreDataSet> validDataList, CoreDataSet areaData)
        {
            var index = validDataList.IndexOf(areaData);

            while (index > 0)
            {
                // If preceeding item has the same rank the use that instead
                if (areaData.Value.Equals(validDataList[index - 1].Value))
                {
                    index--;
                }
                else
                {
                    break;
                }
            }

            int? rank = index + 1;
            return rank;
        }


        private ValueNote GetValueNote(CoreDataSet data)
        {
            return new ValueNote
            {
                Id = data.ValueNoteId,
                Text = _pholioLabelReader.LookUpValueNoteLabel(data.ValueNoteId)
            };
        }
    }
}
