using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataSorting;

namespace PholioVisualisation.DataConstruction
{
    public class ProfileIdListProvider
    {
        private IProfileReader _profileReader;

        public ProfileIdListProvider(IProfileReader profileReader)
        {
            _profileReader = profileReader;
        }

        public IList<int> GetSearchableProfileIds()
        {
            var ids = _profileReader.GetAllProfileIds();
            return ProfileFilter.RemoveSystemProfileIds(ids);
        }
    }
}