using DIContainer.Example.Interfaces;
using StructureMap.Configuration.DSL;

namespace DIContainer.Example.Tests
{
    public class TestsRegistry: Registry
    {
        public TestsRegistry()
        {
            // Override injections if required

            For<IPerson>().Use<Person>().Ctor<string>("name").Is("Test");
        }
    }
}
