
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class GroupDataAtDataPointBySearchParameters : GroupDataParameters
    {
        public IList<int> RestrictResultsToProfileIdList { get; set; }

        /// <summary>
        /// For when data for one specific area is required. So ParentAreaCode is not required.
        /// </summary>
        public string AreaCode { get; set; }

        public GroupDataAtDataPointBySearchParameters(NameValueCollection parameters)
            : base(parameters)
        {
            RestrictResultsToProfileIdList = parameters[ParameterNames.RestrictToProfileId] == null 
                ? new List<int>() 
                : ParseIntList(ParameterNames.RestrictToProfileId,',');
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
