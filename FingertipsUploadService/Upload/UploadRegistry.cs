using StructureMap;
namespace Fpm.Upload
{
    public class UploadRegistry : Registry
    {
        public UploadRegistry()
        {
            // Override injections and any custom parameters constructors if required
            //For<IContentReader>()
            //    .Use<IContentReader>(x => ReaderFactory.GetContentReader());
        }
    }
}