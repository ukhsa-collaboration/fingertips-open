using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class TimePeriodTextFormatter
    {
        private int yearTypeId;

        public TimePeriodTextFormatter(IndicatorMetadata metadata)
        {
            yearTypeId = metadata.YearTypeId;
        }

        /// <summary>
        /// Returns a formatted time period.
        /// </summary>
        /// <returns>Formatted time period</returns>
        public string Format(TimePeriod timePeriod)
        {
            return TimePeriodFormatter.GetTimePeriodString(timePeriod, yearTypeId);
        }

    }
}
