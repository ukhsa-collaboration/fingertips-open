using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class ParentAreaGroupOrganiser
    {
        private IList<ParentAreaGroup> parentAreaGroups;
        private IAreasReader areasReader;

        public ParentAreaGroupOrganiser(IList<ParentAreaGroup> parentAreaGroups, IAreasReader areasReader)
        {
            this.parentAreaGroups = parentAreaGroups;
            this.areasReader = areasReader;
        }

        /// <summary>
        /// Key = child area type ID, Value = available parent area types
        /// </summary>
        public IDictionary<int, IList<IAreaType>> ChildAreaTypeIdToParentArea
        {
            get
            {
                var map = new Dictionary<int, IList<IAreaType>>();

                var groups = parentAreaGroups.GroupBy(x => x.ChildAreaTypeId);

                foreach (var group in groups)
                {
                    var childAreaTypeId = group.Key;
                    map.Add(childAreaTypeId, GetParentAreaTypes(@group));
                }

                return map;
            }
        }

        private IList<IAreaType> GetParentAreaTypes(IGrouping<int, ParentAreaGroup> @group)
        {
            var sorted = @group.ToList().OrderBy(x => x.Sequence);
            return sorted.Select(x => AreaTypeFactory.New(areasReader, x)).ToList();
        }


    }
}
