using PholioVisualisation.UserData.Repositories;

namespace PholioVisualisation.UserData.AreaLists
{
    public class AreaListProvider
    {
        private IAreaListRepository _areaListRepository;

        public AreaListProvider(IAreaListRepository areaListRepository)
        {
            _areaListRepository = areaListRepository;
        }
    }
}
