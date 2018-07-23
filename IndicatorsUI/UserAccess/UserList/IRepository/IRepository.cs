using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndicatorsUI.UserAccess.UserList.IRepository
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        TEntity Get(TKey id);
        TKey Save(TEntity entity);
        void Delete(TKey id);
        void Update(TEntity indicatorList);
    }
}
