using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class SingleValueTargetComparer : TargetComparer
    {
        public SingleValueTargetComparer(TargetConfig config)
            : base(config)
        {
            PolarityId = config.PolarityId;
        }

        public override Significance CompareAgainstTarget(CoreDataSet data)
        {
            if (CanComparisonGoAhead(data) == false)
            {
                return Significance.None;
            }

            var val = data.Value;
            Significance significance = Significance.Worse;

            if (PolarityId == PolarityIds.RagLowIsGood)
            {
                if (val <= Config.LowerLimit)
                {
                    significance = Significance.Better;
                }
            }
            else
            {
                if (val >= Config.LowerLimit)
                {
                    significance = Significance.Better;
                }
            }

            return significance;
        }
    }
}
