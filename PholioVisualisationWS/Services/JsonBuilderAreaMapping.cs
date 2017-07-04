using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderAreaMapping : JsonBuilderBase
    {
        private AreaMappingParameters _parameters;
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();

        public JsonBuilderAreaMapping(HttpContextBase context)
            : base(context)
        {
            _parameters = new AreaMappingParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderAreaMapping(AreaMappingParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            return JsonConvert.SerializeObject(GetParentAreaToChildAreaDictionary());
        }

        public Dictionary<string, IEnumerable<string>> GetParentAreaToChildAreaDictionary()
        {
            Dictionary<string, IEnumerable<string>> parentCodeToChildCodes = new Dictionary<string, IEnumerable<string>>();

            var ignoredAreasFilter = IgnoredAreasFilterFactory.New(_parameters.GetNonSearchProfileId());

            var parentAreas = GetParentAreas();

            if (_parameters.NearestNeighbourCode != "")
            {
                var areaCode = _parameters.NearestNeighbourCode.Substring(5);
                parentCodeToChildCodes.Add(areaCode, parentAreas.Select(x => x.Code).ToList());
            }
            else
            {
                foreach (var parentArea in parentAreas)
                {
                    var parentChildAreaRelationship = new ParentChildAreaRelationshipBuilder(
                        ignoredAreasFilter, new AreaListProvider(areasReader))
                        .GetParentAreaWithChildAreas(parentArea, _parameters.AreaTypeId, _parameters.RetrieveIgnoredAreas);

                    parentCodeToChildCodes.Add(parentArea.Code, parentChildAreaRelationship.GetChildAreaCodes());
                }
            }

            return parentCodeToChildCodes;
        }

        private IList<IArea> GetParentAreas()
        {
            var listBuilder = new AreaListProvider(areasReader);
            if (Area.IsNearestNeighbour(_parameters.NearestNeighbourCode))
            {
                listBuilder.CreateAreaListFromNearestNeighbourAreaCode(_parameters.ProfileId, _parameters.NearestNeighbourCode);
            }
            else
            {
                listBuilder.CreateAreaListFromAreaTypeId(_parameters.ProfileId, _parameters.ParentAreaTypeId);
            }
            return listBuilder.Areas;
        }
    }
}