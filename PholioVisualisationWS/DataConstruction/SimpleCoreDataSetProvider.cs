using System;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class SimpleCoreDataSetProvider : CoreDataSetProvider
    {
        private IGroupDataReader groupDataReader;

        public SimpleCoreDataSetProvider(Area area, IGroupDataReader groupDataReader) : base(area)
        {
            this.groupDataReader = groupDataReader;
        }

        public override CoreDataSet GetData(Grouping grouping, TimePeriod timePeriod, IndicatorMetadata indicatorMetadata)
        {
            return groupDataReader.GetCoreData(grouping, timePeriod, area.Code).FirstOrDefault();
        }
    }
}