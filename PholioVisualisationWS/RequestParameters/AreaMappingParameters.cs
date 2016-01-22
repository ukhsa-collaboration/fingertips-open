using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace PholioVisualisation.RequestParameters
{
    public class AreaMappingParameters : AreasByAreaTypeParameters
    {
        public int ParentAreaTypeId { get; set; }        
        public string NearestNeighbourCode { get; set; }

        public AreaMappingParameters(NameValueCollection parameters)
            : base(parameters)
        {
            ParentAreaTypeId = ParseInt(ParameterNames.ParentAreaTypeId);
            NearestNeighbourCode = ParseString(ParameterNames.NearestNeighbourCode);
        }
    }
}
