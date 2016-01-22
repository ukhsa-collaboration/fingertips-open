using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class CcgCoreDataSetProvider : CoreDataSetProvider
    {
        private CoreDataSetListProvider coreDataSetListProvider;
        private CcgPopulationProvider ccgPopulationProvider;
        private IGroupDataReader groupDataReader;

        public CcgCoreDataSetProvider(Area area, CcgPopulationProvider ccgPopulationProvider,
            CoreDataSetListProvider coreDataSetListProvider, IGroupDataReader groupDataReader)
            : base(area)
        {
            this.ccgPopulationProvider = ccgPopulationProvider;
            this.coreDataSetListProvider = coreDataSetListProvider;
            this.groupDataReader = groupDataReader;
        }

        public override CoreDataSet GetData(Grouping grouping, TimePeriod timePeriod, IndicatorMetadata indicatorMetadata)
        {
            return GetDataFromDatabase(grouping, timePeriod) ??
                CalculateData(grouping, timePeriod, indicatorMetadata);
        }

        private CoreDataSet GetDataFromDatabase(Grouping grouping, TimePeriod timePeriod)
        {
            return groupDataReader.GetCoreData(grouping, timePeriod, area.Code).FirstOrDefault();
        }

        private CoreDataSet CalculateData(Grouping grouping, TimePeriod timePeriod, IndicatorMetadata indicatorMetadata)
        {
            if (RuleShouldCcgAverageBeCalculatedForGroup.Validates(grouping))
            {
                var dataList = coreDataSetListProvider.GetChildAreaData(grouping, area, timePeriod);

                if (dataList.Any())
                {
                    var population = ccgPopulationProvider.GetPopulation(area.Code);
                    return new CcgAverageCalculator(dataList, population, indicatorMetadata).Average;
                }
            }
            return null;
        }
    }
}