using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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
        public ApplicationUser GetApplicationUser(IPrincipal user)
        {
            var identity = user.Identity;
            if (identity.IsAuthenticated)
            {
                using (var db = UserAccessDbContext.Create())
                {
                    var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                    return userManager.FindByName(identity.Name);
                }
            }

            return null;
        }

        public bool IsUserSignedIn(IPrincipal user)
        {
            return user.Identity.IsAuthenticated;
        }
    }
}