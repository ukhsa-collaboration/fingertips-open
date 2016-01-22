
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Formatting
{
    public class GroupRootFormatter
    {
        public void Format(GroupRoot groupRoot, IndicatorMetadata metadata, TimePeriodFormatter timePeriodFormatter, NumericFormatter formatter)
        {
            foreach (CoreDataSet coreData in groupRoot.Data)
            {
                formatter.Format(coreData);
                formatter.FormatConfidenceIntervals(coreData);
            }

            foreach (var grouping in groupRoot.Grouping)
            {
                formatter.Format(grouping.ComparatorData);
                formatter.FormatConfidenceIntervals(grouping.ComparatorData);
                timePeriodFormatter.Format(grouping, metadata);
            }
        }



    }
}
