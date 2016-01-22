
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class DomainParameters : BaseParameters
    {
        public List<int> GroupIds { get; set; }

        public DomainParameters(NameValueCollection parameters)
            : base(parameters)
        {
            GroupIds = ParseIntList(ParameterNames.GroupIds, ',');
        }

        public override bool AreValid
        {
            get
            {
                return GroupIds.Count > 0;
            }
        }
    }
}
