using System;
using NHibernate;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{
    public class ExceptionWriter : BaseReader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sessionFactory">The session factory</param>
        public ExceptionWriter(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public void Save(ExceptionForStorage exception)
        {
            try
            {
                // Save new exception
                CurrentSession.Save(exception);
            }
            catch (Exception ex)
            {
                throw new FingertipsException("Save failed", ex);
            }
        }
    }
}
