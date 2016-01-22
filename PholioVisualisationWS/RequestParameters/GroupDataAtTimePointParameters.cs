
using System.Collections.Specialized;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.RequestParameters
{
    public class GroupDataAtTimePointParameters : GroupDataWithGroupingsParameters
    {
        public const string ParameterYear = "year";
        public const string ParameterYearRange = "yearrange";

        public TimePeriod TimePeriod { get; set; }

        public GroupDataAtTimePointParameters(NameValueCollection parameters)
            : base(parameters)
        {
            int year = ParseInt(ParameterYear);
            if (year < 1900 || year > 2100)
            {
                year = -1;
            }

            int yearRange = ParseInt(ParameterYearRange);
            if (yearRange < 0 || yearRange > 100)
            {
                yearRange = -1;
            }

            TimePeriod = new TimePeriod { Year = year, YearRange = yearRange };
        }

        public override bool AreValid
        {
            get
            {
                return base.AreValid && TimePeriod.Year > 0 && TimePeriod.YearRange > 0;
            }
        }
    }
}
