using IndicatorsUI.MainUI.Models.UserAccess;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.ActionFilters;
using IndicatorsUI.MainUI.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using IndicatorsUI.UserAccess;
using Microsoft.AspNet.Identity.Owin;

namespace IndicatorsUI.MainUI.Controllers
{
    [RoutePrefix("user-account")]
    public class UserAccountController : BaseController
    {
        public const string ErrorEmailAddressNotRecognised = 
            "This email address is not recognised, it may be because you have not signed up yet";

        private readonly IIdentityWrapper _identity;
        private readonly IUserAccessDbContext _userAccessDbContext;
        private readonly IEmailSender _emailSender;
        private readonly IExceptionLoggerWrapper _exceptionLogger;
        private readonly IUserAccountHelper _userAccountHelper;

        public UserAccountController(IIdentityWrapper identity, 
            IUserAccessDbContext userAccessDbContext, IEmailSender emailSender, 
            IExceptionLoggerWrapper exceptionLogger, IUserAccountHelper userAccountHelper,
            IAppConfig appConfig) : base (appConfig)
        {
            _identity = identity;
            _userAccessDbContext = userAccessDbContext;
            _emailSender = emailSender;
            _exceptionLogger = exceptionLogger;
            _userAccountHelper = userAccountHelper;
        }

        /// <summary>
        /// Page that allows the user to either login or register.
        /// </summary>
        [Route("login")]
        public ActionResult Login()
        {
            LoginViewModel loginModel = new LoginViewModel();
            return View(loginModel);
        }

        /// <summary>
        /// Allows user to request another verification email. Only accessible after the user has correctly
        /// entered their email and password.
        /// </summary>
        [Route("validate-login")]
        [MultipleButton(Name = "action", Argument = "ResendEmail")]
        public ActionResult RequestAnotherEmail(LoginViewModel loginViewModel, string returnUrl = null)
        {
            var result = SendVerificationEmail(loginViewModel.UserName);
            if (result != null)
            {
                return result;
            }

            return View("Thankyou");
        }

        /// <summary>
        /// Action when user has submitted the credentials to login.
        /// </summary>
        [Route("validate-login")]
        [MultipleButton(Name = "action", Argument = "SignIn")]
        public async Task<ActionResult> ValidateLogin(LoginViewModel loginViewModel, string returnUrl)
        {
            if (_userAccessDbContext.HasEmailAddressAlreadyBeenRegistered(loginViewModel.UserName) == false)
            {
                // Email address is not recognised
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, ErrorEmailAddressNotRecognised);
                return View("Login", loginViewModel);
            }

            if (!ModelState.IsValid)
            {
                return View("Login", loginViewModel);
            }

            var appUserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var userName = _userAccountHelper.GetUserName(User, loginViewModel.UserName);
            var appUser = await appUserManager.FindAsync(userName, loginViewModel.Password);

            if (appUser != null)
            {
                if (_appConfig.IsFeatureActive(FeatureFlags.EmailVerification) && 
                    appUser.HasUserValidatedEmailAccount == false)
                {
                    // User has not verified there email account yet
                    ModelState.Clear();
                    ViewBag.DisplayResendEmail = true;
                    return View("Login", loginViewModel);
                }

                // User can be signed in
                await SignInAsync(appUser, loginViewModel.KeepUserLoggedIn);
                if (returnUrl == null)
                {
                    return RedirectToAction("Index", "IndicatorList");
                }
                return Redirect(returnUrl);
            }

            return InvalidSignIn(loginViewModel);
        }

        [Route("registration")]
        public ActionResult Registration()
        {
            RegisterViewModel vm = new RegisterViewModel();
            return View(vm);
        }

        [Route("submit-registration")]
        public async Task<ActionResult> SubmitRegistration(RegisterViewModel registrationModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Registration", registrationModel);
            }

            var applicationUser = GetNewApplicationUser(registrationModel);
            var result = await _userAccountHelper.CreateUser(Request, applicationUser, registrationModel.Password);
            if (result.Succeeded)
            {
                if (_appConfig.IsFeatureActive(FeatureFlags.EmailVerification))
                {
                    var emailSendResult = SendVerificationEmail(registrationModel.UserName);
                    if (emailSendResult != null)
                    {
                        return emailSendResult;
                    }
                }

                return View("Thankyou");
            }

            // Add errors that prevented user being created to be displayed in ViewSummary
            foreach (var error in result.Errors)
            {
                // Ignore duplicate of email address
                if (error.StartsWith("Name")) continue;

                if (error.StartsWith("Email") && error.Contains("is already taken"))
                {
                    ModelState.AddModelError(string.Empty,
                        "This email address has already been registered. Try signing in instead.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            return View("Registration", registrationModel);
        }

        [Route("verify-email-address/{id}")]
        public async Task<ActionResult> VerifyEmailAddress(string id)
        {
            PageMessageViewModel message = new PageMessageViewModel();

            /*if id is not passed in url*/
            if (id == null)
            {
                message.Header = "Invalid URL. Contact support.";
                message.Message = "";
                return View("Message", message);
            }

            Guid tempGuid = Guid.Parse(id);
            var appUserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var context = Request.GetOwinContext().Get<UserAccessDbContext>();

            var usr = context.Users.FirstOrDefault(x => x.TempGuid == tempGuid);

            if (usr == null) /*if userid is manipulated*/
            {
                message.Header = "Invalid User";
                message.Message = "";
                return View("Message", message);
            }

            usr.HasUserValidatedEmailAccount = true;
            usr.TempGuid = null;
            var result = await appUserManager.UpdateAsync(usr);
            if (result.Succeeded) /*Activation flag is updated*/
            {
                message.Header =
                    "Your account has been successfully verified. You can now sign in.";
                message.LinkText = "Sign in";
                message.LinkUrl = "/user-account/login";
                return View("Message", message);
            }
            else
            {
                message.Header = "Something went wrong. Please try again.";
                return View("Message", message);
            }

        }

        [Route("reset-password/{id}")]
        public async Task<ActionResult> ResetPassword(Guid id)
        {
            PageMessageViewModel message = new PageMessageViewModel();
            var appUserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var context = Request.GetOwinContext().Get<UserAccessDbContext>();
            var usr = context.Users.FirstOrDefault(x => x.TempGuid == id);
            if (usr != null)
            {

                var result = await appUserManager.VerifyUserTokenAsync(usr.Id, "ResetPassword", usr.AccessTokenResetPassword);

                if (result)
                {
                    ResetPasswordViewModel resetPasswordModel = new ResetPasswordViewModel();
                    resetPasswordModel.UserId = usr.Id;
                    resetPasswordModel.Token = usr.AccessTokenResetPassword;
                    return View(resetPasswordModel);
                }
                else
                {
                    message.Header = "I'm sorry, the link you clicked was invalid or has expired. Please try again.";
                    message.LinkText = "Forgot Password?";
                    message.LinkUrl = "/user-account/forgot-password";
                    return View("Message", message);
                }
            }
            message.Header = "Something went wrong. Please try again.";
            message.LinkText = "Forgot Password?";
            message.LinkUrl = "/user-account/forgot-password";
            return View("Message", message);
        }

        [Route("update-password")]
        public async Task<ActionResult> UpdatePassword(ResetPasswordViewModel resetPasswordVM)
        {
            PageMessageViewModel message = new PageMessageViewModel();
            var appUserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var usr = await appUserManager.FindByIdAsync(resetPasswordVM.UserId);
            if (usr != null)
            {
                var result = await appUserManager.ResetPasswordAsync(resetPasswordVM.UserId, resetPasswordVM.Token,
                    resetPasswordVM.NewPassword);
                if (result.Succeeded)
                {

                    usr.AccessTokenResetPassword = null;
                    usr.TempGuid = null;
                    result = await appUserManager.UpdateAsync(usr);
                    if (result.Succeeded)
                    {
                        message.Header = "Thank you. Your password has updated. Please sign in again.";
                        message.LinkText = "Sign in";
                        message.LinkUrl = "/user-account/login";
                    }
                    else
                    {
                        message.Header = "Something went wrong. Try again";
                    }
                    return View("Message", message);
                }
            }
            message.Header = "Something went wrong. Try again";
            return View("Message", message);
        }

        [Route("forgot-password")]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [Route("email-error")]
        public ActionResult EmailError()
        {
            return View();
        }

        [Route("validate-forgot-password")]
        public async Task<ActionResult> ValidateForgotLogin(ForgotPasswordViewModel forgotPassword)
        {
            if (!ModelState.IsValid)
            {
                return View("ForgotPassword", forgotPassword);
            }

            PageMessageViewModel message = new PageMessageViewModel();
            var appUserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var usr = await appUserManager.FindByNameAsync(forgotPassword.EmailAddress);

            if (usr != null)
            {
                var token = appUserManager.GeneratePasswordResetTokenAsync(usr.Id).Result;

                usr.AccessTokenResetPassword = token;
                usr.TempGuid = Guid.NewGuid();
                var result = await appUserManager.UpdateAsync(usr);
                if (result.Succeeded)
                {
                    _emailSender.SendResetPasswordEmail(usr, HttpUtility.UrlEncode(token));
                    message.Header = "Please check your email for reset password link";
                    return View("Message", message);
                }
            }

            ModelState.AddModelError(string.Empty, "This email address is not recognised. You probably need to create an account.");
            return View("ForgotPassword", forgotPassword);
        }

        [Route("logout")]
        public ActionResult Logout()
        {
            _userAccountHelper.SignOut(Request);

            /* User still authenticated on this request, need to redirect to
             * get new request where user is not authenticated */
            return RedirectToAction("SignedOut");
        }

        [Route("signed-out")]
        public ActionResult SignedOut()
        {
            PageMessageViewModel message = new PageMessageViewModel();

            message.Header = "You have successfully signed out";
            message.LinkText = "Sign in";
            message.LinkUrl = "/user-account/login";

            return View("Message", message);
        }

        [Route("account")]
        public ActionResult EditMyAccount()
        {
            // Check user is logged in
            if (_identity.IsUserSignedIn(User) == false)
            {
                return RedirectToAction("Login");
            }

            var user = _identity.GetApplicationUser(User);
            UserViewModel userVM = new UserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JobTitle = user.JobTitle,
                OrganisationId = user.OrganisationId,
                UserName = user.UserName
            };
            return View("EditMyAccount", userVM);
        }

        private async Task SignInAsync(ApplicationUser user, bool keepUserLoggedIn)
        {
            var authManager = Request.GetOwinContext().Authentication;
            authManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            var appUserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var identity = await appUserManager.CreateIdentityAsync(
                user, DefaultAuthenticationTypes.ApplicationCookie);

            authManager.SignIn(
                new AuthenticationProperties
                {
                    IsPersistent = keepUserLoggedIn
                }, identity);
        }

        private ActionResult InvalidSignIn(LoginViewModel loginViewModel)
        {
            ModelState.Clear();
            ModelState.AddModelError(string.Empty, "That password is not correct");
            return View("Login", loginViewModel);
        }

        private ApplicationUser GetNewApplicationUser(RegisterViewModel registrationModel)
        {
            var userName = _userAccountHelper.GetUserName(User, registrationModel.UserName);

            var appUser = new ApplicationUser
            {
                UserName = userName,
                FirstName = registrationModel.FirstName,
                LastName = registrationModel.LastName,
                JobTitle = registrationModel.JobTitle,
                HasUserValidatedEmailAccount = false,
                Email = registrationModel.UserName,
                OrganisationId = registrationModel.OrganisationId,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                TempGuid = Guid.NewGuid()
            };
            return appUser;
        }

        private ActionResult SendVerificationEmail(string emailAddress)
        {
            try
            {
                var user = _userAccessDbContext.GetUser(emailAddress);
                _emailSender.SendVerificationEmail(emailAddress, user.FirstName, user.TempGuid.Value);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex, "Verification email could not be sent");
                return RedirectToAction("EmailError");
            }

            return null;
        }
    }
}