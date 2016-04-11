using System;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.ProfileData
{
    public class DefaultProfileContentWriter
    {
        public const string ContentDescription = "<p>Please change this description in FPM</p>";
        public const string ContentIntroduction = "<h2>Introduction</h2><p>Please change this introduction in FPM</p>";
        public const string ContentRecentUpdates = "<h3>Jun 2016</h3><p>First version released</p>";

        private readonly ProfilesWriter profilesWriter;
        private int profileId;
        private string userName;

        public DefaultProfileContentWriter(ProfilesWriter profilesWriter, int profileId, string userName)
        {
            this.profilesWriter = profilesWriter;
            this.profileId = profileId;
            this.userName = userName;
        }

        public void CreateContentItems()
        {
            CreateContentItem(ContentKeys.Description, "Description", ContentDescription);
            CreateContentItem(ContentKeys.Introduction, "Introduction", ContentIntroduction);
            CreateContentItem(ContentKeys.RecentUpdates, "Recent updates", ContentRecentUpdates);
        }

        private void CreateContentItem(string tag, string description, string content)
        {
            var contentItem = profilesWriter.NewContentItem(profileId,tag, description, false, content);

            AuditContentChange(contentItem);
        }

        private void AuditContentChange(ContentItem contentItem)
        {
            profilesWriter.NewContentAudit(new ContentAudit
            {
                ContentKey = contentItem.ContentKey,
                ToContent = contentItem.Content,
                ContentId = contentItem.Id,
                UserName = userName,
                Timestamp = DateTime.Now
            });
        }
    }
}