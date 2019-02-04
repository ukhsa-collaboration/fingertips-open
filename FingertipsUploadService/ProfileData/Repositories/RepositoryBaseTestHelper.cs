using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace FingertipsUploadService.ProfileData.Repositories
{
    public class RepositoryBaseTestHelper : RepositoryBase
    {
        public RepositoryBaseTestHelper(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public void AddTransaction(ITransaction newTransaction)
        {
            transaction = newTransaction;
        }
    }
}
