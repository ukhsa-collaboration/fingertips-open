using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class AreaAddressParameters : BaseParameters
    {
        public string AreaCode { get; set; }

        public AreaAddressParameters(NameValueCollection parameters)
            : base(parameters)
        {
            AreaCode = ParseString(ParameterNames.AreaCode);
        }

        public override bool AreValid
        {
            get
            {
                return base.AreValid && string.IsNullOrWhiteSpace(AreaCode) == false;
            }
        }


    }
}
