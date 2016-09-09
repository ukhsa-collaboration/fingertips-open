
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupRootFilter
    {
        private IGroupDataReader _groupDataReader;

        public GroupRootFilter(IGroupDataReader groupDataReader)
        {
            _groupDataReader = groupDataReader;
        }

        public IList<GroupRoot> RemoveRootsWithoutChildAreaData(IEnumerable<GroupRoot> groupRoots)
        {
            var rootsToKeep = new List<GroupRoot>();

            foreach (GroupRoot groupRoot in groupRoots)
            {
                Debug.WriteLine(_groupDataReader.GetCoreDataCountAtDataPoint(groupRoot.FirstGrouping));

                if (_groupDataReader.GetCoreDataCountAtDataPoint(groupRoot.FirstGrouping) > 0)
                {
                    rootsToKeep.Add(groupRoot);
                }
            }

            return rootsToKeep;
        }

        public IList<GroupRoot> KeepRootsWithIndicatorIds(IEnumerable<GroupRoot> groupRoots, IEnumerable<int> indicatorIds)
        {
            return groupRoots.Where(x => indicatorIds.Contains(x.IndicatorId)).ToList();
        }
    }
}
