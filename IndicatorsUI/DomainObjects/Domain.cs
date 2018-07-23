namespace IndicatorsUI.DomainObjects
{
    public class Domain
    {
        public int GroupId { get; internal set; }
        public int Number { get; internal set; }
        public string SubHeading { get; internal set; }
        public string Tooltip { get; internal set; }
        public int ProfileId { get; set; }
    }
}
