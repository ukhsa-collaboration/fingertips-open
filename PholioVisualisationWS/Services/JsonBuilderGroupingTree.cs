
using System.Collections.Generic;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderGroupingTree 
    {
        private GroupingTreeParameters _parameters;

        public JsonBuilderGroupingTree(GroupingTreeParameters parameters)
        {
            _parameters = parameters;
        }

        public IList<GroupingMetadata> GetGroupingMetadatas()
        {
            return new GroupMetadataBuilder(ReaderFactory.GetGroupDataReader()) { GroupIds = _parameters.GroupIds }
                .Build();
        }
    }
}