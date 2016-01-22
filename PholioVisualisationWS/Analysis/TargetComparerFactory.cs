using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class TargetComparerFactory
    {
        public static TargetComparer New(TargetConfig targetConfig)
        {
            if (targetConfig == null)
            {
                return null;
            }

            if (targetConfig.LowerLimit == null && 
                targetConfig.UpperLimit == null && 
                targetConfig.BespokeTargetKey == null)
            {
                throw new FingertipsException(
                    "The configuration details for this target (ID:" + targetConfig.Id + ") has not been set");
            }

            var bespokeKey = targetConfig.BespokeTargetKey;
            if (string.IsNullOrEmpty(bespokeKey) == false)
            {
                switch (bespokeKey)
                {
                    case BespokeTargets.ComparedWithPreviousYearEnglandValue:
                        return new BespokeTargetPreviousYearEnglandValueComparer(targetConfig);
                    case BespokeTargets.TargetPercentileRange:
                        return new BespokeTargetPercentileRangeComparer(targetConfig);
                }

                throw new FingertipsException(
                    "The bespoke target key was not recognised: " + bespokeKey);
            }

            if (targetConfig.UpperLimit.HasValue)
            {
                return new RangeTargetComparer(targetConfig);
            }
            
            if (targetConfig.LowerLimit.HasValue)
            {
                return new SingleValueTargetComparer(targetConfig);
            }

            throw new FingertipsException("Bespoke comparison not implemented");
        }
    }
}
