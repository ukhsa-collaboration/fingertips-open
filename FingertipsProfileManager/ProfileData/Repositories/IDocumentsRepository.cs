using System;
using Fpm.ProfileData.Entities.Documents;

namespace Fpm.ProfileData.Repositories
{
    public interface IDocumentsRepository
    {
        DocumentContent GetDocumentContent(int documentId);

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