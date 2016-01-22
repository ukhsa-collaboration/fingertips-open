using StructureMap;
using StructureMap.Graph;

namespace DIResolver
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
            if (_container == null)
            {
                _container = new Container();

                _container.Configure(registry =>
                    registry.Scan(_ =>
                    {
                        _.TheCallingAssembly();
                        _.LookForRegistries();
                        _.WithDefaultConventions();
                        _.AssembliesFromApplicationBaseDirectory();
                    }));
            }
        }
    }
}
