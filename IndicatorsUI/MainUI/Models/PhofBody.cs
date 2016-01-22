
using Profiles.DomainObjects;

namespace Profiles.MainUI.Models
{
    public class PhofBody
    {
        public string ContentKey { get; set; }
        public int ProfileId { get; set; }

        public PhofBody()
        {   
        }

        private PhofBody(string contentKey, int profileId)
        {
            ContentKey = contentKey;
            ProfileId = profileId;
        }

        public static PhofBody Faqs(int profileId)
        {
            return new PhofBody(ContentKeys.Faqs, profileId);
        }

        public static PhofBody FurtherInfo(int profileId)
        {
            return new PhofBody(ContentKeys.FurtherInfo, profileId);
        }

        public static PhofBody ContactUs(int profileId)
        {
            return new PhofBody(ContentKeys.ContactUs, profileId);
        }

    }
}