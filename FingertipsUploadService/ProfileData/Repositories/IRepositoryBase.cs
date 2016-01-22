using System;

namespace FingertipsUploadService.ProfileData.Repositories
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
}