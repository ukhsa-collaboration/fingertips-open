using System;
using NHibernate;

namespace PholioVisualisation.DataAccess.Repositories
{
    public interface IRepositoryBase
    {
        /// <summary>
        /// Opens a data access session
        /// </summary>
        /// <exception cref="Exception">Thrown if an error occurs while opening the session</exception>
        void OpenSession();

        /// <summary>
        /// IDisposable.Dispose implementation (closes and disposes of the encapsulated session)
        /// </summary>
        void Dispose();
    }

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

        protected int SaveNewObject(object objectToSave)
        {
            if (objectToSave == null)
            {
                return -1;
            }

            int id;
            ITransaction transaction = null;
            try
            {
                transaction = CurrentSession.BeginTransaction();
                id = (int)CurrentSession.Save(objectToSave);
                transaction.Commit();
            }
            catch (Exception)
            {
                if (transaction != null && transaction.WasRolledBack == false)
                {
                    transaction.Rollback();
                }
                throw;
            }
            return id;
        }

        protected void DeleteObject(object objectToDelete)
        {
            if (objectToDelete != null)
            {
                ITransaction transaction = null;
                try
                {
                    transaction = CurrentSession.BeginTransaction();
                    CurrentSession.Delete(objectToDelete);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    if (transaction != null && transaction.WasRolledBack == false)
                    {
                        transaction.Rollback();
                    }
                    throw;
                }
            }
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