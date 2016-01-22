using System;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class SearchParameters : BaseParameters
    {
        public const string ParameterSearchText = "txt";

        public string SearchText { get; set; }

        protected SearchParameters(NameValueCollection parameters)
            : base(parameters)
        {
            SearchText = ParseString(ParameterSearchText);
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
