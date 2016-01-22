using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace PholioVisualisation.RequestParameters
{
    public class ParentAreaGroupsParameters : BaseParameters
    {
        public int ProfileId { get; set; }
        public int TemplateProfileId { get; set; }

        public ParentAreaGroupsParameters(NameValueCollection parameters)
            : base(parameters)
        {
            ProfileId = ParseInt(ParameterNames.ProfileId);
            TemplateProfileId = ParseInt(ParameterNames.TemplateProfileId);
        }

        public override bool AreValid
        {
            get
            {
                return ProfileId > 0;
            }
        }

        public int GetNonSearchProfileId()
        {
            return ParameterHelper.GetNonSearchProfileId(ProfileId, TemplateProfileId);
        }
    }
}
