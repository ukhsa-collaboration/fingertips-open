
using System;
using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupDataAtDataPointRepository
    {
        public bool AssignAreas = true;
        public bool AssignChildAreaData = true;

        public GroupData GetGroupData(string parentAreaCode, int childAreaTypeId, int profileId, int groupId)
        {
            var parentArea = new ParentArea(parentAreaCode, childAreaTypeId);

            ComparatorMap comparatorMap = new ComparatorMapBuilder(parentArea).ComparatorMap;

            return new GroupDataBuilderByGroupings
                {
                    AssignAreas = AssignAreas,
                    AssignChildAreaData = AssignChildAreaData,
                    GroupId = groupId,
                    ProfileId = profileId,
                    ComparatorMap = comparatorMap,
                    ChildAreaTypeId = childAreaTypeId
                }.Build();
        }

        public GroupData GetGroupDataProcessed(string parentAreaCode, int childAreaTypeId, int profileId, int groupId)
        {
            GroupData data = GetGroupData(parentAreaCode, childAreaTypeId, profileId, groupId);

            var targetComparerProvider = new TargetComparerProvider(ReaderFactory.GetGroupDataReader(), ReaderFactory.GetAreasReader());

            new GroupDataProcessor(targetComparerProvider).Process(data);
            return data;
        }
    }
}