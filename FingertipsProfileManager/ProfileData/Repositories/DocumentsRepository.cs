using Fpm.ProfileData.Entities.Documents;
using NHibernate;

namespace Fpm.ProfileData.Repositories
{
    public class DocumentsRepository : RepositoryBase, IDocumentsRepository
    {
        public DocumentsRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public DocumentsRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public DocumentContent GetDocumentContent(int documentId)
        {
            var content = CurrentSession.GetNamedQuery("GetDocumentContent")
                .SetParameter("documentId", documentId)
                .UniqueResult<byte[]>();
            return new DocumentContent {Id = documentId, Content = content};
        }
    }
}