using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace Profiles.MainUI.Helpers
{
    public static class ContentProvider
    {
        private static ProfileReader profileReader = ReaderFactory.GetProfileReader();

        /// <summary>
        /// If not content item is defined then an empty HtmlString is returned.
        /// </summary>
        public static HtmlString GetContent(string contentKey, int profileId)
        {
            var contentItem = GetContentItem(contentKey, profileId);

            return contentItem == null
              ? new HtmlString(string.Empty)
              : contentItem.HtmlEncodedString;
        }

        /// <summary>
        /// If not content item is defined then an empty HtmlString is returned.
        /// </summary>
        public static ContentItem GetContentItem(string contentKey, int profileId)
        {
            profileReader.IsContentCachedInMemory = AppConfig.Instance.IsContentCachedInMemory;

            return profileReader.GetContentItem(contentKey, profileId);
        }
    }
}