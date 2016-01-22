using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace PholioVisualisation.RequestParameters
{
    public class GroupedIndicatorNamesParameters : BaseParameters
    {
        public IList<int> GroupIds { get; set; }

        public GroupedIndicatorNamesParameters(NameValueCollection parameters)
            : base(parameters)
        {
            GroupIds = ParseIntList(ParameterNames.GroupIds, ',');
        }

        public override bool AreValid
        {
            get
            {
                return GroupIds.Any();
            }
        }
    }
}
