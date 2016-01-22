using System.Collections;
using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class AreasOfAreaTypeAction
    {
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();
        private AreaListBuilder areaListBuilder;

        public IList<IArea> GetResponse(int areaTypeId, int profileId,
            int templateProfileId, bool retrieveIgnoredAreas)
        {
            areaListBuilder = new AreaListBuilder(areasReader);
            areaListBuilder.CreateAreaListFromAreaTypeId(profileId, areaTypeId);

            // Remove ignored areas
            if (retrieveIgnoredAreas == false)
            {
                var nonSearchProfileId = ActionHelper.GetNonSearchProfileId(profileId, templateProfileId);
                var ignoredAreasFilter = IgnoredAreasFilterFactory.New(nonSearchProfileId);
                areaListBuilder.RemoveAreasIgnoredEverywhere(ignoredAreasFilter);
            }

            areaListBuilder.SortByOrderOrName();
            return areaListBuilder.Areas;
        }
    }
}