namespace Fpm.ProfileData.Entities.Profile
{
    public class ProfileCollectionItem
    {
        public int Id { get; set; }
        public int ProfileCollectionId { get; set; }
        public int ProfileId { get; set; }
        public ProfileDetails profileDetails { get; set; }
        public bool Selected { get; set; }
        public bool DisplayDomains { get; set; }
    }
}