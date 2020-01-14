using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class ValueLimitsAction
    {
        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();

        public IList<Limits> GetResponse(int groupId, int areaTypeId, string parentAreaCode)
        {
            var useNationalLimits = UseNationalLimits(parentAreaCode);

            var roots = GroupRoots(groupId, areaTypeId);

            var limitsList = new List<Limits>();
            foreach (var groupRoot in roots)
            {
                var indicatorId = groupRoot.IndicatorId;

                var limits = useNationalLimits ?
                    groupDataReader.GetCoreDataLimitsByIndicatorIdAndAreaTypeId(indicatorId, areaTypeId) :
                    groupDataReader.GetCoreDataLimitsByIndicatorIdAndAreaTypeIdAndParentAreaCode(indicatorId, areaTypeId, parentAreaCode);

                limitsList.Add(limits);
            }

            new LimitsProcessor().TruncateList(limitsList);

            return limitsList;
        }

        private bool UseNationalLimits(string parentAreaCode)
        {
            return parentAreaCode == null || areasReader.GetAreaFromCode(parentAreaCode).IsCountry;
        }

        private IList<GroupRoot> GroupRoots(int groupId, int areaTypeId)
        {
            var groupings = groupDataReader
                .GetGroupingsByGroupIdAndAreaTypeIdOrderedBySequence(groupId, areaTypeId);

            return new GroupRootBuilder(groupDataReader).BuildGroupRoots(groupings);
        }
    }
}
