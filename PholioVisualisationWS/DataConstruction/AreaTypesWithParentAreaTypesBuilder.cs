using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class AreaTypesWithParentAreaTypesBuilder
    {
        private IList<ParentAreaGroup> parentAreaGroups;
        private IAreasReader areasReader;

        public AreaTypesWithParentAreaTypesBuilder(IList<ParentAreaGroup> parentAreaGroups, IAreasReader areasReader)
        {
            this.parentAreaGroups = parentAreaGroups;
            this.areasReader = areasReader;
        }

        public IList<AreaType> ChildAreaTypesWithParentAreaTypes
        {
            get
            {
                var areaTypes = new List<AreaType>();

                var list = parentAreaGroups.ToList();
                var groups = list.GroupBy(x => x.ChildAreaTypeId);

                foreach (var group in groups)
                {
                    var childAreaTypeId = group.Key;
                    var areaType = areasReader.GetAreaType(childAreaTypeId);
                    areaType.ParentAreaTypes = GetParentAreaTypes(@group);
                    areaTypes.Add(areaType);
                }

                return areaTypes;
            }
        }

        private IList<IAreaType> GetParentAreaTypes(IGrouping<int, ParentAreaGroup> @group)
        {
            var sorted = @group.ToList().OrderBy(x => x.Sequence);
            return sorted.Select(x => AreaTypeFactory.New(areasReader, x)).ToList();
        }


    }
}
