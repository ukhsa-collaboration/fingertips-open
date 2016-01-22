using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class TimePeriodTextListBuilderFactory
    {
        public static ITimePeriodTextListBuilder New(bool doNotUseNullBuilder, IndicatorMetadata metadata)
        {
            if (doNotUseNullBuilder)
            {
                return new TimePeriodTextListBuilder(metadata);
            }

            return new NullTimePeriodTextListBuilder();
        }

    }
}
