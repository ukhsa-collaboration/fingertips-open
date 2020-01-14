using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderGroupRoots 
    {
        private GroupRootsParameters _parameters;

        public JsonBuilderGroupRoots(GroupRootsParameters parameters)
        {
            _parameters = parameters;
        }

        public IList<GroupRoot> GetGroupRoots()
        {
            var groupDataReader = ReaderFactory.GetGroupDataReader();
            var groupings = groupDataReader.GetGroupingsByGroupIdAndAreaTypeIdOrderedBySequence(
                _parameters.GroupId, _parameters.AreaTypeId);
            return new GroupRootBuilder(groupDataReader).BuildGroupRoots(groupings);
        }
    }
}