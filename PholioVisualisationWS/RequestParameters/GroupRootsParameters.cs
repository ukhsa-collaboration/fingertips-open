using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace PholioVisualisation.RequestParameters
{
    public class GroupRootsParameters : BaseParameters
    {
        public int GroupId { get; set; }
        public int AreaTypeId { get; set; }

        public GroupRootsParameters(NameValueCollection parameters)
            : base(parameters)
        {
            GroupId = ParseInt(ParameterNames.GroupIds);
            AreaTypeId = ParseInt(ParameterNames.AreaTypeId);
        }

        public override bool AreValid
        {
            get
            {
                return GroupId > 0 && AreaTypeId > 0;
            }
        }
    }
}
