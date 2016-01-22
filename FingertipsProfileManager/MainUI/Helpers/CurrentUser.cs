using System.Web;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.User;

namespace Fpm.MainUI.Helpers
{
    public class CurrentUser
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private readonly string _username;

        public CurrentUser()
        {
            _username = AppConfig.CurrentUserName;
        }

        public string Name
        {
            get
            {
                return string.IsNullOrEmpty(_username)
                    ? HttpContext.Current.User.Identity.Name
                    : _username;
            }
        }

        public FpmUser User
        {
            get { return _reader.GetUserByUserName(Name); }
        }

        public bool IsAdministrator
        {
            get
            {
                var fpmUser = User;
                CheckUserExists(fpmUser);
                return fpmUser.IsAdministrator;
            }
        }

        private void CheckUserExists(FpmUser user)
        {
            if (user == null)
            {
                throw new FpmException("FPM user does not exist for " + Name);
            }
        }

        public int Id {
            get
            {
                FpmUser fpmUser = User;
                return fpmUser.Id;
            }
        }
    }
}