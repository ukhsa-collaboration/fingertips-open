using System;
using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public interface ITimePeriodTextListBuilder
    {
        void Add(TimePeriod timePeriod);
        void AddRange(IEnumerable<TimePeriod> timePeriods);
        IList<string> GetTimePeriodStrings();
    }
}
