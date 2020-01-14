using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.ServicesWeb.Validations;
using System;
using System.Net.Http;

namespace PholioVisualisation.ServicesWebTest.Validations
{
    public class AbstractValidatorClassTest : AbstractValidator
    {
        protected override void Validate()
        {
        }
    }

    [TestClass]
    public class AbstractValidatorTest
    {
        [TestMethod]
        public void ShouldBeValid()
        {
            var validator = new AbstractValidatorClassTest();

            Assert.IsTrue(validator.IsValid());
        }

        [TestMethod]
        public void ShouldBeNotValid()
        {
            var validator = new AbstractValidatorClassTest();

            validator.ValidationsExceptionsFound.Add(new Exception("Test exception"));

            Assert.IsFalse(validator.IsValid());
        }

        [TestMethod]
        public void ShouldClearExceptions()
        {
            var validator = new AbstractValidatorClassTest();

            validator.ValidationsExceptionsFound.Add(new Exception("Test exception"));

            validator.ClearExceptions();

            Assert.IsTrue(validator.IsValid());
        }

        [TestMethod]
        public void GetExceptionStringContentMessages()
        {
            var validator = new AbstractValidatorClassTest();

            validator.ValidationsExceptionsFound.Add(new Exception("Test exception"));

            var result = validator.GetExceptionStringContentMessages();
            Assert.IsTrue(result is StringContent);
        }
    }
}
