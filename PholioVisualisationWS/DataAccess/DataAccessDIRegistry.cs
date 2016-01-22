using StructureMap.Configuration.DSL;

namespace PholioVisualisation.DataAccess
{
    public class DataAccessDiRegistry : Registry
    {
        public DataAccessDiRegistry()
        {
            // Override injections and any custom parameters constructors if required
            For<IContentReader>()
                .Use<IContentReader>(x => ReaderFactory.GetContentReader());

            For<IProfileReader>()
                .Use<IProfileReader>(x => ReaderFactory.GetProfileReader());

            For<IGroupDataReader>()
                .Use<IGroupDataReader>(x => ReaderFactory.GetGroupDataReader());

            For<IAreasReader>()
                .Use<IAreasReader>(x => ReaderFactory.GetAreasReader());
        }
    }
}