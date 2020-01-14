using System;

namespace IndicatorsUI.DomainObjects
{
    public class Email
    {
        public int Id { get; set; }
        public string To { get; set; }
        public string TemplateId { get; set; }
        public string TemplateParameters { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime? SentTimestamp { get; set; }
        public int RetryCount { get; set; }
        public string Content { get; set; }
        public string ErrorMessage { get; set; }
    }
}
