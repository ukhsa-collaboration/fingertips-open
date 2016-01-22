using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace PholioVisualisation.RequestParameters
{
    public class PracticeChartParameters : ChartParameters
    {
        public const string ParameterGroupId1 = "gid1";
        public const string ParameterGroupId2 = "gid2";
        public const string ParameterIndicatorId1 = "iid1";
        public const string ParameterIndicatorId2 = "iid2";
        public const string ParameterSexId1 = "sex1";
        public const string ParameterSexId2 = "sex2";
        public const string ParameterAgeId1 = "age1";
        public const string ParameterAgeId2 = "age2";
        public const string ParameterDataPointOffset = IndicatorStatsParameters.ParameterDataPointOffset;
        public const string ParameterParentArea = ParameterNames.ParentAreaCode;

        public int GroupId1 { get; set; }
        public int GroupId2 { get; set; }
        public string AreaCode { get; set; }
        public string ParentAreaCode { get; set; }
        public int DataPointOffset { get; set; }
        public int IndicatorId1 { get; set; }
        public int IndicatorId2 { get; set; }
        public int SexId1 { get; set; }
        public int SexId2 { get; set; }
        public int AgeId1 { get; set; }
        public int AgeId2 { get; set; }

        public PracticeChartParameters(NameValueCollection parameters)
            : base(parameters)
        {
            AreaCode = ParseString(ParameterNames.AreaCode);
            ParentAreaCode = ParseString(ParameterParentArea);
            GroupId1 = ParseInt(ParameterGroupId1);
            GroupId2 = ParseInt(ParameterGroupId2);
            IndicatorId1 = ParseInt(ParameterIndicatorId1);
            IndicatorId2 = ParseInt(ParameterIndicatorId2);
            SexId1 = ParseInt(ParameterSexId1);
            SexId2 = ParseInt(ParameterSexId2);
            AgeId1 = ParseInt(ParameterAgeId1);
            AgeId2 = ParseInt(ParameterAgeId2);
            DataPointOffset = ParseInt(ParameterDataPointOffset);
        }

        /// <summary>
        /// Used to uniquely identify the current parameter values.
        /// </summary>
        /// <returns></returns>
        public string GetCacheKey()
        {
            var bits = new string[] {AreaCode, 
                   ParentAreaCode,
                   GroupId1.ToString(),
                   GroupId2.ToString(),
                   IndicatorId1.ToString(),
                   IndicatorId2.ToString(),
                   SexId1.ToString(),
                   SexId2.ToString(),
                   AgeId1.ToString(),
                   AgeId2.ToString(),
                   DataPointOffset.ToString(),
                   Height.ToString(),
                   Width.ToString()};

            return String.Join("-", bits);
        }

    }
}
