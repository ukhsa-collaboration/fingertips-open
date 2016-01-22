
using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.PholioObjects
{
    public class ComparatorMap
    {
        private List<Comparator> comparators = new List<Comparator>();

        public ComparatorMap LimitByParentArea(ParentArea parentArea)
        {
            ComparatorMap map = new ComparatorMap();
            IEnumerable<Comparator> result = comparators.Where(c => c.ChildAreaTypeId == parentArea.ChildAreaTypeId &&
                c.Area.Code == parentArea.AreaCode);
            foreach (var comparator in result)
            {
                map.Add(comparator);
            }
            return map;
        }

        public void Add(Comparator comparator)
        {
            if (comparator != null)
            {
                comparators.Add(comparator);
            }
        }

        public IEnumerable<Comparator> Comparators
        {
            get { return comparators.AsReadOnly(); }
        }

        public int Count
        {
            get { return comparators.Count; }
        }

        public Comparator GetComparatorById(int id, int areaTypeId)
        {
            IEnumerable<Comparator> result = comparators.Where(c => c.ComparatorId == id && c.ChildAreaTypeId == areaTypeId);
            if (result.Count() > 1)
            {
                throw new FingertipsException("More than 1 comparator with same ID where only 1 expected.");
            }
            return result.FirstOrDefault();
        }

        public Comparator GetComparatorById(int id)
        {
            IEnumerable<Comparator> result = comparators.Where(c => c.ComparatorId == id);
            if (result.Count() > 1)
            {
                throw new FingertipsException("More than 1 comparator with same ID where only 1 expected.");
            }
            return result.FirstOrDefault();
        }

        public Comparator GetSubnationalComparator()
        {
            return GetComparatorById(ComparatorIds.Subnational);
        }

        public Comparator GetRegionalComparatorByRegion(ParentArea parentArea)
        {
            return comparators.FirstOrDefault(comparator => comparator.ComparatorId == ComparatorIds.Subnational &&
                                                            comparator.Area.Code == parentArea.AreaCode &&
                                                            comparator.ChildAreaTypeId == parentArea.ChildAreaTypeId);
        }

        public Comparator GetNationalComparator()
        {
            return comparators.FirstOrDefault(comparator => comparator.ComparatorId == ComparatorIds.England);
        }
    }
}
