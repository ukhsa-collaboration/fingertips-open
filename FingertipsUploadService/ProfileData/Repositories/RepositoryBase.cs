using NHibernate;
using System;

namespace FingertipsUploadService.ProfileData.Repositories
{
    public class RepositoryBase : IRepositoryBase, IDisposable
    {
        /// <summary>
        /// The session factory, used to create new sessions as required
        /// </summary>
        private ISessionFactory sessionFactory;

        /// <summary>
        /// The current session (if any)
        /// </summary>
        internal ISession CurrentSession;

        internal ITransaction transaction = null;

        public RepositoryBase(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
            OpenSession();
        }

        /// <summary>
        /// Opens a data access session
        /// </summary>
        /// <exception cref="Exception">Thrown if an error occurs while opening the session</exception>
        public void OpenSession()
        {
            if (CurrentSession == null)
            {
                try
                {
                    CurrentSession = sessionFactory.OpenSession();
                }
                catch (Exception ex)
                {
                    CurrentSession = null;
                    throw new Exception("Cannot open session", ex);
                }
            }
        }

        /// <summary>
        /// IDisposable.Dispose implementation (closes and disposes of the encapsulated session)
        /// </summary>
        public void Dispose()
        {
            CloseSession();
        }

        internal void HandleException(Exception exception)
        {
            if (transaction != null && transaction.WasRolledBack == false)
            {
                transaction.Rollback();
            }
            throw exception;
        }

        /// <summary>
        /// Closes a data access session
        /// </summary>
        private void CloseSession()
        {
            // Can always close (used by Dipose method, so mustn't throw an exception)
            if (CurrentSession != null && CurrentSession.IsOpen)
            {
                CurrentSession.Close();
                CurrentSession.Dispose();
                CurrentSession = null;
            }
        }
    }
}