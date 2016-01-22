using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GpDeprivationDecileCoreDataSetProvider : CoreDataSetProvider
    {
        private CategoryArea categoryArea;
        private PracticeDataAccess practiceDataAccess;

        public GpDeprivationDecileCoreDataSetProvider(CategoryArea categoryArea, PracticeDataAccess practiceDataAccess)
            : base(categoryArea)
        {
            this.categoryArea = categoryArea;
            this.practiceDataAccess = practiceDataAccess;
        }

        public override CoreDataSet GetData(Grouping grouping, TimePeriod timePeriod, IndicatorMetadata indicatorMetadata)
        {
            double val = practiceDataAccess.GetGpDeprivationDecileDataValue(grouping, timePeriod, categoryArea);

            return val.Equals(ValueData.NullValue) ?
                null :
                new CoreDataSet { Value = val };
        }

    }
}