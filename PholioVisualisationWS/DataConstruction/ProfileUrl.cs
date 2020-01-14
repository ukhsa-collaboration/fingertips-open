using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class ProfileUrl
    {
        private ProfilePerIndicator _profile;
        private bool _isEnvironmentLive;

        public ProfileUrl(ProfilePerIndicator profile, bool isEnvironmentLive)
        {
            _profile = profile;
            _isEnvironmentLive = isEnvironmentLive;
        }

        /// <summary>
        /// Get the URL of the web page that displayed the profile data.
        /// </summary>
        public string DataUrl
        {
            get
            {
                if (_profile.ProfileId == ProfileIds.PracticeProfiles)
                {
                    return GetPracticeProfilesUrl();
                }

                if (_profile.ProfileId == ProfileIds.PublicHealthDashboardLongerLives)
                {
                    return GetPublicHealthDashboardLongerLivesUrl();
                }

                var sb = new StringBuilder();
                sb.Append(Protocol)
                    .Append(Host)
                    .Append("/")
                    .Append(_profile.ProfileUrl)
                    .Append("#gid/")
                    .Append(_profile.GroupId)
                    .Append("/ati/")
                    .Append(_profile.AreaTypeId);
                return sb.ToString();
            }
        }

        private string GetPracticeProfilesUrl()
        {
            // e.g. https://testprofiles.phe.org.uk/profile/general-practice/data

            var sb = new StringBuilder();
            sb.Append(Protocol)
                .Append(Host)
                .Append("/profile/general-practice/data");
            return sb.ToString();
        }

        private string GetPublicHealthDashboardLongerLivesUrl()
        {
            // e.g. https://testhealthierlives.phe.org.uk/topic/public-health-dashboard/map-with-data#/gid/1938133145

            var sb = new StringBuilder();
            sb.Append(Protocol)
                .Append(Host)
                .Append("/topic/public-health-dashboard/map-with-data#/gid/")
                .Append(_profile.GroupId);
            return sb.ToString();
        }

        private string Host
        {
            get
            {
                var baseUrl = _isEnvironmentLive 
                    ? _profile.LiveHostUrl 
                    : _profile.TestHostUrl;
                return baseUrl;
            }
        }

        private string Protocol
        {
            get
            {
                var protocol = _isEnvironmentLive 
                    ? "http://" 
                    : "https://";
                return protocol;
            }
        }
    }
}