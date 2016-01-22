using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class NeighbourAreaType : IAreaType
    {
        public const int IdAddition = 20000;

        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Short")]
        public string ShortName { get; set; }

        public static NeighbourAreaType New(int neighbourId)
        {
            int spoofAreaTypeId = neighbourId + IdAddition;

            return new NeighbourAreaType
            {
                Id = spoofAreaTypeId,
                Name = "Nearest Neighbours",
                ShortName = "Nearest Neighbours"
            };
        }
    }
}
