using System;
using System.Linq;
using DIContainer.BaseScan;
using DIContainer.Example.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DIContainer.Example.Tests
{
    [TestClass]
    public class WhenUsingProgram
    {
        private IProgram _program;

        [TestInitialize]
        public void Init()
        {
            IoC.Register();
            _program = IoC.Container.GetInstance<IProgram>();
        }

        [TestMethod]
        public void Start_Returns_Events()
        {
            // This ensure that it runs TestsRegistry and override any default initialisations

            var events = _program.Start();

            events.ToList().ForEach(Console.WriteLine);

            Assert.IsTrue(events != null && 
                events.Count() == 3 && 
                events.ToList()[2].Contains("briskly") == false &&
                events.ToList()[2].Contains("Test") 
                );
        }
    }
}
