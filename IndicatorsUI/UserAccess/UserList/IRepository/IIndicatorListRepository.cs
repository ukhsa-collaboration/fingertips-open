using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndicatorsUI.UserAccess.UserList.IRepository
{
    public interface IIndicatorListRepository : IRepository<IndicatorList,int>
    {
        IEnumerable<IndicatorList> GetTopIndicatorList(int listCount, string userId);
        IEnumerable<IndicatorList> GetAll(string userId);
        string GetListNameByPublicId(string publicId);
        IndicatorList GetListByPublicId(string publicId);
        bool DoesUserOwnList(string publicListId, string userId);
    }
}
