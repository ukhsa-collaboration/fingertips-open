using Profiles.DataAccess;
using Profiles.DomainObjects;
using System.Collections.Generic;
using System.Linq;

namespace Profiles.MainUI.Helpers
{
    public class NearestNeighbourHelper
    {
        private NearestNeighbourReader _reader;

        public NearestNeighbourHelper()
        {
            _reader = ReaderFactory.GetNearestNeighbourReader();
        }

        /// <summary>
        /// Returns a dictionary of areaTypeIds and nearestNeighbourTypes
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns>areaTypeId,NeighbourType</returns>
        public Dictionary<int, NeighbourType> GetNeighbourConfig(int profileId)
        {
            var mappings = _reader.GetProfileNearestNeighbourAreaTypeMapping(profileId);
            var orderedMappings = mappings.OrderByDescending(x => x.ProfileId);

            var nearestNeighbourTypeIds = mappings.Select(x => x.NeighbourTypeId).Distinct().ToList();
            var nearestNeighbourAreaTypes = _reader.GetNearestNeighbourAreaType(nearestNeighbourTypeIds);
            var response = new Dictionary<int, NeighbourType>();

            foreach (var mapping in orderedMappings)
            {
                if (response.ContainsKey(mapping.AreaTypeId) == false)
                {
                    var type = nearestNeighbourAreaTypes.FirstOrDefault(x => x.NeighbourTypeId == mapping.NeighbourTypeId);
                    response.Add(mapping.AreaTypeId, type);
                }
            }

            return response;
        }
    }
}
