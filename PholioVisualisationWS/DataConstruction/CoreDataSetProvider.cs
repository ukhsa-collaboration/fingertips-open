
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public abstract class CoreDataSetProvider
    {
        protected IArea area;

        protected CoreDataSetProvider(IArea area)
        {
            this.area = area;
        }

        public abstract CoreDataSet GetData(Grouping grouping, TimePeriod timePeriod,
            IndicatorMetadata indicatorMetadata);
    }
}