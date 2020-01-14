using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.Export.FileBuilder.Wrappers
{
    public class OnDemandQueryParametersWrapper
    {
        public IList<int> IndicatorIds { get; private set; }
        public IList<string> ChildAreaCodeList { get; private set; }
        public IList<int> GroupIds { get; private set; }
        public bool AllPeriods { get; private set; }
        public IDictionary<int, IList<InequalitySearch>> Inequalities { get; private set; }
        public int ProfileId { get; set; }
        public string[] CategoryAreaCode { get; set; }
        public bool AreQuinaryPopulations = false;

        public OnDemandQueryParametersWrapper(int profileId, IList<int> indicatorIds, IDictionary<int, IList<InequalitySearch>> inequalities, IList<string> childAreaCodeList = null, IList<int> groupListIds = null,
            bool allPeriods = false, string[] categoryAreaCode = null )
        {
            IndicatorIds = indicatorIds;
            ChildAreaCodeList = childAreaCodeList;
            GroupIds = GetGroupIds(profileId, groupListIds);
            AllPeriods = allPeriods;
            Inequalities = inequalities;
            ProfileId = profileId;
            CategoryAreaCode = categoryAreaCode;
        }

        public static Dictionary<int, IList<InequalitySearch>> GetInitializedInequalitiesDictionary(IList<int> indicatorIds)
        {
            return indicatorIds.ToDictionary<int, int, IList<InequalitySearch>>(iid => iid, iid => new List<InequalitySearch>());
        }

        private static IList<int> GetGroupIds(int profileId, IList<int> groupListIds = null)
        {
            return groupListIds == null
                ? new GroupIdProvider(ReaderFactory.GetProfileReader()).GetGroupIds(profileId)
                : groupListIds;
        }
    }
}
