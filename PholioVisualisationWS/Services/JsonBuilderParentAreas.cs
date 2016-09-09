using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderParentAreas : JsonBuilderBase
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

        public JsonBuilderParentAreas(HttpContextBase context)
            : base(context)
        {
            _parameters = new ParentAreasParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderParentAreas(ParentAreasParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            var childAreaToParentsMap = GetChildAreaToParentsMap();
            return JsonConvert.SerializeObject(childAreaToParentsMap);
        }

        public AreaToParentsMap GetChildAreaToParentsMap()
        {
            var areaTypeIds = _parameters.AreaTypeIds;

            // First level parents
            string areaCode = _parameters.ChildAreaCode;
            AreaToParentsMap childAreaToParentsMap = new AreaToParentsMap(areaCode, reader.GetAreaFromCode(areaCode).AreaTypeId);
            childAreaToParentsMap.Parents = GetParentAreas(areaCode, areaTypeIds);

            // Parents of parents
            foreach (var area in childAreaToParentsMap.Parents)
            {
                area.Parents = GetParentAreas(area.Code, areaTypeIds);
            }

            return childAreaToParentsMap;
        }

        private IEnumerable<AreaToParentsMap> GetParentAreas(string areaCode, IList<int> areaTypeIds)
        {
            return reader.GetParentAreas(areaCode)
                .Where(area => areaTypeIds.Contains(area.AreaTypeId))
                .Select(a => new AreaToParentsMap(a.Code, a.AreaTypeId)).ToList();
        }
    }
}