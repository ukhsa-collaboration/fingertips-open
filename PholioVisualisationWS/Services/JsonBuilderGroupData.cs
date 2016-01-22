
using System.Collections.Generic;
using PholioVisualisation.PholioObjects;
using Newtonsoft.Json;

namespace PholioVisualisation.Services
{
    public class JsonBuilderGroupData
    {

        public IList<GroupRoot> GroupRoots { get; set; }
        public IList<IndicatorMetadata> IndicatorMetadata { get; set; }

        public string BuildJson()
        {
            return IsRequiredDataAvailable ?
                JsonConvert.SerializeObject(GroupRoots) :
                string.Empty;
        }

        public bool IsRequiredDataAvailable
        {
            get
            {
                return GroupRoots != null;
            }
        }
    }
}