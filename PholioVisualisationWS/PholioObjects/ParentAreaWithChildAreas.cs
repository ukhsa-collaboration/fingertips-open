using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class ParentAreaWithChildAreas
    {
        [JsonProperty]
        public IArea Parent { get; set; }

        [JsonProperty]
        public IList<IArea> Children { get; set; }

        [JsonIgnore]
        public int ChildAreaTypeId { get; set; }

        protected ParentAreaWithChildAreas()
        {
        }

        public ParentAreaWithChildAreas(IArea parent, IEnumerable<IArea> children, int childAreaTypeId)
        {
            Parent = parent;
            Children = children.ToList();
            ChildAreaTypeId = childAreaTypeId;
        }

        public IEnumerable<string> GetChildAreaCodes()
        {
            return Children.Select(x => x.Code);
        }
    }
}
