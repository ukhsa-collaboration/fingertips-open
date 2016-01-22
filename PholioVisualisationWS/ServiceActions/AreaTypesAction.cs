using System.Collections.Generic;
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
            return new AreaTypeListProvider(new GroupIdProvider(profileReader), areasReader, groupDataReader)
                .GetChildAreaTypesUsedInProfiles(profileIds);
        }
    }
}