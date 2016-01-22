using DIContainer.Example.Interfaces;

namespace DIContainer.Example
{
    public class Foo : IFoo
    {
        public string Start()
        {
            return ("starting");
        }

        public string Stop()
        {
            return ("stopping");
        }
    }
}