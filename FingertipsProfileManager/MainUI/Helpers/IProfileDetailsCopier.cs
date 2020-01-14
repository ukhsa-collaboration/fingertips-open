using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Helpers
{
    public interface IProfileDetailsCopier
    {
        int CreateCopy(ProfileDetails sourceProfile);
        void CopyContentItems(int sourceProfileId, int newProfileId);
    }
}