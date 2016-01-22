
using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate;

namespace Fpm.ProfileData
{
    public class BaseReader : IDisposable
    {
        /// <summary>
        /// The session factory, used to create new sessions as required
        /// </summary>
        private ISessionFactory sessionFactory;

        /// <summary>
        /// The current session (if any)
        /// </summary>
        internal ISession CurrentSession;

        /// <summary>
        /// Parameterless constructor to enable mocking.
        /// </summary>
        protected BaseReader()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sessionFactory">The session factory</param>
        internal BaseReader(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
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

        /// <summary>
        /// IDisposable.Dispose implementation (closes and disposes of the encapsulated session)
        /// </summary>
        public void Dispose()
        {
            CloseSession();
        }


    }
}
