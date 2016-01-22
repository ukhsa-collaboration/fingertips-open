using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class DataDownloadBespokeParameters : BaseParameters
    {
        public const string ParameterProfileKey = "pro";

        public string ProfileKey { get; set; }
        public List<int> GroupIds { get; set; }
        public string AreaCode { get; set; }
        public int AreaTypeId { get; set; }

        public DataDownloadBespokeParameters(NameValueCollection parameters)
            : base(parameters)
        {
            ProfileKey = ParseString(ParameterProfileKey);
            GroupIds = ParseIntList(ParameterNames.GroupIds, ',');
            AreaCode = ParseString(ParameterNames.AreaCode);
            AreaTypeId = ParseInt(ParameterNames.AreaTypeId);
        }

    }
}
