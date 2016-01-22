using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class HelpTextParameters : BaseParameters
    {
        public string HelpKey { get; set; }

        public HelpTextParameters(NameValueCollection parameters)
            : base(parameters)
        {
            HelpKey = ParseString(ParameterNames.Key);
        }

        public override bool AreValid
        {
            get
            {
                return string.IsNullOrWhiteSpace(HelpKey) == false;
            }
        }
    }
}
