using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.ServiceActions;

namespace PholioVisualisation.Services
{
    /// <summary>
    /// Returns list of available parent types for areas.
    /// </summary>
    public class JsonBuilderParentAreaGroups : JsonBuilderBase
    {
        private ParentAreaGroupsParameters _parameters;

        public JsonBuilderParentAreaGroups(HttpContextBase context)
            : base(context)
        {
            _parameters = new ParentAreaGroupsParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderParentAreaGroups(ParentAreaGroupsParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            return JsonConvert.SerializeObject(GetChildAreaTypeIdToParentAreaTypes());
        }

        public IList<IAreaType> GetChildAreaTypeIdToParentAreaTypes()
        {
            var areasReader = ReaderFactory.GetAreasReader();
            var profileId = _parameters.GetNonSearchProfileId();

            // Get parent area groups
            var parentAreaGroups = areasReader.GetParentAreaGroupsForProfile(profileId);
            if (parentAreaGroups.Any() == false)
            {
                parentAreaGroups = areasReader.GetParentAreaGroupsForProfile(ProfileIds.Undefined);
            }

            // Limit to supported area types for that profile
            if (profileId != ProfileIds.Undefined)
            {
                parentAreaGroups = FilterParentAreaGroupsToThoseUsedInProfile(profileId, parentAreaGroups);
            }

            var organiser = new AreaTypesWithParentAreaTypesBuilder(parentAreaGroups, ReaderFactory.GetAreasReader());

            return organiser.ChildAreaTypesWithParentAreaTypes.Cast<IAreaType>().ToList();
        }

        private static IList<ParentAreaGroup> FilterParentAreaGroupsToThoseUsedInProfile(int profileId, IList<ParentAreaGroup> parentAreaGroups)
        {
            var usedAreaTypeIds = new AreaTypesAction().GetResponse(new List<int> {profileId})
                .Select(x => x.Id);

            parentAreaGroups = parentAreaGroups
                .Where(x => usedAreaTypeIds.Contains(x.ChildAreaTypeId))
                .ToList();
            return parentAreaGroups;
        }
    }
}