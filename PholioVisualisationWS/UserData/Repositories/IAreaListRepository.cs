using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.UserData.Repositories
{
    public interface IAreaListRepository
    {
        bool DoesUserOwnList(string publicListId, string userId);
        AreaList Get(int id);
        AreaList GetAreaListByPublicId(string publicId);
        string GetUserIdByPublicId(string publicId);
        IEnumerable<AreaList> GetAll(string userId);
        IEnumerable<AreaListAreaCode> GetAreaListAreaCodes(int areaListId);
        int Save(AreaList areaList, IList<string> areaCodes);
        void Update(int areaListId, string areaListName, IList<string> areaCodes);
        void Delete(int id);
    }
}
