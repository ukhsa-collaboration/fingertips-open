namespace PholioVisualisation.PholioObjects
{
    public class ContentItem
    {
        public int Id { get; set; }
        public string ContentKey { get; set; }
        public int ProfileId { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public bool IsPlainText { get; set; }
    }
}
