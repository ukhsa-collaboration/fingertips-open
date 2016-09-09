using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class AreaLookupParameters : BaseParameters
    {
        public string SearchText { get; set; }
        public int PolygonAreaTypeId { get; set; }
        public List<int> ParentAreaTypesToIncludeInResults { get; set; }
        public bool AreEastingAndNorthingRequired { get; set; }

        public AreaLookupParameters(NameValueCollection parameters)
            : base(parameters)
        {
            SearchText = ParseString(ParameterNames.Text);
            PolygonAreaTypeId = ParseInt(ParameterNames.PolygonAreaTypeId);
            AreEastingAndNorthingRequired = ParseBool(ParameterNames.AreEastingAndNorthingRequired);
            ParentAreaTypesToIncludeInResults = ParseIntList(ParameterNames.ParentAreaTypesToIncludeInResults, ',');
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
