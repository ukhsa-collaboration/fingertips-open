
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class IndicatorStatsParameters : DataParameters
    {
        public const string ParameterDataPointOffset = "off";

        public int GroupId { get; set; }
        public int DataPointOffset { get; set; }
        public string ParentAreaCode { get; set; }
        public int ChildAreaTypeId { get; set; }
        public IndicatorStatsType IndicatorStatsType { get; set; }

        public List<int> RestrictResultsToProfileIdList { get; set; }

        public IndicatorStatsParameters(NameValueCollection parameters)
            : base(parameters)
        {
            GroupId = ParseInt(ParameterNames.GroupIds);
            DataPointOffset = ParseInt(ParameterDataPointOffset);
            ParentAreaCode = ParseString(ParameterNames.ParentAreaCode);
            ChildAreaTypeId = ParseInt(ParameterNames.AreaTypeId);
            RestrictResultsToProfileIdList = ParseIntList(ParameterNames.RestrictToProfileId, Convert.ToChar(","));
            SetIndicatorStatsType();
        }

        private void SetIndicatorStatsType()
        {
            var statsType = ParseString(ParameterNames.Type);
            if (statsType != null && statsType == "cl")
            {
                IndicatorStatsType = IndicatorStatsType.ControlLimits;
            }
            else
            {
                IndicatorStatsType = IndicatorStatsType.Percentiles25And75;
            }
        }

        public override bool AreValid
        {
            get
            {
                return GroupId > 0 && string.IsNullOrEmpty(ParentAreaCode) == false;
            }
        }
    }
}
