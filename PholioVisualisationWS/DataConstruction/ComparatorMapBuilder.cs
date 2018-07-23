
using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class ComparatorMapBuilder
    {
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();

        public ComparatorMap ComparatorMap { get; set; }

        public ComparatorMapBuilder(int childAreaTypeId) 
        {
            ComparatorMap = new ComparatorMap();
            AddNationalComparator(childAreaTypeId);
        }

        public ComparatorMapBuilder(ParentArea parentArea) :
            this(new List<ParentArea> { parentArea }) { }

        public ComparatorMapBuilder(IList<ParentArea> parentAreas)
        {
            ComparatorMap = new ComparatorMap();

            foreach (var parentArea in parentAreas)
            {
                AddNationalComparator(parentArea.ChildAreaTypeId);
                AddSubnationalComparator(parentArea);
            }
        }

        private void AddSubnationalComparator(ParentArea parentArea)
        {
            ComparatorMap.Add(new ComparatorDetails
            {
                ComparatorId = ComparatorIds.Subnational,
                ChildAreaTypeId = parentArea.ChildAreaTypeId,
                Area = AreaFactory.NewArea(areasReader, parentArea.AreaCode)
            });
        }

        private void AddNationalComparator(int areaTypeId)
        {
            ComparatorMap.Add(new ComparatorDetails
            {
                ComparatorId = ComparatorIds.England,
                ChildAreaTypeId = areaTypeId,
                Area = areasReader.GetAreaFromCode(AreaCodes.England)
            });
        }
    }
}
