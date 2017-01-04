using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class SubnationalAreaAverageCalculator
    {
        private IGroupDataReader _groupDataReader;
        private ChildAreaListBuilder _childAreaListBuilder;

        public SubnationalAreaAverageCalculator(IGroupDataReader groupDataReader, ChildAreaListBuilder childAreaListBuilder)
        {
            _groupDataReader = groupDataReader;
            _childAreaListBuilder = childAreaListBuilder;
        }

        public CoreDataSet CalculateAverage(Grouping grouping, TimePeriod timePeriod, 
            IndicatorMetadata metadata, IArea subnationalArea)
        {
            var childAreas = _childAreaListBuilder.GetChildAreas(subnationalArea.Code, 
                grouping.AreaTypeId);

            var childDataList = _groupDataReader.GetCoreData(grouping, timePeriod,
                childAreas.Select(x => x.Code).ToArray());

            var averageCalculator = AverageCalculatorFactory.New(childDataList, metadata);
            return averageCalculator.Average;
        }

    }
}