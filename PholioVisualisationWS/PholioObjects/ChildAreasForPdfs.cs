using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class ChildAreasForPdfs : ParentAreaWithChildAreas
    {
        [JsonProperty]
        public Area Benchmark { get; set; }

        public ChildAreasForPdfs(ParentAreaWithChildAreas parentAreaWithChildAreas, Area benchmark)
        {
            Parent = parentAreaWithChildAreas.Parent;
            Children = parentAreaWithChildAreas.Children;
            Benchmark = benchmark;
        }
        
    }
}
