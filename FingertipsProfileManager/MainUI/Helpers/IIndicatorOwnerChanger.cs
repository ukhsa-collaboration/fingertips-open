namespace Fpm.MainUI.Helpers
{
    public interface IIndicatorOwnerChanger
    {
        void AssignIndicatorToProfile(int indicatorId, int newOwnerProfileId);
    }
}