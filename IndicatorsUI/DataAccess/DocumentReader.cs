using NHibernate;
using NHibernate.Criterion;
using Profiles.DomainObjects;

namespace Profiles.DataAccess
{
    public class DocumentReader : BaseReader
    {
        internal DocumentReader(ISessionFactory sessionFactory) : base(sessionFactory)
        {
            
        }

        public Document GetDocument(string filename)
        {
            var document = CurrentSession.CreateCriteria<Document>()
                .Add(Restrictions.Eq("FileName", filename))
                .UniqueResult<Document>();
            return document;
        }
    }
}
