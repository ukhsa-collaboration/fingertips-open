using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class NhsChoicesIdParameters : BaseParameters
    {
        public string AreaCode { get; set; }

        public NhsChoicesIdParameters(NameValueCollection parameters) : base(parameters)
        {
            AreaCode = ParseString(ParameterNames.AreaCode);
        }
    }
}
