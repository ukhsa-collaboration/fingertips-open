using System;
using System.Collections.Generic;
using Fpm.MainUI.Helpers;
using Unity;
using Unity.Injection;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using NHibernate;

namespace Fpm.MainUI
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
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

            // Register type mappings
            container.RegisterType<IIndicatorMetadataTextChanger, IndicatorMetadataTextChanger>();
            container.RegisterType<IIndicatorMetadataTextCreator, IndicatorMetadataTextCreator>();
            container.RegisterType<IIndicatorMetadataTextParser, IndicatorMetadataTextParser>();
            container.RegisterType<IIndicatorOwnerChanger, IndicatorOwnerChanger>();
            container.RegisterType<INewDataDeploymentCount, NewDataDeploymentCount>();
            container.RegisterType<IProfileDetailsCopier, ProfileDetailsCopier>();
            container.RegisterType<IProfileMenuHelper, ProfileMenuHelper>();
            container.RegisterType<IProfileDetailsCopier, ProfileDetailsCopier>();
            container.RegisterType<IProfileMenuHelper, ProfileMenuHelper>();
            container.RegisterType<IRemoveIndicatorChecker, RemoveIndicatorChecker>();
            container.RegisterType<ITimePeriodHelper, TimePeriodHelper>();
            container.RegisterType<ITimePeriodReader, TimePeriodReader>();
            container.RegisterType<IUserDetails, UserDetails>();

            container.RegisterType<ISessionFactory>(new InjectionFactory(x => NHibernateSessionFactory.GetSession()));

            container.RegisterType<IProfilesReader>(new InjectionFactory(x => ReaderFactory.GetProfilesReader()));
            container.RegisterType<IProfilesWriter>(new InjectionFactory(x => ReaderFactory.GetProfilesWriter()));

            container.RegisterType<IProfileRepository>(new InjectionFactory(x =>
                new ProfileRepository(NHibernateSessionFactory.GetSession())));

            container.RegisterType<ILookUpsRepository>(new InjectionFactory(x =>
                new LookUpsRepository(NHibernateSessionFactory.GetSession())));

            container.RegisterType<ICoreDataRepository>(new InjectionFactory(x =>
                new CoreDataRepository(NHibernateSessionFactory.GetSession())));

            container.RegisterType<IEmailRepository>(new InjectionFactory(x =>
                new EmailRepository(NHibernateSessionFactory.GetSession())));

            container.RegisterType<IUserRepository>(new InjectionFactory(x =>
                new UserRepository(NHibernateSessionFactory.GetSession())));

            container.RegisterType<IDocumentsRepository>(new InjectionFactory(x =>
                new DocumentsRepository(NHibernateSessionFactory.GetSession())));

            container.RegisterType<IExceptionsRepository>(new InjectionFactory(x =>
                new ExceptionsRepository(NHibernateSessionFactory.GetSession())));

            container.RegisterType<ILoggingRepository>(new InjectionFactory(x =>
                new LoggingRepository(NHibernateSessionFactory.GetSession())));

            container.RegisterType<IReportRepository>(new InjectionFactory(x =>
                new ReportRepository(NHibernateSessionFactory.GetSession())));

            container.RegisterType<IUploadJobRepository>(new InjectionFactory(x =>
                new UploadJobRepository(NHibernateSessionFactory.GetSession())));

            container.RegisterType<IUserFeedbackRepository>(new InjectionFactory(x =>
                new UserFeedbackRepository(NHibernateSessionFactory.GetSession())));

            container.RegisterType<IAreaTypeRepository>(new InjectionFactory(x =>
                new AreaTypeRepository(NHibernateSessionFactory.GetSession())));
        }
    }
}