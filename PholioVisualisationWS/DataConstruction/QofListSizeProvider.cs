using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class QofListSizeProvider
    {
        public const int IndicatorId = IndicatorIds.QofListSize;

        /// <summary>
        /// Will be average of child practices if area is not a practice.
        /// </summary>
        public double? Value { get; set; }

        public QofListSizeProvider(IGroupDataReader groupDataReader, IArea area, int groupId, int dataPointOffset,
            YearType yearType)
        {
            Grouping grouping = groupDataReader.GetGroupingsByGroupIdAndIndicatorId(groupId, IndicatorId);
            if (grouping != null)
            {
                var period = new DataPointOffsetCalculator(grouping, dataPointOffset, yearType).TimePeriod;

                if (area.IsCcg)
                {
                    //Note: zeroes may occur and should be included
                    var coreDataSetList = groupDataReader
                        .GetCoreDataListForChildrenOfArea(grouping, period, area.Code);
                    Value = 0;

                    if (coreDataSetList.Count > 0)
                    {
                        Value = coreDataSetList
                            .Where(x => x.IsValueValid)
                            .Average(x => x.Value);
                    }
                }
                else
                {
                    SetSingleAreaValue(groupDataReader, area, grouping, period);
                }
            }
        }

        private void SetSingleAreaValue(IGroupDataReader groupDataReader, IArea area, Grouping grouping, TimePeriod period)
        {
            IList<CoreDataSet> dataList = groupDataReader.GetCoreData(grouping, period, area.Code);
            if (dataList.Count == 1)
            {
                var data = dataList.First();
                if (data.IsValueValid)
                {

                    if (area.IsGpPractice)
                    {
                        Value = data.Value;
                    }
                    else
                    {
                        // Calculate average
                        var areasReader = ReaderFactory.GetAreasReader();

                        int count = area.IsCountry ?
                            areasReader.GetAreaCountForAreaType(grouping.AreaTypeId) :
                            areasReader.GetChildAreaCount(area.Code, grouping.AreaTypeId);

                        if (count > 0)
                        {
                            Value = data.Value / count;
                        }
                    }
                }
            }
            else if (dataList.Count > 1)
            {
                throw new FingertipsException(string.Format(
                    "More data than expected on determination of QOF list size: area:{0}", area.Code));
            }
        }
    }
}
