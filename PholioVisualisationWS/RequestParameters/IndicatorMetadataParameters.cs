
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace PholioVisualisation.RequestParameters
{
    public class IndicatorMetadataParameters : BaseParameters
    {
        /// <summary>
        /// Can be "yes" (default), "no" 
        /// </summary>
        public const string ParameterIncludeDefinition = "def";
        public const string ParameterIncludeSystemContent = "include_system_content";

        public IList<int> GroupIds { get; set; }
        public IList<int> IndicatorIds { get; set; }
        public bool IncludeDefinition { get; set; }
        public bool IncludeSystemContent { get; set; }

        public List<int> RestrictResultsToProfileIds { get; set; }

        public IndicatorMetadataParameters(NameValueCollection parameters)
            : base(parameters)
        {
            GroupIds = ParseIntList(ParameterNames.GroupIds, ',');
            IndicatorIds = ParseIntList(DataParameters.ParameterIndicatorIds, ',');
            IncludeDefinition = ParseYesOrNo(ParameterIncludeDefinition, false);
            IncludeSystemContent = ParseYesOrNo(ParameterIncludeSystemContent, false);
            RestrictResultsToProfileIds = ParseIntList(ParameterNames.RestrictToProfileId, ',');
        }

        public override bool AreValid
        {
            get
            {
                return true;
            }
        }

        public bool UseIndicatorIds
        {
            get { return IndicatorIds.Any(); }
        }

        public bool UseGroupIds
        {
            get { return GroupIds.Any(); }
        }
    }
}
