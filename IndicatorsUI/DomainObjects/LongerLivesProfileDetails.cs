using System.Collections.Generic;

namespace Profiles.DomainObjects
{
    public class LongerLivesProfileDetails
    {
        public int ProfileId { get; set; }
        public int SupportingProfileId { get; set; }
        public int DomainsToDisplay { get; set; }
        public bool HasPracticeData { get; set; }
        public string ExtraJsFiles { get; set; }
        public string Title { get; set; }       
        public ProfileDetails SupportingProfileDetails { get; set; }

    }
}