using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class AreaTypesAction
    {
        private readonly IAreasReader areasReader = ReaderFactory.GetAreasReader();
        private readonly IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        private readonly IProfileReader profileReader = ReaderFactory.GetProfileReader();

        public IList<AreaType> GetResponse(IList<int> profileIds)
        {
            var areaTypeListProvider = new AreaTypeListProvider(new GroupIdProvider(profileReader), areasReader, groupDataReader);

            if (profileIds.Any())
            {
                return areaTypeListProvider.GetChildAreaTypesUsedInProfiles(profileIds);
            }
            return areaTypeListProvider.GetAllAreaTypes();
        }

        public IList<AreaType> GetResponse()
        {
            var areaTypeListProvider = new AreaTypeListProvider(new GroupIdProvider(profileReader), areasReader, groupDataReader);
            var areaTypes = areaTypeListProvider.GetAllAreaTypesWithData();
            return areaTypes.OrderBy(x => x.ShortName).ToList();
        }
    }
}