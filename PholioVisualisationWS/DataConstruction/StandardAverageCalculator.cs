using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class StandardAverageCalculator : AverageCalculator
    {
        private readonly CoreDataSetFilter _coreDataSetFilter;
        private readonly Unit _unit;

        public StandardAverageCalculator(CoreDataSetFilter coreDataSetFilter, Unit unit)
        {
            _coreDataSetFilter = coreDataSetFilter;
            _unit = unit;
        }

        public override CoreDataSet Average
        {
            get
            {
                var dataList = _coreDataSetFilter.SelectWhereCountAndDenominatorAreValid().ToList();

                // Do not calculate average if only have one value
                if (dataList.Count > 1)
                {
                    return CalculateAverage(dataList, _unit);
                }

                return null;
            }
        }

        public static CoreDataSet CalculateAverage(IList<CoreDataSet> validData, Unit unit)
        {
            var average = CoreDataSet.GetNullObject();
            average.Count = validData.Sum(x => x.Count.Value);
            average.Denominator = validData.Sum(x => x.Denominator);
            average.Value = (average.Count.Value / average.Denominator) * unit.Value;

            average.ValueNoteId = ValueNoteIds.ValueAggregatedFromAllKnownGeographyValues;
            return average;
        }
    }
}