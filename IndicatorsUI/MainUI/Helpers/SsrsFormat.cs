namespace IndicatorsUI.MainUI.Helpers
{
    public class SsrsFormat
    {
        public string ContentType { get; set; }
        public string Extension{ get; set; }

        public SsrsFormat(string format)
        {
            if (format == "pdf")
            {
                ContentType = "application/pdf";
                Extension = "pdf";
            }
            else if (format == "word")
            {
                ContentType = "application/msword";
                Extension = "doc";
            }
        }
    }
}