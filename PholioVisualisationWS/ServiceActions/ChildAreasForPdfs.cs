using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class ChildAreasForPdfs : ParentAreaWithChildAreas
    {
        public Area Benchmark { get; set; }

        public ChildAreasForPdfs(ParentAreaWithChildAreas parentAreaWithChildAreas, Area benchmark)
        {
            Parent = parentAreaWithChildAreas.Parent;
            Children = parentAreaWithChildAreas.Children;
            Benchmark = benchmark;
        }
        
    }
}
