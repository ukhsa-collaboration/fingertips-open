using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Helpers
{
    public static class ProfileDetailsCleaner
    {
        public static void CleanUserInput(ProfileDetails profileDetails)
        {
            profileDetails.Name = Clean(profileDetails.Name);

            profileDetails.AreasIgnoredForSpineChart = 
                CleanAreasString(profileDetails.AreasIgnoredForSpineChart);

            profileDetails.AreasIgnoredEverywhere =
                CleanAreasString(profileDetails.AreasIgnoredEverywhere);

            profileDetails.ExtraCssFiles =
                CleanAreasString(profileDetails.ExtraCssFiles);
        }

        private static string Clean(string s)
        {
            return s != null 
                ? s.Trim()
                : s;
        }

        private static string CleanAreasString(string s)
        {
            if (string.IsNullOrWhiteSpace(s) == false)
            {
                return s.Trim().Replace(" ", "");
            }
            return null;
        }
    }
}