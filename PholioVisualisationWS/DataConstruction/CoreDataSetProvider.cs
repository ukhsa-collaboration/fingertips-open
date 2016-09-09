
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public abstract class CoreDataSetProvider
    {
        protected IArea Area;

        protected CoreDataSetProvider(IArea area)
        {
            Area = area;
        }

        public abstract CoreDataSet GetData(Grouping grouping, TimePeriod timePeriod,
            IndicatorMetadata indicatorMetadata);
    }
}