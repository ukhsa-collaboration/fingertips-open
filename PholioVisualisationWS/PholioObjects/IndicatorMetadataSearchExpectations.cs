namespace PholioVisualisation.PholioObjects
{
    public class IndicatorMetadataSearchExpectation
    {
        public int IndicatorId { get; set; }
        public string SearchTerm { get; set; }
        public bool Expectation { get; set; }

        public override bool Equals(object obj)
        {
            IndicatorMetadataSearchExpectation other = obj as IndicatorMetadataSearchExpectation;

            if (other != null)
            {
                return IndicatorId == other.IndicatorId &&
                    SearchTerm == other.SearchTerm;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (SearchTerm + IndicatorId).GetHashCode();
        }

    }
}