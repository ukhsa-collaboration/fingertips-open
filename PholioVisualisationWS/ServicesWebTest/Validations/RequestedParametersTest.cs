using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.ServicesWeb.Validations;

namespace PholioVisualisation.ServicesWebTest.Validations
{
    [TestClass]
    public class RequestedParametersTest
    {
        private RequestedParameters _nullRequestedParameters;
        private RequestedParameters _requestedParameters;

        [TestInitialize]
        public void StartUp()
        {
            var id = 1;
            var stringId = "id";
            _requestedParameters = new RequestedParameters(id, id, stringId, stringId, stringId, stringId, stringId, stringId, id, id, stringId);

            _nullRequestedParameters = new RequestedParameters(id, id, null, stringId, stringId);
        }

        [TestMethod]
        public void ShouldReturnNullForValidateValidateChildAreaTypeId()
        {
            var exception = _requestedParameters.ValidateChildAreaTypeId();

            Assert.IsNull(exception);
        }

        [TestMethod]
        public void ShouldReturnNullForValidateParentAreaTypeId()
        {
            var exception = _requestedParameters.ValidateParentAreaTypeId();

            Assert.IsNull(exception);
        }

        [TestMethod]
        public void ShouldReturnNullForValidateIndicatorIds()
        {
            var exception = _requestedParameters.ValidateIndicatorIds();

            Assert.IsNull(exception);
        }

        [TestMethod]
        public void ShouldReturnExceptionForValidateIndicatorIds()
        {
            var expectedMessage = "IndicatorIds cannot be null or empty";
            var exception = _nullRequestedParameters.ValidateIndicatorIds();

            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [TestMethod]
        public void ShouldReturnNullForValidateAreaCode()
        {
            var exception = _requestedParameters.ValidateAreaCode();

            Assert.IsNull(exception);            
        }

        [TestMethod]
        public void ShouldReturnExceptionForValidateAreaCode()
        {
            var expectedMessage = "AreaCode cannot be null or empty";
            var exception = _nullRequestedParameters.ValidateAreaCode();

            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [TestMethod]
        public void ShouldReturnNullForValidateCategoryAreaCode()
        {
            var exception = _requestedParameters.ValidateCategoryAreaCode();

            Assert.IsNull(exception);
        }

        [TestMethod]
        public void ShouldReturnExceptionForValidateCategoryAreaCode()
        {
            var expectedMessage = "CategoryAreaCode cannot be null or empty";
            var exception = _nullRequestedParameters.ValidateCategoryAreaCode();

            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [TestMethod]
        public void ShouldReturnNullForValidateProfileId()
        {
            var exception = _requestedParameters.ValidateProfileId();

            Assert.IsNull(exception);
        }

        [TestMethod]
        public void ShouldReturnExceptionForValidateProfileId()
        {
            var expectedMessage = "ProfileId cannot be null";
            var exception = _nullRequestedParameters.ValidateProfileId();

            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [TestMethod]
        public void ShouldReturnNullForValidateGroupId()
        {
            var exception = _requestedParameters.ValidateGroupId();

            Assert.IsNull(exception);
        }

        [TestMethod]
        public void ShouldReturnExceptionForValidateGroupId()
        {
            var expectedMessage = "GroupId cannot be null";
            var exception = _nullRequestedParameters.ValidateGroupId();

            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [TestMethod]
        public void ShouldReturnNullForValidateInequalities()
        {
            var exception = _requestedParameters.ValidateInequalities();

            Assert.IsNull(exception);
        }

        [TestMethod]
        public void ShouldReturnExceptionForValidateInequalities()
        {
            var expectedMessage = "Inequalities cannot be null or empty";
            var exception = _nullRequestedParameters.ValidateInequalities();

            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [TestMethod]
        public void ShouldReturnNullForValidateParentAreaCode()
        {
            var exception = _requestedParameters.ValidateParentAreaCode();

            Assert.IsNull(exception);
        }

        [TestMethod]
        public void ShouldReturnExceptionForValidateParentAreaCode()
        {
            var expectedMessage = "ParentAreaCode cannot be null or empty";
            var exception = _nullRequestedParameters.ValidateParentAreaCode();

            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}
