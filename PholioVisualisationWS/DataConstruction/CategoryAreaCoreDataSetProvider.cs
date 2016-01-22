using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class CategoryAreaCoreDataSetProvider : CoreDataSetProvider
    {
        private IGroupDataReader groupDataReader;

        public CategoryAreaCoreDataSetProvider(CategoryArea area, IGroupDataReader groupDataReader) : base(area)
        {
            this.groupDataReader = groupDataReader;
        }

        public override CoreDataSet GetData(Grouping grouping, TimePeriod timePeriod, IndicatorMetadata indicatorMetadata)
        {
            return groupDataReader.GetCoreDataForCategoryArea(grouping, timePeriod, (CategoryArea)area);
        }
    }
}