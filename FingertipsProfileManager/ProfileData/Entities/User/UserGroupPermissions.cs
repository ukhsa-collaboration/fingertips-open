namespace Fpm.ProfileData.Entities.User
{
    public class UserGroupPermissions
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProfileId { get; set; }
        public FpmUser FpmUser { get; set; }
        public bool Assigned { get; set; }
    }
}