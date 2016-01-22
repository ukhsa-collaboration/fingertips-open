using StructureMap.Configuration.DSL;

namespace PholioVisualisation.DataConstruction
{
    public class DataConstructionDIRegistry: Registry
    {
        public DataConstructionDIRegistry()
        {
            For<IGroupIdProvider>()
                .Use<GroupIdProvider>();

            For<IAreaTypeListProvider>()
                .Use<AreaTypeListProvider>();
        }
    }
}