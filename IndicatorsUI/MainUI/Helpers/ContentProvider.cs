using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Exceptions;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUI.Helpers
{
    public static class ContentProvider
    {
        private static ProfileReader profileReader = ReaderFactory.GetProfileReader();

        /// <summary>
        /// If not content item is defined then an empty HtmlString is returned.
        /// </summary>
        public static HtmlString GetContent(string contentKey, int profileId)
        {
            return GetWithRetry(profileId, contentKey);
        }

        /// <summary>
        /// If not content item is defined then an empty HtmlString is returned.
        /// </summary>
        public static ContentItem GetContentItem(string contentKey, int profileId)
        {
            profileReader.IsContentCachedInMemory = AppConfig.Instance.IsContentCachedInMemory;

            return profileReader.GetContentItem(contentKey, profileId);
        }

        /// <summary>
        /// Get recent updates
        /// </summary>
        public static HtmlString GetRecentUpdates(int profileId)
        {
            // Fails frequently on live so retries
            return GetWithRetry(profileId, ContentKeys.RecentUpdates);
        }

        /// <summary>
        /// Get the introduction
        /// </summary>
        public static HtmlString GetIntroduction(int profileId)
        {
            // Fails frequently on live so retries
            return GetWithRetry(profileId, ContentKeys.Introduction);
        }


        private static HtmlString GetWithRetry(int profileId, string contentKey)
        {
            HtmlString recentUpdates = new HtmlString("");
            var retryCount = 3;
            while (retryCount > 0)
            {
                try
                {
                    recentUpdates = GetContentString(contentKey, profileId);
                    break;
                }
                catch (GenericADOException)
                {
                }
                retryCount--;
            }
            return recentUpdates;
        }

        public static HtmlString GetContentString(string contentKey, int profileId)
        {
            var contentItem = GetContentItem(contentKey, profileId);

            return contentItem == null
              ? new HtmlString(string.Empty)
              : contentItem.HtmlEncodedString;
        }
    }
}