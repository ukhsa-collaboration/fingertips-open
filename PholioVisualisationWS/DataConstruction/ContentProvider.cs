using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.Formatting;

namespace PholioVisualisation.DataConstruction
{
    public interface IContentProvider
    {
        string GetContentWithHtmlRemoved(int profileId, string contentKey);
    }

    public class ContentProvider : IContentProvider
    {
        private readonly IContentItemRepository _repository;
        private readonly HtmlCleaner _htmlCleaner;

        public ContentProvider(IContentItemRepository repository, HtmlCleaner htmlCleaner)
        {
            _repository = repository;
            _htmlCleaner = htmlCleaner;
        }

        public string GetContentWithHtmlRemoved(int profileId, string contentKey)
        {
            var contentItem = _repository.GetContentForProfile(profileId, contentKey);
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