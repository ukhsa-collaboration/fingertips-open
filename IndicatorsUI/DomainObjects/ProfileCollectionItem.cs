namespace Profiles.DomainObjects
{
    public class ProfileCollectionItem
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int ProfileCollectionId { get; set; }
        public ProfileDetails ProfileDetails { get; set; }
        public bool DisplayDomains { get; set; }
        public string ExternalUrl { get; set; }
        public int Sequence { get; set; }

        public ProfileCollection ParentCollection { get; set; }

        public bool IsBeingDisplayedOnCollectionFrontPage
        {
            get
            {
                return ParentCollection != null && ParentCollection.IsCollectionFrontPage;
            }
        }
    }
}