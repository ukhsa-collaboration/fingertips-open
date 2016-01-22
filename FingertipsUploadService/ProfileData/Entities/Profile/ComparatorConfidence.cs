namespace FingertipsUploadService.ProfileData.Entities.Profile
{
    public class ComparatorConfidence
    {
        public int ComparatorMethodId { get; set; }
        public double ConfidenceValue { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as ComparatorConfidence;

            if (other != null)
            {
                return ComparatorMethodId == other.ComparatorMethodId &&
                    ConfidenceValue.Equals(other.ConfidenceValue);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (ComparatorMethodId.ToString() + ConfidenceValue.ToString()).GetHashCode();
        }
    }
}