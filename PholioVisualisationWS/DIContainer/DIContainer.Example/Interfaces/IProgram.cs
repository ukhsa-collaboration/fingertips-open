using System.Collections.Generic;

namespace DIContainer.Example.Interfaces
{
    public interface IProgram
    {
        IEnumerable<string> Start();
        string Stop();
    }
}