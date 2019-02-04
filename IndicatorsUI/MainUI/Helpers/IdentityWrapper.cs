using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using IndicatorsUI.UserAccess;

namespace IndicatorsUI.MainUI.Helpers
{
    public interface IIdentityWrapper
    {
        ApplicationUser GetApplicationUser(IPrincipal user);
        bool IsUserSignedIn(IPrincipal user);
    }

    public class IdentityWrapper : IIdentityWrapper
    {
        private bool isStaging = AppConfig.Instance.IsEnvironmentTest;

        public ApplicationUser GetApplicationUser(IPrincipal user)
        {
            var identity = user.Identity;
            if (identity.IsAuthenticated)
            {
                return FindUserByName(identity);
            }

            return null;
        }

        public bool IsUserSignedIn(IPrincipal user)
        {
            var identity = user.Identity;
             
            // On test server user will be authenticated due to basic authentication but
            // the application user will be null if they have not registered for an account
            // NOTE: this might make identity work on staging and confusing on dev
            if (isStaging)
            {
                var appUser = FindUserByName(identity);
                return appUser != null;
            }

            return identity.IsAuthenticated;
        }

        private static ApplicationUser FindUserByName(IIdentity identity)
        {
            using (var db = UserAccessDbContext.Create())
            {
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                return userManager.FindByName(identity.Name);
            }
        }
    }
}