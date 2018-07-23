using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class QuartilesComparer : IndicatorComparer, ICategoryComparer
    {
        private IList<double> _bounds;
        private int _binCount = 4;

        public ICategoryComparer NewInstance()
        {
            return new QuartilesComparer();
        }

        public override Significance Compare(CoreDataSet data, CoreDataSet comparator, IndicatorMetadata metadata)
        {
            throw new FingertipsException("Use GetCategory instead of Compare for " + GetType().Name);
        }

        public void SetDataForCategories(IList<double> validValues)
        {
            if (validValues != null && validValues.Count >= _binCount)
            {
                _bounds = new QuartilesCalculator(validValues).Bounds;
            }
        }

        public int GetCategory(CoreDataSet data)
        {
            if (_bounds == null || data == null || data.IsValueValid == false)
            {
                return 0;
            }

            int boundsIndex = 1;
            while (boundsIndex <= _binCount)
            {
                if (data.Value <= _bounds[boundsIndex])
                {
                    return boundsIndex;
                }
                boundsIndex++;
            }

            throw new FingertipsException("Value does not fall within expected boundaries: " +
                data.AreaCode);
        }
    }
}
