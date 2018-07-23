using System;
using System.Collections.Generic;
using Fpm.ProfileData.Entities.LookUps;
using NHibernate;

namespace Fpm.ProfileData.Repositories
{
    public interface IAreaTypeRepository
    {
        IList<AreaType> GetAllAreaTypes();
        AreaType GetAreaType(int areaTypeId);
        IList<AreaTypeComponent> GetAreaTypeComponents(int parentAreaTypeId);
        void SaveAreaType(AreaType areaType);
    }

    public class AreaTypeRepository : RepositoryBase, IAreaTypeRepository
    {
        public AreaTypeRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public AreaTypeRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public IList<AreaType> GetAllAreaTypes()
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

        public IList<AreaTypeComponent> GetAreaTypeComponents(int parentAreaTypeId)
        {
            return CurrentSession.QueryOver<AreaTypeComponent>()
                .Where(x => x.AreaTypeId == parentAreaTypeId)
                .Cacheable()
                .List();
        }

        public void SaveAreaType(AreaType areaType)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                DeleteAreaTypeComponents(areaType);
                CurrentSession.Save(areaType);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        /// <summary>
        /// Delete existing area type components that are no longer required
        /// </summary>
        private void DeleteAreaTypeComponents(AreaType areaType)
        {
            if (areaType.ComponentAreaTypesToDelete != null)
            {
                foreach (var areaTypeComponent in areaType.ComponentAreaTypesToDelete)
                {
                    CurrentSession.Delete(areaTypeComponent);
                }
            }
        }
    }
}