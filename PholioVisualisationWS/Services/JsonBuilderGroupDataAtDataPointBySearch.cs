
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Services
{
    public class JsonBuilderGroupDataAtDataPointBySearch
    {
        private GroupDataBuilderBase _groupDataBuilder;

        public JsonBuilderGroupDataAtDataPointBySearch(GroupDataBuilderBase groupDataBuilder)
        {
            _groupDataBuilder = groupDataBuilder;
        }

        public IList<GroupRoot> GetGroupRoots()
        {
            var data = _groupDataBuilder.Build();

            var targetComparerProvider = new TargetComparerProvider(ReaderFactory.GetGroupDataReader(), ReaderFactory.GetAreasReader());
            new GroupDataProcessor(targetComparerProvider).Process(data);

            if (data.IsDataOk)
            {
                var groupDataReader = ReaderFactory.GetGroupDataReader();
                data.GroupRoots = new GroupRootFilter(groupDataReader).RemoveRootsWithoutChildAreaData(data.GroupRoots);
            }

            return data.GroupRoots ?? new List<GroupRoot>();
        }
    }
}