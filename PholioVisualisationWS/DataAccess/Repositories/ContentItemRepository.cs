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
                    var contentItemInDb = GetContentForProfile(contentItem.ProfileId, contentItem.ContentKey);

                    if (contentItemInDb == null)
                    {
                        // Insert new content item
                        CurrentSession.Save(contentItem);
                    }
                    else
                    {
                        // Update content item
                        contentItemInDb.Content = contentItem.Content;
                        contentItemInDb.Description = contentItem.Description;
                        contentItemInDb.IsPlainText = contentItem.IsPlainText;
                        CurrentSession.Update(contentItemInDb);
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
