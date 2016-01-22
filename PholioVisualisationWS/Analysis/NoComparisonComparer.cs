using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class NoComparisonComparer : IndicatorComparer
    {
        public override Significance Compare(CoreDataSet data, CoreDataSet comparator, IndicatorMetadata metadata)
        {
            return Significance.None;
        }

    }
}
