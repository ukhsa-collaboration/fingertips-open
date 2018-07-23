using System;
using System.Collections.Generic;
using NHibernate;
using PholioVisualisation.DataAccess.Repositories.Fpm.ProfileData.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess.Repositories
{
    public interface ICoreDataSetRepository
    {
        void Save(CoreDataSet coreDataSet);
    }

    public class CoreDataSetRepository : RepositoryBase, ICoreDataSetRepository
    {
        // poor man injection, should be removed when we use DI containers
        public CoreDataSetRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public CoreDataSetRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public void Save(CoreDataSet coreDataSet)
        {
            SaveNewObject(coreDataSet);
        }

        public void Delete(CoreDataSet coreDataSet)
        {
            DeleteObject(coreDataSet);
        }

        /// <summary>
        /// Method to replace the core data set for an indicator
        /// </summary>
        /// <param name="coreDataSets">List of core data set objects</param>
        public void ReplaceCoreDataSetForAnIndicator(IList<CoreDataSet> coreDataSets)
        {
            ITransaction transaction = null;

            try
            {
                // Begin transaction
                transaction = CurrentSession.BeginTransaction();

                // Delete the grouping corresponding to the indicator id
                IQuery q = CurrentSession.CreateQuery("delete from CoreDataSet c where c.IndicatorId = :indicatorId");
                q.SetParameter(BaseReader.ParameterIndicatorId, coreDataSets[0].IndicatorId);
                q.ExecuteUpdate();

                // Loop through the core data set and save them
                foreach (CoreDataSet coreDataSet in coreDataSets)
                {
                    CurrentSession.Save(coreDataSet);
                }

                // All went well, commit the transaction
                transaction.Commit();
            }
            catch (Exception)
            {
                // Something wrong, rollback the transaction
                if (transaction != null && transaction.WasRolledBack == false)
                {
                    transaction.Rollback();
                }

                // Throw the exception
                throw;
            }
        }
    }
}
