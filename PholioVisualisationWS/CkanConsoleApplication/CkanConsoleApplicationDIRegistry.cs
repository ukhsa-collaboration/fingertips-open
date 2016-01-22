using Ckan.Client;
using StructureMap.Configuration.DSL;

namespace CkanConsoleApplication
{
    public class CkanConsoleApplicationDIRegistry : Registry
    {
        public CkanConsoleApplicationDIRegistry()
        {
            // Override injections and any custom parameters constructors if required
          
            For<ICkanHttpClient>()
                .Use<CkanHttpClient>()
                .Ctor<string>("repository").Is(CkanApplicationConfiguration.CkanRepositoryHostname)
                .Ctor<string>("apiKey").Is(CkanApplicationConfiguration.CkanRepositoryApiKey);
        }   
    }
}
