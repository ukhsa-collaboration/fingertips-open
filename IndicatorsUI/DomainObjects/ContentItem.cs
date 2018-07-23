using System.Web;

namespace IndicatorsUI.DomainObjects
{
    public class ContentItem
    {
        public int Id { get; set; }
        public string ContentKey { get; set; }
        public int ProfileId { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public HtmlString HtmlEncodedString
        {
            get { return new HtmlString(HttpUtility.HtmlDecode(Content)); }
        }
    }
}
