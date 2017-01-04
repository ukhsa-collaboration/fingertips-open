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

            var mappingForNonMatchingProfile = new List<ProfileNearestNeighbourAreaTypeMapping>();

            var mappingForMatchingProfile = new List<ProfileNearestNeighbourAreaTypeMapping>();


            foreach (var mapping in mappings)
            {
                if (mapping.ProfileId == profileId)
                {
                    mappingForMatchingProfile.Add(mapping);
                }

                mappingForNonMatchingProfile.Add(mapping);
            }


            List<int> areaTypeIds;
            List<int> neighbourTypeIds;

            if (mappingForMatchingProfile.Count > 0)
            {
                areaTypeIds = mappingForMatchingProfile.Select(a => a.AreaTypeId).ToList();
                neighbourTypeIds = mappingForMatchingProfile.Select(n => n.NeighbourTypeId).ToList();
            }
            else
            {
                areaTypeIds = mappingForNonMatchingProfile.Select(a => a.AreaTypeId).ToList();
                neighbourTypeIds = mappingForNonMatchingProfile.Select(n => n.NeighbourTypeId).ToList();

            }

            var nearestNeighbourAreaTypes = _reader.GetNearestNeighbourAreaType(neighbourTypeIds);
            var response = new Dictionary<int, NeighbourType>();

            foreach (var area in areaTypeIds)
            {
                var neighbour = mappingForMatchingProfile.Count > 0
                    ? mappingForMatchingProfile.First(x => x.AreaTypeId == area)
                    : mappingForNonMatchingProfile.First(x => x.AreaTypeId == area);

                var neighbourData = nearestNeighbourAreaTypes.First(n => n.NeighbourTypeId == neighbour.NeighbourTypeId);

                response.Add(area, neighbourData);
            }

            return response;
        }
    }
}
