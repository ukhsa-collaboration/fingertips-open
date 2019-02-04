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

        /// <summary>
        /// Open and close session before and after query
        /// </summary>
        /// <typeparam name="T">Type of returning</typeparam>
        /// <param name="query">Code with the query to execute</param>
        /// <returns>Any "T" type</returns>
        public T SecureExecuteQuery<T>(Func<T> query)
        {
            try
            {
                OpenSession();

                var queryResult = query.Invoke();

                CloseSession();
                
                return queryResult;
            }
            catch (Exception exception)
            {
                HandleException(exception);
                throw;
            }
        }

        /// <summary>
        /// Open and close session before and after a transaction
        /// </summary>
        /// <typeparam name="I">Type passing parameter</typeparam>
        /// <typeparam name="T">Type of returning</typeparam>
        /// <param name="sqlAction">Action to execute</param>
        /// <param name="data">Data that it wants to be the action</param>
        /// <returns></returns>
        public T SecureExecuteSqlAction<I,T>(Func<I,T> sqlAction, I data)
        {
            try
            {
                OpenSession();

                var queryResult = sqlAction.Invoke(data);

                CloseSession();
                
                return queryResult;
            }
            catch (Exception exception)
            {
                HandleException(exception);
                throw;
            }
        }

        /// <summary>
        /// Open and close session before and after transaction
        /// </summary>
        /// <param name="transaction">Code within transactions</param>
        public void SecureExecuteTransaction(Action transaction)
        {
            try
            {
                OpenSession();

                transaction.Invoke();

                CloseSession();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
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