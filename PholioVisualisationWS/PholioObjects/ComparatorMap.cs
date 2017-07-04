
using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.PholioObjects
{
    public class ComparatorMap
    {
        private List<ComparatorDetails> comparators = new List<ComparatorDetails>();

        public ComparatorMap LimitByParentArea(ParentArea parentArea)
        {
            ComparatorMap map = new ComparatorMap();
            IEnumerable<ComparatorDetails> result = comparators.Where(c => c.ChildAreaTypeId == parentArea.ChildAreaTypeId &&
                c.Area.Code == parentArea.AreaCode);
            foreach (var comparator in result)
            {
                map.Add(comparator);
            }
            return map;
        }

        public void Add(ComparatorDetails comparatorDetails)
        {
            if (comparatorDetails != null)
            {
                comparators.Add(comparatorDetails);
            }
        }

        public IEnumerable<ComparatorDetails> Comparators
        {
            get { return comparators.AsReadOnly(); }
        }

        public int Count
        {
            get { return comparators.Count; }
        }

        public ComparatorDetails GetComparatorById(int id, int areaTypeId)
        {
            IEnumerable<ComparatorDetails> result = comparators.Where(c => c.ComparatorId == id && c.ChildAreaTypeId == areaTypeId);
            if (result.Count() > 1)
            {
                throw new FingertipsException("More than 1 comparator with same ID where only 1 expected.");
            }
            return result.FirstOrDefault();
        }

        public ComparatorDetails GetComparatorById(int id)
        {
            IEnumerable<ComparatorDetails> result = comparators.Where(c => c.ComparatorId == id);
            if (result.Count() > 1)
            {
                throw new FingertipsException("More than 1 comparator with same ID where only 1 expected.");
            }
            return result.FirstOrDefault();
        }

        public ComparatorDetails GetSubnationalComparator()
        {
            return GetComparatorById(ComparatorIds.Subnational);
        }

        public ComparatorDetails GetRegionalComparatorByRegion(ParentArea parentArea)
        {
            return comparators.FirstOrDefault(comparator => comparator.ComparatorId == ComparatorIds.Subnational &&
                                                            comparator.Area.Code == parentArea.AreaCode &&
                                                            comparator.ChildAreaTypeId == parentArea.ChildAreaTypeId);
        }

        public ComparatorDetails GetNationalComparator()
        {
            return comparators.FirstOrDefault(comparator => comparator.ComparatorId == ComparatorIds.England);
        }
    }
}
