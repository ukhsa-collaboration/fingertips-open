
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class GroupDataAtDataPointBySearchParameters : GroupDataParameters
    {
        public List<int> RestrictResultsToProfileIdList { get; set; }

        public GroupDataAtDataPointBySearchParameters(NameValueCollection parameters)
            : base(parameters)
        {
            RestrictResultsToProfileIdList = ParseIntList(ParameterNames.RestrictToProfileId, Convert.ToChar(","));
        }

        public override bool AreValid
        {
            get
            {
                return UseIndicatorIds;
            }
        }
    }
}
