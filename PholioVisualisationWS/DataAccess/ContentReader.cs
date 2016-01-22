using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{
    public interface IContentReader
    {
        ContentItem GetContent(string contentKey);
        ContentItem GetContentForProfile(int profileId, string contentKey);
    }

    public class ContentReader : BaseReader, IContentReader
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="sessionFactory">The session factory</param>
        internal ContentReader(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public ContentItem GetContent(string contentKey)
        {
            return CurrentSession.CreateCriteria<ContentItem>()
                .Add(Restrictions.Eq("ContentKey", contentKey))
                .UniqueResult<ContentItem>();
        }

        public ContentItem GetContentForProfile(int profileId, string contentKey)
        {
            return CurrentSession.CreateCriteria<ContentItem>()
                .Add(Restrictions.Eq("ContentKey", contentKey))
                .Add(Restrictions.Eq("ProfileId", profileId))
                .UniqueResult<ContentItem>();
        }

    }
}
