using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class RangeTargetComparer : TargetComparer
    {
        public RangeTargetComparer(TargetConfig config)
            : base(config)
        {
            ValidateConfig();
            PolarityId = config.PolarityId;
        }

        private void ValidateConfig()
        {
            var config = Config;

            if (config.LowerLimit > config.UpperLimit)
            {
                throw new FingertipsException("Lower limit cannot be greater than upper limit");
            }
        }

        public override Significance CompareAgainstTarget(CoreDataSet data)
        {
            if (CanComparisonGoAhead(data) == false)
            {
                return Significance.None;
            }

            var val = data.Value;

            Significance significance = Significance.Same;

            if (PolarityId == PolarityIds.RagLowIsGood)
            {
                if (val < Config.LowerLimit)
                {
                    significance = Significance.Better;
                }
                else if (val >= Config.UpperLimit)
                {
                    significance = Significance.Worse;
                }
            }
            else
            {
                if (val < Config.LowerLimit)
                {
                    significance = Significance.Worse;
                }
                else if (val >= Config.UpperLimit)
                {
                    significance = Significance.Better;
                }
            }

            return significance;
        }
    }
}
