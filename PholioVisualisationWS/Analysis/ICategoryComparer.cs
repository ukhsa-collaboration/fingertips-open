using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public interface ICategoryComparer
    {
        void SetDataForCategories(IList<double> validValues);
        int GetCategory(CoreDataSet data);
    }
}