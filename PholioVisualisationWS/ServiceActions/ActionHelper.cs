using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class ActionHelper
    {
        public static int GetNonSearchProfileId(int profileId, int templateProfileId)
        {
            return profileId == ProfileIds.Search ?
                templateProfileId :
                profileId;
        } 
    }
}