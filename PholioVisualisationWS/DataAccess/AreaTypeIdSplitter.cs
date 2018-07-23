
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess.Repositories;

namespace PholioVisualisation.DataAccess
{
    public interface IAreaTypeIdSplitter
    {
        List<int> GetComponentAreaTypeIds(int areaTypeId);
        List<int> GetComponentAreaTypeIds(IEnumerable<int> areaTypeIds);
    }

    public class AreaTypeIdSplitter : IAreaTypeIdSplitter
    {
        private IAreaTypeComponentRepository _areaTypeComponentRepository;

        public AreaTypeIdSplitter(IAreaTypeComponentRepository areaTypeComponentRepository)
        {
            _areaTypeComponentRepository = areaTypeComponentRepository;
        }

        public List<int> GetComponentAreaTypeIds(int areaTypeId)
        {
            return GetComponentAreaTypeIds(new List<int> {areaTypeId});
        }

        public List<int> GetComponentAreaTypeIds(IEnumerable<int> areaTypeIds)
        {
            var ids = new List<int>();

            foreach (var areaTypeId in areaTypeIds)
            {
                var componentAreaTypeIds = _areaTypeComponentRepository
                    .GetAreaTypeComponents(areaTypeId)
                    .Select(x => x.ComponentAreaTypeId)
                    .ToList();

                if (componentAreaTypeIds.Any())
                {
                    ids.AddRange(componentAreaTypeIds);
                } else
                {
                    // Area type is not made up of other area types
                    ids.Add(areaTypeId);
                }
            }

            return ids;
        }
    }
}
