using Fpm.MainUI.Helpers;
using OpenQA.Selenium;

namespace Fpm.MainUISeleniumTest
{
    public class NavigateTo
    {
        public static string BaseUrl = ApplicationConfiguration.SiteUrlForTesting;

        private IWebDriver driver;

        public NavigateTo(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void ProfilesAndIndicatorsPage()
        {
            GoToUrl(string.Empty);
        }

        public void ProfilesPage()
        {
            GoToUrl("profilemanager");
        }

        public void ProfilesForNonAdminPage()
        {
            GoToUrl("profile/EditProfileNonAdmin");
        }

        public void ContentIndexPage()
        {
            GoToUrl("content/content-index");
        }

        public void ReportsIndexPage()
        {
            GoToUrl("reports");
        }

        public void UserIndexPage()
        {
            GoToUrl("user/user-index");
        }

        public void LookupTablesPage()
        {
            GoToUrl("lookup-tables");
        }

        public void CategoriesPage()
        {
            GoToUrl("lookup-tables/categories");
        }

        public void GoToUrl(string relativeUrl)
        {
            driver.Navigate().GoToUrl(BaseUrl + relativeUrl);
        }
    }
}
