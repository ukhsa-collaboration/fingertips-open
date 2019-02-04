using System;
using System.Collections.Generic;
using NHibernate;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess.Repositories
{
    public class FpmIndicatorMetadataTextValueRepository : RepositoryBase
    {
        public FpmIndicatorMetadataTextValueRepository() : this(NHibernateSessionFactory.GetSession())
        {
        }

        public FpmIndicatorMetadataTextValueRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public void ReplaceIndicatorMetadataTextValues(IList<IndicatorMetadataTextValue> indicatorMetadataTextValues)
        {
            ITransaction transaction = null;

            try
            {
                // Begin transaction
                transaction = CurrentSession.BeginTransaction();

                // Loop through the indicator meta data text values and save them
                foreach (IndicatorMetadataTextValue indicatorMetadataTextValue in indicatorMetadataTextValues)
                {
                    CurrentSession.SaveOrUpdate(indicatorMetadataTextValue);
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
