using System;
using System.Collections.Generic;
using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Entities.User;
using NHibernate;

namespace Fpm.ProfileData.Repositories
{
    public class AreaTypeRepository : RepositoryBase
    {
        public AreaTypeRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public AreaTypeRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public IEnumerable<AreaType> GetAllAreaTypes()
        {
            return CurrentSession.QueryOver<AreaType>()
                .OrderBy(x => x.ShortName).Asc
                .List();
        }

        public AreaType GetAreaType(int areaTypeId)
        {
            return CurrentSession.QueryOver<AreaType>()
                .Where(x => x.Id == areaTypeId)
                .SingleOrDefault();
        }

        public void SaveAreaType(AreaType areaType)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.Save(areaType);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

    }
}