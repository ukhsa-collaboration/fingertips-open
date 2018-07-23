using System.Linq;
using System.Web.Http;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ServicesWeb.DependencyResolver;
using PholioVisualisation.ServicesWeb.Helpers;
using Unity;
using Unity.Injection;

namespace PholioVisualisation.ServicesWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.DependencyResolver = new UnityResolver(UnityContainerBuilder.GetUnityContainer());

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "",
               defaults: new { id = RouteParameter.Optional }
            );

            // Remove XML as default response type
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }
}
