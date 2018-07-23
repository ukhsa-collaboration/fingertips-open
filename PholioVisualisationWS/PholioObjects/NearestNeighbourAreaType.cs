using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class NearestNeighbourAreaType : IAreaType
    {
        public const int IdAddition = 20000;

        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Short")]
        public string ShortName { get; set; }

        public static NearestNeighbourAreaType New(int neighbourId)
        {
            int spoofAreaTypeId = neighbourId + IdAddition;

            return new NearestNeighbourAreaType
            {
                Id = spoofAreaTypeId,
                Name = "Nearest Neighbours",
                ShortName = "Nearest Neighbours"
            };
        }

        public static int GetNearestNeighbourTypeIdFromAreaTypeId(int areaTypeId)
        {
            return areaTypeId - IdAddition;
        }

        public static int GetAreaTypeIdFromNearestNeighbourTypeId(int neighbourTypeId)
        {
            return neighbourTypeId + IdAddition;
        }

        public static bool IsCategoryAreaTypeId(int areaTypeId)
        {
            return areaTypeId > IdAddition;
        }
    }
}
