using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

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

            var parentAreaTypesToInclude = ParseIntList(ParameterNames.ParentAreaTypesToIncludeInResults, ',');
            ParentAreaTypesToIncludeInResults = parentAreaTypesToInclude.Where(x => x != UndefinedInteger).ToList();
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
