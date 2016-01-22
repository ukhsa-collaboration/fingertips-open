using System;
using System.Collections.Generic;
using System.Linq;
using DIContainer.BaseScan;
using DIContainer.Example.Interfaces;

namespace DIContainer.Example
{
    public class Program : IProgram
    {
        private readonly IPerson _person;
        private readonly IFoo _foo;

        public Program(IPerson person, IFoo foo)
        {
            _person = person;
            _foo = foo;
        }

        public IEnumerable<string> Start()
        {
            yield return _foo.Start();
            yield return _person.Speak();
            yield return _person.Walk();
        }

        public string Stop()
        {
            return _foo.Stop();
        }


        static void Main(string[] args)
        {
            Console.WriteLine("activating person");

            IoC.Register();

            var program = IoC.Container.GetInstance<IProgram>();

            program.Start().ToList().ForEach(Console.WriteLine);

            Console.WriteLine("Press any key to Stop");
            Console.ReadLine();

            Console.WriteLine(program.Stop());

            Console.WriteLine("Press any key to Exit");
            Console.ReadLine();

        }
    }
}
