using StructureMap;
using StructureMap.Configuration.DSL;

namespace FingertipsUploadService.ProfileData
{
    class ProfileDataRegistry : Registry
    {
        public ProfileDataRegistry()
        {
            // Override injections and any custom parameters constructors if required
            //For<IContentReader>()
            //    .Use<IContentReader>(x => ReaderFactory.GetContentReader());
        }
    }
}
