using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderAreaValues : JsonBuilderBase
    {
        private AreaValuesParameters parameters;

        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        private IProfileReader profileReader = ReaderFactory.GetProfileReader();

        public JsonBuilderAreaValues(HttpContextBase context)
            : base(context)
        {
            parameters = new AreaValuesParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            var profileId = GetProfileId();

            var grouping = GetGrouping(profileId);
            object values = null;

            var indicatorComparerFactory = new IndicatorComparerFactory
                {
                    PholioReader = ReaderFactory.GetPholioReader()
                };

            ChildAreaValuesBuilder builder = new ChildAreaValuesBuilder(indicatorComparerFactory, groupDataReader,
                ReaderFactory.GetAreasReader(), profileReader)
                {
                    AreaTypeId = parameters.AreaTypeId,
                    ParentAreaCode = parameters.ParentAreaCode,
                    DataPointOffset = parameters.DataPointOffset,
                    ComparatorId = parameters.ComparatorId,
                    RestrictToProfileId = GetProfileId()
                };

            values = builder.Build(grouping);

            return JsonConvert.SerializeObject(values);
        }

        private int GetProfileId()
        {
            return parameters.RestrictToProfileId != -1
                ? parameters.RestrictToProfileId
                : parameters.ProfileId;
        }

        private Grouping GetGrouping(int profileId)
        {
            if (parameters.GroupIds.Count > 1)
            {
                throw new FingertipsException("Only one group ID at a time allowed");
            }

            var groupId = parameters.GroupIds.First();

            GroupIdProvider groupIdProvider = new GroupIdProvider(profileReader);
            var grouping = new SingleGroupingProvider(groupDataReader, groupIdProvider)
                .GetGrouping(profileId, groupId, parameters.AreaTypeId, 
                    parameters.IndicatorId, parameters.SexId, parameters.AgeId);
            return grouping;
        }
    }
}