using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace PholioVisualisation.RequestParameters
{
    public class GroupingTreeParameters : BaseParameters
    {
        public List<int> GroupIds { get; set; }

        public GroupingTreeParameters(NameValueCollection parameters)
            : base(parameters)
        {
            GroupIds = ParseIntList(ParameterNames.GroupIds, ',');
        }

        public override bool AreValid
        {
            get
            {
                return base.AreValid && GroupIds.Any();
            }
        }


    }
}
