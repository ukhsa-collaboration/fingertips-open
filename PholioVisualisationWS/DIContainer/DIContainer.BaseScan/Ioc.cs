using StructureMap;
using StructureMap.Graph;

namespace DIContainer.BaseScan
{
    public static class IoC
    {
        private static Container _container;

        public static Container Container
        {
            get { return _container; }
        }
        
        public static void Register()
        {
            _container = new Container();
            
            _container.Configure(registry => registry.Scan(assembly =>
            {
                assembly.AssembliesFromApplicationBaseDirectory();
                assembly.TheCallingAssembly();
                assembly.WithDefaultConventions();
                assembly.LookForRegistries();
            }));
        }
    }
}
