using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class QuintilesComparer : IndicatorComparer, ICategoryComparer
    {
        private IList<double> _bounds;
        private int _binCount = 5;

        public ICategoryComparer NewInstance()
        {
            return new QuintilesComparer(); 
        }

        public override Significance Compare(CoreDataSet data, CoreDataSet comparator, IndicatorMetadata metadata)
        {
            throw new FingertipsException("Use GetCategory instead of Compare for " + GetType().Name);
        }

        public void SetDataForCategories(IList<double> validValues)
        {
            if (validValues != null && validValues.Count >= _binCount)
            {
                _bounds = new QuintilesCalculator(validValues).Bounds;
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
                    if (PolarityId == PolarityIds.RagHighIsGood)
                    {
                        switch (boundsIndex)
                        {
                            case 1:
                                return 5;
                            case 2:
                                return 4;
                            case 3:
                                return 3;
                            case 4:
                                return 2;
                            case 5:
                                return 1;
                        }
                    }

                    return boundsIndex;
                }

                boundsIndex++;
            }

            throw new FingertipsException("Value does not fall within expected boundaries: " +
                data.AreaCode);
        }
    }
}
