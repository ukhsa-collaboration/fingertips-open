using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class ParentAreasParameters : BaseParameters
    {
        public IList<int> AreaTypeIds { get; set; }
        public string ChildAreaCode { get; set; }

        public ParentAreasParameters(NameValueCollection parameters)
            : base(parameters)
        {
            ChildAreaCode = ParseString(ParameterNames.AreaCode);
            AreaTypeIds = ParseIntList(ParameterNames.AreaTypeId, ',');
        }

        public override bool AreValid
        {
            get
            {
                return base.AreValid &&
                    string.IsNullOrWhiteSpace(ChildAreaCode) == false &&
                    AreaTypeIds.Count > 0;
            }
        }
    }
}
