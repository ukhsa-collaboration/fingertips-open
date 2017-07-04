using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;

namespace PholioVisualisation.Export
{
    public interface IIndicatorIdListProvider
    {
        IList<int> GetIdsForGroup(int groupId);
        IList<int> GetIdsForProfile(int profileId);
    }

    public class IndicatorIdListProvider : IIndicatorIdListProvider
    {
        private readonly IGroupDataReader _groupDataReader;
        private readonly IGroupIdProvider _groupIdProvider;

        public IndicatorIdListProvider(IGroupDataReader groupDataReader, IGroupIdProvider groupIdProvider)
        {
            _groupDataReader = groupDataReader;
            _groupIdProvider = groupIdProvider;
        }

        public IList<int> GetIdsForGroup(int groupId)
        {
            return _groupDataReader
                .GetGroupingsByGroupId(groupId)
                .Select(x => x.IndicatorId)
                .Distinct()
                .ToList();
        }

        public IList<int> GetIdsForProfile(int profileId)
        {
            var indicatorIds = new List<int>();

            var groupIds = _groupIdProvider.GetGroupIds(profileId);
            foreach (var groupId in groupIds)
            {
                var ids = _groupDataReader
                    .GetGroupingsByGroupId(groupId)
                    .Select(x => x.IndicatorId);
                indicatorIds.AddRange(ids);
            }

            return indicatorIds.Distinct().ToList();
        }
    }
}