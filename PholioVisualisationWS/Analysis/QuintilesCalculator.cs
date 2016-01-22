using System.Collections.Generic;

namespace PholioVisualisation.Analysis
{
    public class QuintilesCalculator : ValueCategoriesCalculator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="validValuesList">A list of valid values</param>
        public QuintilesCalculator(IEnumerable<double> validValuesList)
            : base(validValuesList) { }

        public IList<double> Bounds
        {
            get
            {
                return GetCategoryBounds(5);
            }
        }
    }
}
