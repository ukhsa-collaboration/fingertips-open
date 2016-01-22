using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class AreasByAreaTypeParameters : BaseParameters
    {
        public int AreaTypeId { get; set; }
        public int ProfileId { get; set; }
        public int TemplateProfileId { get; set; }
        public bool RetrieveIgnoredAreas { get; set; }

        public AreasByAreaTypeParameters(NameValueCollection parameters)
            : base(parameters)
        {
            AreaTypeId = ParseInt(ParameterNames.AreaTypeId);
            ProfileId = ParseInt(ParameterNames.ProfileId);
            TemplateProfileId = ParseInt(ParameterNames.TemplateProfileId);
            RetrieveIgnoredAreas = ParseYesOrNo(ParameterNames.RetrieveIgnoredAreas, false);
        }

        public override bool AreValid
        {
            get
            {
                return AreaTypeId > 0;
            }
        }

        public int GetNonSearchProfileId()
        {
            return ParameterHelper.GetNonSearchProfileId(ProfileId, TemplateProfileId);
        }
    }
}
