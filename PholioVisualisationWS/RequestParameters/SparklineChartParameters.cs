
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.RequestParameters
{
    public class SparklineChartParameters : ChartParameters
    {
        protected static readonly char[] DataSeparator = new[] { ';' };

        public const string ParameterYMin = "min";
        public const string ParameterYMax = "max";

        public double YMin { get; set; }
        public double YMax { get; set; }

        public SparklineChartParameters(NameValueCollection parameters)
            : base(parameters)
        {
            YMax = ParseDouble(ParameterYMax);
            YMin = ParseDouble(ParameterYMin);
        }

        protected IList<ValueWithCIsData> ParseData(string name)
        {
            //TODO pull this out into class and unit test
            string dataString = Parameters[name];

            List<ValueWithCIsData> dataList = new List<ValueWithCIsData>();

            string[] points = dataString.Split(DataSeparator, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pointListString/*e.g. "23,14,27" */ in points)
            {
                dataList.Add(ValueWithCIsData.Parse(pointListString));
            }

            return dataList;
        }

    }
}
