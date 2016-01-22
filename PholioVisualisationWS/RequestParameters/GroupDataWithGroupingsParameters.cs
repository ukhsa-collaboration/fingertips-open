
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public abstract class GroupDataWithGroupingsParameters : GroupDataParameters
    {
        public int GroupId { get; set; }

        protected GroupDataWithGroupingsParameters(NameValueCollection parameters)
            : base(parameters)
        {
            GroupId = ParseInt(ParameterNames.GroupIds);
        }

        public override bool AreValid
        {
            get
            {
                return base.AreValid &&
                    GroupId > 0;
            }
        }
    }
}
