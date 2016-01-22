
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupRootFilter
    {
        private List<GroupRoot> rootsToKeep;

        public GroupRootFilter(IEnumerable<GroupRoot> groupRoots)
        {
            rootsToKeep = groupRoots.ToList();
        }

        public IList<GroupRoot> RemoveRootsWithoutChildAreaData()
        {
            List<GroupRoot> rootsToRemove = rootsToKeep.Where(root => root.Data.Count == 0).ToList();

            foreach (GroupRoot groupRoot in rootsToRemove)
            {
                rootsToKeep.Remove(groupRoot);
            }

            return rootsToKeep;
        }

        public IList<GroupRoot> KeepRootsWithIndicatorIds(IEnumerable<int> indicatorIds)
        {
            return rootsToKeep.Where(x => indicatorIds.Contains(x.IndicatorId)).ToList();
        }
    }
}
