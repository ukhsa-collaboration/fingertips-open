using DIContainer.Example.Interfaces;
using StructureMap.Configuration.DSL;

namespace DIContainer.Example
{
    public class ExampleRegistry: Registry
    {
        public ExampleRegistry()
        {
            // Override injections if required

            For<IPerson>().Use<SpecialPerson>().Ctor<string>("name").Is("Mike");
        }
    }
}
