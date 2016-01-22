using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class AreaValuesParameters : DomainParameters
    {
        public const string ParameterDataPointOffset = IndicatorStatsParameters.ParameterDataPointOffset;

        public int AreaTypeId { get; set; }
        public string ParentAreaCode { get; set; }
        public int PracticeYear { get; set; }
        public int DataPointOffset { get; set; }
        public int IndicatorId { get; set; }
        public int SexId { get; set; }
        public int AgeId { get; set; }
        public int ComparatorId { get; set; }
        public int ProfileId { get; set; }
        public int RestrictToProfileId { get; set; }
        public string Key { get; set; }

        public AreaValuesParameters(NameValueCollection parameters)
            : base(parameters)
        {
            AreaTypeId = ParseInt(ParameterNames.AreaTypeId);
            ParentAreaCode = ParseString(ParameterNames.ParentAreaCode);
            PracticeYear = ParseInt(ParameterNames.PracticeYear);
            DataPointOffset = ParseInt(ParameterDataPointOffset);
            IndicatorId = ParseInt(ParameterNames.IndicatorId);
            SexId = ParseInt(ParameterNames.SexId);
            AgeId = ParseInt(ParameterNames.AgeId);
            ComparatorId = ParseInt(ParameterNames.ComparatorId);
            ProfileId = ParseInt(ParameterNames.ProfileId);
            RestrictToProfileId = ParseInt(ParameterNames.RestrictToProfileId);
            Key = ParseString(ParameterNames.Key);
        }

    }
}
