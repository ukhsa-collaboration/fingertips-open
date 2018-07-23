using System;

namespace PholioVisualisation.PholioObjects
{
    public class IndicatorDateChange
    {
        public bool HasDataChangedRecently { get; set; }
        public DateTime? DateOfLastChange { get; set; }

        public static IndicatorDateChange GetNoChange()
        {
            return new IndicatorDateChange
            {
                HasDataChangedRecently = false,
                DateOfLastChange = null
            };
        }
    }
}
