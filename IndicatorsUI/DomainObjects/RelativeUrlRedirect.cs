namespace IndicatorsUI.DomainObjects
{
    public class RelativeUrlRedirect
    {
        public int Id { get; set; }
        public string FromUrl { get; set; }
        public string ToUrl { get; set; }
        public bool useFingertipsHostnameForAbsolutePath { get; set; }
    }
}