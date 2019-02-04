using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.UserData;
using PholioVisualisation.UserData.Repositories;

namespace PholioVisualisation.Services
{
    public class JsonBuilderAreaMapping 
    {
        private AreaMappingParameters _parameters;
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();

        public JsonBuilderAreaMapping(AreaMappingParameters parameters)
        {
            _parameters = parameters;
        }

        public Dictionary<string, IEnumerable<string>> GetParentAreaToChildAreaDictionary()
        {
            var nearestNeighbourCode = _parameters.NearestNeighbourCode;
            var userId = _parameters.UserId;
            var parentAreaTypeId = _parameters.ParentAreaTypeId;

            Dictionary<string, IEnumerable<string>> parentCodeToChildCodes = new Dictionary<string, IEnumerable<string>>();

            var ignoredAreasFilter = IgnoredAreasFilterFactory.New(_parameters.GetNonSearchProfileId());

            var listBuilder = new AreaListProvider(areasReader);

            if (parentAreaTypeId == 30000 && userId != null)
            {
                IAreaListRepository areaListRepository = new AreaListRepository(new fingertips_usersEntities());
                var areaLists = areaListRepository.GetAll(userId);
                foreach (var areaList in areaLists)
                {
                    List<string> areaListAreaCodes = new List<string>();
                    foreach (var areaListAreaCode in areaList.AreaListAreaCodes)
                    {
                        areaListAreaCodes.Add(areaListAreaCode.AreaCode);
                    }

                    parentCodeToChildCodes.Add(areaList.PublicId, areaListAreaCodes);
                }
            }
            else if (nearestNeighbourCode != "")
            {
                // Find nearest neighbours
                CheckNeighbourCodeIsValid(nearestNeighbourCode);
                listBuilder.CreateAreaListFromNearestNeighbourAreaCode(nearestNeighbourCode);
                var neighbours = listBuilder.Areas;
                var areaCode = NearestNeighbourArea.GetAreaCodeFromNeighbourAreaCode(nearestNeighbourCode);
                parentCodeToChildCodes.Add(areaCode, neighbours.Select(x => x.Code).ToList());
            }
            else
            {
                // Find child areas of parents
                listBuilder.CreateAreaListFromAreaTypeId(_parameters.ProfileId, _parameters.ParentAreaTypeId, _parameters.UserId);
                var parentAreas = listBuilder.Areas;
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

        private static void CheckNeighbourCodeIsValid(string nearestNeighbourCode)
        {
            if (NearestNeighbourArea.IsNearestNeighbourAreaCode(nearestNeighbourCode) == false)
            {
                throw new FingertipsException("Area code is not a neighbour code: " + nearestNeighbourCode);
            }
        }
    }
}