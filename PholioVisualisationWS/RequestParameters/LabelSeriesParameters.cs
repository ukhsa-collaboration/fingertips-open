
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace PholioVisualisation.RequestParameters
{
    public class LabelSeriesParameters : BaseParameters
    {
        public const string ParameterSeries = "id";

        public string SeriesId { get; set; }

        public LabelSeriesParameters(NameValueCollection parameters)
            : base(parameters)
        {
            SeriesId = ParseString(ParameterSeries);
        }

    }
}
