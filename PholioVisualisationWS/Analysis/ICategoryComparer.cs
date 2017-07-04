using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public interface ICategoryComparer
    {
        /// <summary>
        /// Set the values of the child areas
        /// </summary>
        void SetDataForCategories(IList<double> validValues);

        /// <summary>
        /// Gets the significance within the category bounds for the data
        /// </summary>
        int GetCategory(CoreDataSet data);

        /// <summary>
        /// Creates a new instance of the category comparer
        /// </summary>
        ICategoryComparer NewInstance();
    }
}