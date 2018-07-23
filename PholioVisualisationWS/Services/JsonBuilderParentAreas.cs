using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.DataAccess.Repositories;

namespace PholioVisualisation.Services
{
    public class JsonBuilderParentAreas 
    {
        public class AreaToParentsMap
        {
            public string Code { get; set; }
            public int AreaTypeId { get; set; }
            public IEnumerable<AreaToParentsMap> Parents { get; set; }

            public AreaToParentsMap(string areaCode, int areaTypeId)
            {
                Code = areaCode;
                AreaTypeId = areaTypeId;
                Parents = new List<AreaToParentsMap>();
            }
        }

        private ParentAreasParameters _parameters;
        private IAreasReader reader = ReaderFactory.GetAreasReader();

        public JsonBuilderParentAreas(ParentAreasParameters parameters)
        {
            _parameters = parameters;
        }

        public AreaToParentsMap GetChildAreaToParentsMap()
        {
            var parentAreaTypeIds = _parameters.AreaTypeIds;

            // First level parents
            string childAreaCode = _parameters.ChildAreaCode;
            var childArea = reader.GetAreaFromCode(childAreaCode);
            AreaToParentsMap childAreaToParentsMap = new AreaToParentsMap(childAreaCode, 
                childArea.AreaTypeId);
            childAreaToParentsMap.Parents = GetParentAreas(childAreaCode, parentAreaTypeIds);

            // Parents of parents
            foreach (var area in childAreaToParentsMap.Parents)
            {
                area.Parents = GetParentAreas(area.Code, parentAreaTypeIds);
            }

            return childAreaToParentsMap;
        }

        private IEnumerable<AreaToParentsMap> GetParentAreas(string areaCode, IList<int> parentAreaTypeIds)
        {
            //TODO should work for composite area types, e.g. 10002
            var singleAreaTypeIds = GetComponentAreaTypeIds(parentAreaTypeIds);
            var parentAreas = reader.GetParentAreas(areaCode);
            return parentAreas
                .Where(area => singleAreaTypeIds.Contains(area.AreaTypeId))
                .Select(a => new AreaToParentsMap(a.Code, a.AreaTypeId)).ToList();
        }

        private IList<int> GetComponentAreaTypeIds(IList<int> areaTypeIds)
        {
            return new AreaTypeIdSplitter(new AreaTypeComponentRepository())
                .GetComponentAreaTypeIds(areaTypeIds);
        }
    }
}