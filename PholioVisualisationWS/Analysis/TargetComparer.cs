using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public abstract class TargetComparer : IndicatorComparer
    {
        protected TargetConfig Config;
        
        protected TargetComparer(TargetConfig config)
        {
            Config = config;
        }

        public override Significance Compare(CoreDataSet data, CoreDataSet comparator, IndicatorMetadata metadata)
        {
            throw new FingertipsException("Use CompareAgainstTarget instead");
        }

        public abstract Significance CompareAgainstTarget(CoreDataSet data);

        protected bool CanComparisonGoAhead(ValueWithCIsData data)
        {
            return IsPolarityValid() && IsDataValid(data);
        }

        /// <summary>
        /// Helper method to set the target significance. Only used to reduce duplicated code.
        /// </summary>
        public static void AddTargetSignificance(CoreDataSet data, TargetComparer targetComparer)
        {
            if (data != null && targetComparer != null)
            {
                Significance significance = targetComparer.CompareAgainstTarget(data);
                data.AddSignificance(ComparatorIds.Target, significance);
            }
        }
    }
}
