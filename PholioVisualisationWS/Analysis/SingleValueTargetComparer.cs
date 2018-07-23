using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class SingleValueTargetComparer : TargetComparer
    {
        private double? _limit;

        public SingleValueTargetComparer(TargetConfig config)
            : base(config)
        {
            PolarityId = config.PolarityId;
            _limit = config.LowerLimit;
        }

        public override Significance CompareAgainstTarget(CoreDataSet data)
        {
            if (CanComparisonGoAhead(data) == false)
            {
                return Significance.None;
            }

            Significance significance;

            if (Config.UseCIsForLimitComparison)
            {
                significance = CompareCIsToLowerLimit(data);
            }
            else
            {
                significance = CompareValueToLowerLimit(data);
            }

            return significance;
        }

        private Significance CompareValueToLowerLimit(CoreDataSet data)
        {
            Significance significance = Significance.Worse;
            var val = data.Value;
            if (PolarityId == PolarityIds.RagLowIsGood)
            {
                if (val <= _limit)
                {
                    significance = Significance.Better;
                }
            }
            else
            {
                if (val >= _limit)
                {
                    significance = Significance.Better;
                }
            }
            return significance;
        }

        private Significance CompareCIsToLowerLimit(CoreDataSet data)
        {
            if (data.Are95CIsValid == false) return Significance.None;

            Significance significance;
            var lowerCI = data.LowerCI95;
            var upperCI = data.UpperCI95;

            if (lowerCI >= _limit)
            {
                significance = PolarityId == PolarityIds.RagLowIsGood
                ? Significance.Worse
                : Significance.Better;

            }
            else if (upperCI < _limit)
            {
                significance = PolarityId == PolarityIds.RagLowIsGood
                ? Significance.Better
                : Significance.Worse;
            }
            else
            {
                significance = Significance.Same;
            }

            return significance;
        }
    }
}
