using NHibernate;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.ServicesWeb.Helpers;
using Unity;
using Unity.Injection;

namespace PholioVisualisation.ServicesWeb.DependencyResolver
{
    public static class UnityContainerBuilder
    {
        public static UnityContainer GetUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<ISessionFactory>(new InjectionFactory(x => ReaderFactory.GetSessionFactory()));
            container.RegisterType<IProfileReader>(new InjectionFactory(x => ReaderFactory.GetProfileReader()));
            container.RegisterType<IGroupDataReader>(new InjectionFactory(x => ReaderFactory.GetGroupDataReader()));
            container.RegisterType<IAreasReader>(new InjectionFactory(x => ReaderFactory.GetAreasReader()));
            container.RegisterType<IPholioReader>(new InjectionFactory(x => ReaderFactory.GetPholioReader()));

            container.RegisterType<IIndicatorMetadataRepository>(new InjectionFactory(x => new IndicatorMetadataRepository()));
            container.RegisterType<IContentItemRepository>(new InjectionFactory(x => new ContentItemRepository()));

            container.RegisterType<IRequestContentParserHelper, RequestContentParserHelper>();
            container.RegisterType<ICoreDataSetValidator, CoreDataSetValidator>();

            return container;
        }
    }
}