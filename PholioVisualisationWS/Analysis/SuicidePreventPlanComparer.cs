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
                switch (Convert.ToInt32(data.Value))
                {
                    case SuicidePlanStatus.Exists:
                        return Significance.Better;
                    case SuicidePlanStatus.None:
                        return Significance.Worse;
                    default:
                        return Significance.Same;
                }
            }
            return Significance.None;
        }
    }
}