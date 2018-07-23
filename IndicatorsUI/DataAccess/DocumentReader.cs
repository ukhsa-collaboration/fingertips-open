using NHibernate;
using NHibernate.Criterion;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.DataAccess
{
    public class DocumentReader : BaseReader
    {
        internal DocumentReader(ISessionFactory sessionFactory) : base(sessionFactory)
        {
            
        }

        public Document GetDocument(string filename)
        {
            var document = CurrentSession.CreateCriteria<Document>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("FileName", filename))
                .UniqueResult<Document>();
            return document;
        }
    }
}
