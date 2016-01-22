using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class ChildAreasWithAddressesParameters : BaseParameters
    {
        public string ParentArea { get; set; }
        public int ChildAreaTypeId { get; set; }

        public ChildAreasWithAddressesParameters(NameValueCollection parameters)
            : base(parameters)
        {
            ParentArea = ParseString(ParameterNames.ParentAreaCode);
            ChildAreaTypeId = ParseInt(ParameterNames.AreaTypeId);
        }

        public override bool AreValid
        {
            get { return !string.IsNullOrEmpty(ParentArea) && ChildAreaTypeId > 0; }
        }
    }
}
