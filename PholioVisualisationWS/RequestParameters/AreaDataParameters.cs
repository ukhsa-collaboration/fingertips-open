
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class AreaDataParameters : DomainParameters
    {
        public const string ParameterComparatorAreaCodes = "com";
        public const string ParameterIncludeTimePeriods = "tim";
        public const string ParameterLatestDataOnly = "latest_data_only";

        public int AreaTypeId { get; set; }
        public IList<string> AreaCodes { get; set; }
        public IList<string> ComparatorAreaCodes { get; set; }
        public bool IncludeTimePeriods { get; set; }
        public bool LatestDataOnly { get; set; }

        public AreaDataParameters(NameValueCollection parameters)
            : base(parameters)
        {
            AreaTypeId = ParseInt(ParameterNames.AreaTypeId);
            AreaCodes = ParseStringList(ParameterNames.AreaCode, ',');
            ComparatorAreaCodes = ParseStringList(ParameterComparatorAreaCodes, ',');
            IncludeTimePeriods = ParseYesOrNo(ParameterIncludeTimePeriods, false);
            LatestDataOnly = ParseYesOrNo(ParameterLatestDataOnly, false);
        }

        public override bool AreValid
        {
            get
            {
                return base.AreValid && AreaCodes.Count > 0;
            }
        }

    }
}
