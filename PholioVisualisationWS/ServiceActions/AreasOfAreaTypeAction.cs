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
        private AreaListProvider _areaListProvider;

        public IList<IArea> GetResponse(int areaTypeId, int profileId,
            int templateProfileId, bool retrieveIgnoredAreas)
        {
            _areaListProvider = new AreaListProvider(areasReader);
            _areaListProvider.CreateAreaListFromAreaTypeId(profileId, areaTypeId);

            // Remove ignored areas
            if (retrieveIgnoredAreas == false)
            {
                var nonSearchProfileId = ActionHelper.GetNonSearchProfileId(profileId, templateProfileId);
                var ignoredAreasFilter = IgnoredAreasFilterFactory.New(nonSearchProfileId);
                _areaListProvider.RemoveAreasIgnoredEverywhere(ignoredAreasFilter);
            }

            _areaListProvider.SortByOrderOrName();
            return _areaListProvider.Areas;
        }
    }
}