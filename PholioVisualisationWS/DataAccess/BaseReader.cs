
using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{

    public class BaseReader : IDisposable
    {
        internal const string ParameterGroupId = "groupId";
        internal const string ParameterIndicatorId = "indicatorId";
        internal const string ParameterSexId = "sexId";
        internal const string ParameterAreaType = "areaTypeId";

        /// <summary>
        /// The session factory, used to create new sessions as required
        /// </summary>
        private ISessionFactory sessionFactory;

        /// <summary>
        /// The current session (if any)
        /// </summary>
        internal ISession CurrentSession;

        /// <summary>
        /// Parameterless constructor to allow mocking
        /// </summary>
        internal BaseReader() { }

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
                    throw new FingertipsException("Cannot open session", ex);
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
