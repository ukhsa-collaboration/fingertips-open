using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    /// <summary>
    /// Returns list of available parent types for areas.
    /// </summary>
    public class JsonBuilderParentAreaGroups : JsonBuilderBase
    {
        private ParentAreaGroupsParameters parameters;

        public JsonBuilderParentAreaGroups(HttpContextBase context)
            : base(context)
        {
            parameters = new ParentAreaGroupsParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            var areasReader = ReaderFactory.GetAreasReader();
            var profileId = parameters.GetNonSearchProfileId();
            var parentAreaGroups = areasReader.GetParentAreaGroups(profileId);

            if (parentAreaGroups.Any() == false)
            {
                parentAreaGroups = areasReader.GetParentAreaGroups(ProfileIds.Undefined);
            }

            var organiser = new ParentAreaGroupOrganiser(parentAreaGroups, ReaderFactory.GetAreasReader());

            return JsonConvert.SerializeObject(organiser.ChildAreaTypeIdToParentArea);
        }

    }
}