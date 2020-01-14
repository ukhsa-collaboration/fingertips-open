using System;
using System.Collections.Generic;
using Fpm.ProfileData.Entities.Exceptions;

namespace Fpm.ProfileData.Repositories
{
    public interface IExceptionsRepository
    {
        IList<ExceptionLog> GetExceptionsForSpecificServers(int exceptionDays, params string[] exceptionServers);
        IList<ExceptionLog> GetExceptionsForAllServers(int exceptionDays);
        ExceptionLog GetException(int exceptionId);
        IEnumerable<string> GetDistinctExceptionServers();

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
}