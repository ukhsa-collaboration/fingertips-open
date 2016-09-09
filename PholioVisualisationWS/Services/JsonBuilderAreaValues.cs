using System.Collections.Generic;
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
        private AreaValuesParameters _parameters;

        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        private IProfileReader profileReader = ReaderFactory.GetProfileReader();

        public JsonBuilderAreaValues(HttpContextBase context)
            : base(context)
        {
            _parameters = new AreaValuesParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderAreaValues(AreaValuesParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            var values = GetValues();
            return JsonConvert.SerializeObject(values);
        }

        public IList<CoreDataSet> GetValues()
        {
            var profileId = GetProfileId();
            var grouping = GetGrouping(profileId);

            var indicatorComparerFactory = new IndicatorComparerFactory
            {
                PholioReader = ReaderFactory.GetPholioReader()
            };

            ChildAreaValuesBuilder builder = new ChildAreaValuesBuilder(indicatorComparerFactory, groupDataReader,
                ReaderFactory.GetAreasReader(), profileReader)
            {
                AreaTypeId = _parameters.AreaTypeId,
                ParentAreaCode = _parameters.ParentAreaCode,
                DataPointOffset = _parameters.DataPointOffset,
                ComparatorId = _parameters.ComparatorId,
                RestrictToProfileId = GetProfileId()
            };

            return builder.Build(grouping);
        }

        private int GetProfileId()
        {
            return _parameters.RestrictToProfileId != -1
                ? _parameters.RestrictToProfileId
                : _parameters.ProfileId;
        }

        private Grouping GetGrouping(int profileId)
        {
            if (_parameters.GroupIds.Count > 1)
            {
                throw new FingertipsException("Only one group ID at a time allowed");
            }

            var groupId = _parameters.GroupIds.First();

            GroupIdProvider groupIdProvider = new GroupIdProvider(profileReader);
            var grouping = new SingleGroupingProvider(groupDataReader, groupIdProvider)
                .GetGroupingByProfileIdAndGroupIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(profileId, groupId, _parameters.AreaTypeId, 
                    _parameters.IndicatorId, _parameters.SexId, _parameters.AgeId);
            return grouping;
        }
    }
}