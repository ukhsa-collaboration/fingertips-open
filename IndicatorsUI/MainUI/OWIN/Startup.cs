using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using IndicatorsUI.UserAccess;
using Owin;

[assembly: OwinStartup(typeof(IndicatorsUI.MainUI.OWIN.Startup))]
namespace IndicatorsUI.MainUI.OWIN
{
    public class Startup
    {
        // Keep user logged in for the day
        public static TimeSpan TimeToKeepUserLoggedIn = TimeSpan.FromMinutes(7 * 24 * 60);

        public void Configuration(IAppBuilder app)
        {
            ConfigureOwinDependencies(app);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                SlidingExpiration = true,
                ExpireTimeSpan = TimeToKeepUserLoggedIn
            });
        }

        private void ConfigureOwinDependencies(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(UserAccessDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
        }
    }
}