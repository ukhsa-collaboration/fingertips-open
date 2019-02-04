using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess.Repositories
{
    public interface IContentItemRepository
    {
        ContentItem GetContentForProfile(int profileId, string contentKey);

        void SaveContentItems(IList<ContentItem> contentItem);
    }

    public class ContentItemRepository : RepositoryBase, IContentItemRepository
    {
        public ContentItemRepository() : this(NHibernateSessionFactory.GetSession())
        {
        }

        public ContentItemRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public ContentItem GetContentForProfile(int profileId, string contentKey)
        {
            return CurrentSession.CreateCriteria<ContentItem>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("ContentKey", contentKey))
                .Add(Restrictions.Eq("ProfileId", profileId))
                .UniqueResult<ContentItem>();
        }

        public void SaveContentItems(IList<ContentItem> contentItems)
        {
            ITransaction transaction = null;

            try
            {
                // Begin transaction
                transaction = CurrentSession.BeginTransaction();

                foreach (ContentItem contentItem in contentItems)
                {
                    var contentItemInDb = CurrentSession.CreateCriteria<ContentItem>()
                        .Add(Restrictions.Eq("ProfileId", contentItem.ProfileId))
                        .Add(Restrictions.Eq("ContentKey", contentItem.ContentKey))
                        .UniqueResult<ContentItem>();

                    if (contentItemInDb == null)
                    {
                        CurrentSession.GetNamedQuery("Insert_ContentItem")
                            .SetParameter("ContentKey", contentItem.ContentKey)
                            .SetParameter("ProfileId", contentItem.ProfileId)
                            .SetParameter("Description", contentItem.Description)
                            .SetParameter("Content", contentItem.Content)
                            .SetParameter("IsPlainText", contentItem.IsPlainText)
                            .ExecuteUpdate();
                    }
                    else
                    {
                        if (contentItemInDb.ProfileId == contentItem.ProfileId &&
                            contentItemInDb.ContentKey == contentItem.ContentKey)
                        {
                            IQuery q = CurrentSession.CreateQuery("update ContentItem f set f.Content = :Content where f.ProfileId = :ProfileId and f.ContentKey = :ContentKey");
                            q.SetParameter("Content", contentItem.Content);
                            q.SetParameter("ProfileId", contentItem.ProfileId);
                            q.SetParameter("ContentKey", contentItem.ContentKey);
                            q.ExecuteUpdate();
                        }
                        else
                        {
                            throw new Exception(string.Format("No match found for the profile id {0} and the content key {1}",
                                contentItem.ProfileId, contentItem.ContentKey));
                        }
                    }
                }

                // All went well, commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Something wrong, rollback the transaction
                if (transaction != null && transaction.WasRolledBack == false)
                {
                    transaction.Rollback();
                }

                // Throw the exception
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
