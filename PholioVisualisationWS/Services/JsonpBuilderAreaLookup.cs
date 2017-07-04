﻿
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.SearchQuerying;

namespace PholioVisualisation.Services
{
    public class JsonpBuilderAreaLookup
    {
        private readonly AreaLookupParameters _parameters;

        public JsonpBuilderAreaLookup(AreaLookupParameters parameters)
        {
            _parameters = parameters;
        }

        public List<GeographicalSearchResult> GetGeographicalSearchResults()
        {
            return new GeographicalSearch
            {
                AreEastingAndNorthingRetrieved = _parameters.AreEastingAndNorthingRequired
            }.SearchPlacePostcodes(_parameters.SearchText, _parameters.PolygonAreaTypeId,
                _parameters.ParentAreaTypesToIncludeInResults);
        }
    }
}