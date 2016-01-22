using StructureMap.Configuration.DSL;

namespace Fpm.MainUI
{
    public class MainUIRegistry : Registry
    {
        public MainUIRegistry()
        {
            // Override injections and any custom parameters constructors if required
            //For<IContentReader>()
            //    .Use<IContentReader>(x => ReaderFactory.GetContentReader());
        }
    }
}