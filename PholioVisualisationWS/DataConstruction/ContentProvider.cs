using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;

namespace PholioVisualisation.DataConstruction
{
    public interface IContentProvider
    {
        string GetContentWithHtmlRemoved(int profileId, string contentKey);
    }

    public class ContentProvider : IContentProvider
    {
        private IContentReader _contentReader;
        private HtmlCleaner _htmlCleaner;

        public ContentProvider(IContentReader contentReader, HtmlCleaner htmlCleaner)
        {
            _contentReader = contentReader;
            _htmlCleaner = htmlCleaner;
        }

        public string GetContentWithHtmlRemoved(int profileId, string contentKey)
        {
            var contentItem = _contentReader.GetContentForProfile(profileId, contentKey);
            if (contentItem == null)
            {
                return string.Empty;
            }

            var content = contentItem.Content;
            if (contentItem.IsPlainText == false)
            {
                content = _htmlCleaner.RemoveHtml(content);
            }

            return content;
        }
    }
}