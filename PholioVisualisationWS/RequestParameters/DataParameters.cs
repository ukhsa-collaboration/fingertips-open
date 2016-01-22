
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class DataParameters : BaseParameters
    {
        public const string ParameterIndicatorIds = "iids";

        public int ProfileId { get; set; }
        public IList<int> IndicatorIds { get; set; }

        public DataParameters(NameValueCollection parameters)
            : base(parameters)
        {
            ProfileId = ParseInt(ParameterNames.ProfileId);
            IndicatorIds = ParseIntList(ParameterIndicatorIds, ',');
        }

        public bool UseProfile
        {
            get { return !UseIndicatorIds; }
        }

        public bool UseIndicatorIds
        {
            get { return IndicatorIds.Count > 0; }
        }
    }
}
