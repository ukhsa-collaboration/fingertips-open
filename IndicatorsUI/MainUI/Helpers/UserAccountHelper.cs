using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using IndicatorsUI.DataAccess;
using IndicatorsUI.UserAccess;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace IndicatorsUI.MainUI.Helpers
{
    public interface IUserAccountHelper
    {
        string GetUserName(IPrincipal user, string userName);

        Task<IdentityResult> CreateUser(HttpRequestBase request, ApplicationUser applicationUser,
            string password);

        void SignOut(HttpRequestBase request);
    }

    /// <summary>
    /// Wrapper for actions that require a web context to proceed to enable unit testing
    /// of a controller.
    /// </summary>
    public class UserAccountHelper : IUserAccountHelper
    {
        /// <summary>
        /// In the test environment basic authentication override cookie authentication
        /// and the user identity will be the PHE Windows identity of the user. To 
        /// get around this we use the PHE identity as the user name instead of the
        /// email address that the user has entered.
        /// </summary>
        public string GetUserName(IPrincipal user,string userName)
        {
            var identity = user.Identity;

            if (AppConfig.Instance.IsEnvironmentTest &&
                identity != null &&
                string.IsNullOrEmpty(identity.Name) == false)
            {
                // Use PHE user name from basic authentication
                return identity.Name;
            }

            return userName;
        }

        public async Task<IdentityResult> CreateUser(HttpRequestBase request, ApplicationUser applicationUser,
            string password)
        {
            var appUserManager = request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return await appUserManager.CreateAsync(applicationUser, password);
        }

        public void SignOut(HttpRequestBase request)
        {
            var authenticationManager = request.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}