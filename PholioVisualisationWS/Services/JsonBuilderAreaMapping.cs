using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderAreaMapping : JsonBuilderBase
    {
        private AreaMappingParameters parameters;
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();

        public JsonBuilderAreaMapping(HttpContextBase context)
            : base(context)
        {
            parameters = new AreaMappingParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            Dictionary<string, IEnumerable<string>> parentCodeToChildCodes = new Dictionary<string, IEnumerable<string>>();

            var ignoredAreasFilter = IgnoredAreasFilterFactory.New(parameters.GetNonSearchProfileId());

            var parentAreas = GetParentAreas();
            
            if (parameters.NearestNeighbourCode != "")
            {                
                parentCodeToChildCodes.Add(parameters.NearestNeighbourCode.Substring(5), 
                    parentAreas.Select(x => x.Code).ToList());
            }
            else
            {
                foreach (var parentArea in parentAreas)
                {
                    var parentChildAreaRelationship = new ParentChildAreaRelationshipBuilder(
                        ignoredAreasFilter, new AreaListBuilder(areasReader))
                        .GetParentAreaWithChildAreas(parentArea, parameters.AreaTypeId, parameters.RetrieveIgnoredAreas);

                    parentCodeToChildCodes.Add(parentArea.Code, parentChildAreaRelationship.GetChildAreaCodes());
                }
            }

            return JsonConvert.SerializeObject(parentCodeToChildCodes);
        }

        private IList<IArea> GetParentAreas()
        {
            var listBuilder = new AreaListBuilder(areasReader);
            if (Area.IsNearestNeighbour(parameters.NearestNeighbourCode))
            {
                listBuilder.CreateAreaListFromNearestNeighbourAreaCode(parameters.ProfileId, parameters.NearestNeighbourCode);
            }
            else
            {
                listBuilder.CreateAreaListFromAreaTypeId(parameters.ProfileId, parameters.ParentAreaTypeId);
            }
            return listBuilder.Areas;
        }
    }
}