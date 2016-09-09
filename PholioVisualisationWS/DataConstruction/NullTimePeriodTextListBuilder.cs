using System;
using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    /// Does nothing with added TimePeriods.
    /// </summary>
    public class NullTimePeriodTextListBuilder : ITimePeriodTextListBuilder
    {
        /// <summary>
        /// Time periods are ignored
        /// </summary>
        public void Add(TimePeriod timePeriod) { }

        /// <summary>
        /// Returns null.
        /// </summary>
        public IList<string> GetTimePeriodStrings()
        {
            return null;
        }

        /// <summary>
        /// Time periods are ignored
        /// </summary>
        public void AddRange(IEnumerable<TimePeriod> timePeriods)
        {
        }
    }
}
