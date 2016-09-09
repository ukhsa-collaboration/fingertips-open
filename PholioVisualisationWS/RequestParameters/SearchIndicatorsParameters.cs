using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace PholioVisualisation.RequestParameters
{
    public class SearchIndicatorsParameters : BaseParameters
    {
        public const string ParameterSearchText = "txt";

        public string SearchText { get; set; }

        public List<int> RestrictResultsToProfileIdList { get; set; }

        public SearchIndicatorsParameters(NameValueCollection parameters)
            : base(parameters)
        {
            SearchText = ParseString(ParameterSearchText);
            RestrictResultsToProfileIdList = ParseIntList(ParameterNames.RestrictToProfileId, ',');
        }

        public override bool AreValid
        {
            get
            {
                return base.AreValid && string.IsNullOrWhiteSpace(SearchText) == false;
            }
        }


    }
}
