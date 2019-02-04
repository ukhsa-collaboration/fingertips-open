using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class AreaMappingParameters : AreasByAreaTypeParameters
    {
        public int ParentAreaTypeId { get; set; }        
        public string NearestNeighbourCode { get; set; }
        public string UserId { get; set; }

        public AreaMappingParameters(NameValueCollection parameters)
            : base(parameters)
        {
            ParentAreaTypeId = ParseInt(ParameterNames.ParentAreaTypeId);
            NearestNeighbourCode = ParseString(ParameterNames.NearestNeighbourCode);
            UserId = ParseString(ParameterNames.UserId);
        }
    }
}
