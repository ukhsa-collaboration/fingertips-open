using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData.Repositories;

namespace Fpm.ProfileData
{
    public interface IAreaTypeIdSplitter
    {
        List<int> GetComponentAreaTypeIds(int areaTypeId);
        List<int> GetComponentAreaTypeIds(IEnumerable<int> areaTypeIds);
    }

    /// <summary>
    /// Splits area types into their component area types where appropriate.
    /// </summary>
    /// <remarks>Copy of class in PholioVisualisationWS</remarks>
    public class AreaTypeIdSplitter : IAreaTypeIdSplitter
    {
        private IAreaTypeRepository _areaTypeComponentRepository;

        public AreaTypeIdSplitter(IAreaTypeRepository areaTypeComponentRepository)
        {
            _areaTypeComponentRepository = areaTypeComponentRepository;
        }

        public List<int> GetComponentAreaTypeIds(int areaTypeId)
        {
            return GetComponentAreaTypeIds(new List<int> { areaTypeId });
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
                }
                else
                {
                    // Area type is not made up of other area types
                    ids.Add(areaTypeId);
                }
            }

            return ids;
        }
    }
}
