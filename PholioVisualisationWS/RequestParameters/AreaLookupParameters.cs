using System;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class AreaLookupParameters : BaseParameters, IJsonpParameters
    {
        public string SearchText { get; set; }
        public int PolygonAreaTypeId { get; set; }
        public string Callback { get; set; }
        public bool AreEastingAndNorthingRequired { get; set; }
        public bool ExcludeCcGs { get; set; }

        public AreaLookupParameters(NameValueCollection parameters)
            : base(parameters)
        {
            SearchText = ParseString(ParameterNames.Text);
            PolygonAreaTypeId = ParseInt(ParameterNames.PolygonAreaTypeId);
            Callback = ParseString(ParameterNames.JsonpCallback);
            AreEastingAndNorthingRequired = ParseBool(ParameterNames.AreEastingAndNorthingRequired);
            ExcludeCcGs = ParseBool(ParameterNames.ExcludeCcGs);
        }

        public override bool AreValid
        {
            get
            {
                return base.AreValid && 
                    string.IsNullOrWhiteSpace(SearchText) == false &&
                    PolygonAreaTypeId != UndefinedInteger;
            }
        }
    }
}
