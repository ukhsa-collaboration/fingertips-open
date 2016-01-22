using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class AreaCategoriesParameters : BaseParameters
    {
        public int ChildAreaTypeId { get; set; }
        public int CategoryTypeId { get; set; }
        public int ProfileId { get; set; }

        public AreaCategoriesParameters(NameValueCollection parameters)
            : base(parameters)
        {
            ChildAreaTypeId = ParseInt(ParameterNames.AreaTypeId);
            CategoryTypeId = ParseInt(ParameterNames.CategoryTypeId);
            ProfileId = ParseInt(ParameterNames.ProfileIdFull);
        }

        public override bool AreValid
        {
            get
            {
                return
                    ChildAreaTypeId > 0 &&
                    CategoryTypeId > 0;
            }
        }
    }
}
