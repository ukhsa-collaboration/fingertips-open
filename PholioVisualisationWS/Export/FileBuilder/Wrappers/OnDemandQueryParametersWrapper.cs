using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using System.Collections.Generic;

namespace PholioVisualisation.Export.FileBuilder.Wrappers
{
    public class OnDemandQueryParametersWrapper
    {
        public IList<int> IndicatorIds { get; private set; }
        public IList<string> ChildAreaCodeList { get; private set; }
        public IList<int> GroupIds { get; private set; }
        public bool AllPeriods { get; private set; }
        public IDictionary<int, IList<Inequality>> Inequalities { get; private set; }

        public OnDemandQueryParametersWrapper(int profileId, IList<int> indicatorIds, IDictionary<int, IList<Inequality>> inequalities, IList<string> childAreaCodeList = null, IList<int> groupListIds = null,
            bool allPeriods = false)
        {
            IndicatorIds = indicatorIds;
            ChildAreaCodeList = childAreaCodeList;
            GroupIds = GetGroupIds(profileId, groupListIds);
            AllPeriods = allPeriods;
            Inequalities = inequalities;
        }

        private static IList<int> GetGroupIds(int profileId, IList<int> groupListIds = null)
        {
            return groupListIds == null
                ? new GroupIdProvider(ReaderFactory.GetProfileReader()).GetGroupIds(profileId)
                : groupListIds;
        }
    }

    public class Inequality
    {
        [JsonProperty("CategoryTypeId")]
        public int CategoryTypeId { get; set; }
        [JsonProperty("CategoryId")]
        public int CategoryId { get; set; }
        [JsonProperty("Sex")]
        public int? SexId { get; set; }
        [JsonProperty("Age")]
        public int? AgeId { get; set; }

        // Necessary for deserializing
        public Inequality()
        {
        }

        public Inequality(int categoryTypeId, int categoryId)
        {
            CategoryTypeId = categoryTypeId;
            CategoryId = categoryId;
        }

        public Inequality(int categoryTypeId, int categoryId, int sexId, int ageId)
        {
            CategoryTypeId = categoryTypeId;
            CategoryId = categoryId;
            SexId = sexId;
            AgeId = ageId;
        }
    }
}
