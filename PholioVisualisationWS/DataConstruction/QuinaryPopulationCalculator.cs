using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class QuinaryPopulationCalculator
    {
        private readonly IGroupDataReader _groupDataReader;
        private readonly IAreasReader _areasReader;

        private QuinaryPopulation population;

        public QuinaryPopulationCalculator(IGroupDataReader groupDataReader, IAreasReader areasReader)
        {
            _groupDataReader = groupDataReader;
            _areasReader = areasReader;
        }

        public QuinaryPopulation CalculateParentPopulationValues(string parentAreaCode, int areaTypeId, int indicatorId, int sexId, TimePeriod period)
        {
            population = new QuinaryPopulation();
            population.Values = new List<QuinaryPopulationValue>();

            var grouping = new Grouping
            {
                IndicatorId = indicatorId,
                AreaTypeId = areaTypeId,
                SexId = sexId
            };

            if (parentAreaCode == AreaCodes.England)
            {
                SetValueTotalsForAllChildAreas(grouping, indicatorId, period);
            }
            else
            {
                var childAreas = GetChildAreas(parentAreaCode, areaTypeId);
                SetValueTotalsForChildAreas(grouping, indicatorId, period, childAreas);
                population.ChildAreaCount = childAreas.Count;
            }

            return population;
        }

        private IList<IArea> GetChildAreas(string parentAreaCode, int areaTypeId)
        {
            var childAreasList = new ChildAreaListBuilder(_areasReader).GetChildAreas(parentAreaCode, areaTypeId);

            if (NearestNeighbourArea.IsNearestNeighbourAreaCode(parentAreaCode))
            {
                // Add code for area with neighbours
                var childAreaCode = NearestNeighbourArea.GetAreaCodeFromNeighbourAreaCode(parentAreaCode);
                var childArea = AreaFactory.NewArea(_areasReader, childAreaCode);
                childAreasList.Add(childArea);
            }

            return childAreasList;
        }

        private void SetValueTotalsForAllChildAreas(Grouping grouping, int indicatorId, TimePeriod period)
        {
            var ageIds = GetAgeIds(indicatorId);

            foreach (var ageId in ageIds)
            {
                grouping.AgeId = ageId;

                var values = _groupDataReader.GetCoreDataForAllAreasOfType(grouping, period)
                    .Where(x => x.IsValueValid)
                    .Select(x => x.Value)
                    .ToList();

                if (population.ChildAreaCount == 0)
                {
                    population.ChildAreaCount = values.Count;
                }

                AddPopulationValue(ageId, values);
            }

        }

        private void SetValueTotalsForChildAreas(Grouping grouping, int indicatorId, TimePeriod period, IEnumerable<IArea> childAreas)
        {
            var ageIds = GetAgeIds(indicatorId);
            var areaCodes = childAreas.Select(x => x.Code).ToArray();

            foreach (var ageId in ageIds)
            {
                grouping.AgeId = ageId;

                var values = _groupDataReader.GetCoreData(grouping, period, areaCodes)
                    .Where(x => x.IsValueValid)
                    .Select(x => x.Value);

                AddPopulationValue(ageId, values);
            }
        }

        private void AddPopulationValue(int ageId, IEnumerable<double> values)
        {
            var value = new QuinaryPopulationValue
            {
                AgeId = ageId,
                Value = values.Sum()
            };

            population.Values.Add(value);
        }

        private IList<int> GetAgeIds(int indicatorId)
        {
            var ageIds = _groupDataReader.GetAllAgeIdsForIndicator(indicatorId);

            if (ageIds.Contains(AgeIds.Over95))
            {
                return QuinaryPopulationSorter.GetAgeIdsToOver95();
            }

            return QuinaryPopulationSorter.GetAgeIdsToOver90();
        }
    }
}