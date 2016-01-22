using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace PholioVisualisation.RequestParameters
{
    public class SearchIndicatorsParameters : SearchParameters
    {
        public List<int> RestrictResultsToProfileIdList { get; set; }

        public SearchIndicatorsParameters(NameValueCollection parameters)
            : base(parameters)
        {
            RestrictResultsToProfileIdList = ParseIntList(ParameterNames.RestrictToProfileId, ',');
        }

    }
}
