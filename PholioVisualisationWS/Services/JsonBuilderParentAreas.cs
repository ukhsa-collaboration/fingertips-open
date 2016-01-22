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

        private ParentAreasParameters parameters;
        private IAreasReader reader = ReaderFactory.GetAreasReader();

        public JsonBuilderParentAreas(HttpContextBase context)
            : base(context)
        {
            parameters = new ParentAreasParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            var areaTypeIds = parameters.AreaTypeIds;

            // First level parents
            string areaCode = parameters.ChildAreaCode;
            AreaToParentsMap childAreaToParentsMap = new AreaToParentsMap(areaCode, reader.GetAreaFromCode(areaCode).AreaTypeId);
            childAreaToParentsMap.Parents = GetParentAreas(areaCode, areaTypeIds);

            // Parents of parents
            foreach (var area in childAreaToParentsMap.Parents)
            {
                area.Parents = GetParentAreas(area.Code, areaTypeIds);
            }

            return JsonConvert.SerializeObject(childAreaToParentsMap);
        }

        private IEnumerable<AreaToParentsMap> GetParentAreas(string areaCode, IList<int> areaTypeIds)
        {
            return reader.GetParentAreas(areaCode)
                .Where(area => areaTypeIds.Contains(area.AreaTypeId))
                .Select(a => new AreaToParentsMap(a.Code, a.AreaTypeId)).ToList();
        }
    }
}