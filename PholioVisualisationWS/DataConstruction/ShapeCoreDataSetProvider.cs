using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class ShapeCoreDataSetProvider : CoreDataSetProvider
    {
        private PracticeDataAccess practiceDataAccess;

        public ShapeCoreDataSetProvider(Area area, PracticeDataAccess practiceDataAccess)
            : base(area)
        {
            this.practiceDataAccess = practiceDataAccess;
        }

        public override CoreDataSet GetData(Grouping grouping, TimePeriod timePeriod, IndicatorMetadata indicatorMetadata)
        {
            double val = practiceDataAccess.GetPracticeAggregateDataValue(grouping, timePeriod, area.Code);

            return val.Equals(ValueData.NullValue) ?
                null :
                new CoreDataSet { Value = val };
        }

    }
}