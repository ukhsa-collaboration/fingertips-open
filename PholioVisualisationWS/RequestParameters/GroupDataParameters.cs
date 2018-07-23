
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public abstract class GroupDataParameters : DataParameters
    {
        public string ParentAreaCode { get; set; }
        public int AreaTypeId { get; set; }

        protected GroupDataParameters(NameValueCollection parameters)
            : base(parameters)
        {
            ParentAreaCode = ParseString(ParameterNames.ParentAreaCode);
            AreaTypeId = ParseInt(ParameterNames.AreaTypeId);
        }

        public override bool AreValid
        {
            get
            {
                return base.AreValid  && AreaTypeId > 0;
            }
        }
    }
}
