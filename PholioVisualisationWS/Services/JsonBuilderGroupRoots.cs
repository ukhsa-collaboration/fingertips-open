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
            var groupings = ReaderFactory.GetGroupDataReader().GetGroupingsByGroupIdAndAreaTypeIdOrderedBySequence(
                _parameters.GroupId, _parameters.AreaTypeId);
            return new GroupRootBuilder().BuildGroupRoots(groupings);
        }
    }
}