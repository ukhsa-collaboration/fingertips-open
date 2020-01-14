using IndicatorsUI.UserAccess.UserList.IRepository;
using IndicatorsUI.UserAccess.UserList.Repository;
using System;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DataAccess.Repository;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.UserAccess;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace IndicatorsUI.MainUI
{
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
            new Lazy<IUnityContainer>(() =>
            {
                var container = new UnityContainer();
                RegisterTypes(container);
                return container;
            });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container
        {
            get { return container.Value; }
        }

        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // Registered type mappings
            container.RegisterType<IIndicatorListRepository, IndicatorListRepo>();
            container.RegisterType<IIdentityWrapper, IdentityWrapper>();
            container.RegisterType<IUserAccessDbContext, UserAccessDbContext>();
            container.RegisterType<IExceptionLoggerWrapper, ExceptionLoggerWrapper>();
            container.RegisterType<IUserAccountHelper, UserAccountHelper>();
            container.RegisterType<IPublicIdGenerator, PublicIdGenerator>();
            container.RegisterType<IEmailRepository, EmailRepository>();

            // Register reader created by factory method
            container.RegisterType<ProfileReader>(new InjectionFactory(x => ReaderFactory.GetProfileReader()));

            // Register app config singleton
            container.RegisterType<IAppConfig>(new ContainerControlledLifetimeManager(), 
                new InjectionFactory(c => AppConfig.Instance));

            // Register GoogleAnalyticsEventLogger singleton
            GoogleAnalyticsEventLogger.Instance = new GoogleAnalyticsEventLogger(new MeasurementProtocolDownloadFiles());
            container.RegisterType<IGoogleAnalyticsEventLogger>(new ContainerControlledLifetimeManager(),
                new InjectionFactory(c => GoogleAnalyticsEventLogger.Instance));
        }
    }
}