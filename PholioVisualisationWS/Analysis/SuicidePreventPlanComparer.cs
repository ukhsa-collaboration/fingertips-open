using System;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class SuicidePreventPlanComparer: IndicatorComparer
    {
        public override Significance Compare(CoreDataSet data, CoreDataSet comparator, IndicatorMetadata metadata)
        {
            if (data.IsValueValid)
            {
               return (Significance)Convert.ToInt32(data.Value);
            }
            return Significance.None;
        }
    }
}