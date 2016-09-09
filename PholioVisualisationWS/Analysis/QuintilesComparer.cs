using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class QuintilesComparer : IndicatorComparer, ICategoryComparer
    {
        private IList<double> bounds;

        public override Significance Compare(CoreDataSet data, CoreDataSet comparator, IndicatorMetadata metadata)
        {
            throw new FingertipsException("Use GetCategory instead of Compare for QuintilesComparer");
        }

        public void SetDataForCategories(IList<double> validValues)
        {
            if (validValues != null && validValues.Count >= 5)
            {
                bounds = new QuintilesCalculator(validValues).Bounds;
            }
        }

        public int GetCategory(CoreDataSet data)
        {
            if (bounds == null || data == null || data.IsValueValid == false)
            {
                return 0;
            }

            int boundsIndex = 1;
            while (boundsIndex <= 5)
            {
                if (data.Value <= bounds[boundsIndex])
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
