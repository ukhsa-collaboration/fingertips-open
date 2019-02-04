using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class AreasOfAreaTypeAction
    {
        private readonly IAreasReader _areasReader = ReaderFactory.GetAreasReader();
        private AreaListProvider _areaListProvider;

        public IList<IArea> GetResponse(int areaTypeId, int profileId,
            int templateProfileId, bool retrieveIgnoredAreas, string userId)
        {
            _areaListProvider = new AreaListProvider(_areasReader);
            _areaListProvider.CreateAreaListFromAreaTypeId(profileId, areaTypeId, userId);

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

        public IArea GetResponse(int areaTypeId, int profileId, string userId, string parentCode)
        {
            _areaListProvider = new AreaListProvider(_areasReader);
            _areaListProvider.CreateAreaListFromAreaTypeId(profileId, areaTypeId, userId);

            return _areaListProvider.Areas.FirstOrDefault(x => x.Code == parentCode);
        }

        public IList<IArea> GetResponse(int areaTypeId, string areaNameSearchText)
        {
            _areaListProvider = new AreaListProvider(_areasReader);
            _areaListProvider.GetAreaListFromAreaTypeIdAndAreaNameSearchText(areaTypeId, areaNameSearchText);

            _areaListProvider.SortByOrderOrName();
            return _areaListProvider.Areas;
        }
    }
}