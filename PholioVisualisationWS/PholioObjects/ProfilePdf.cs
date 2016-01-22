namespace PholioVisualisation.PholioObjects
{
    /// <summary>
    /// Presence of this object indicates that a PDF
    /// can be generated for a particular profile.
    /// </summary>
    public class ProfilePdf
    {
        public int ProfileId { get; set; }
        public int AreaTypeId { get; set; }

        public override bool Equals(object obj)
        {
            ProfilePdf other = obj as ProfilePdf;

            if (other != null)
            {
                return ProfileId == other.ProfileId &&
                    AreaTypeId == other.AreaTypeId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ProfileId.GetHashCode() ^
                AreaTypeId.GetHashCode();
        }
    }
}