using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndicatorsUI.UserAccess
{
    public interface IUserAccessDbContext
    {
        bool HasEmailAddressAlreadyBeenRegistered(string emailAddress);
        ApplicationUser GetUser(string emailAddress);
    }

    public class UserAccessDbContext : IdentityDbContext<ApplicationUser>, IUserAccessDbContext
    {
        public UserAccessDbContext()
           :base(System.Configuration.ConfigurationManager.ConnectionStrings["FingertipsUsersConnectionString"].ConnectionString, throwIfV1Schema:false)
        {
        }

        public static UserAccessDbContext Create()
        {
            return new UserAccessDbContext();
        }

        public bool HasEmailAddressAlreadyBeenRegistered(string emailAddress)
        {
            return Users.Any(x => x.Email.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase));
        }

        public ApplicationUser GetUser(string emailAddress)
        {
            return Users.FirstOrDefault(x => x.Email.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
