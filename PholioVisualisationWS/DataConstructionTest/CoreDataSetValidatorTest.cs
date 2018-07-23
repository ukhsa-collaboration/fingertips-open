using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class CoreDataSetValidatorTest
    {
        public const int ValidAgeId = 1;
        public const string ValidAreaCode = "a";

        private Mock<IAreasReader> _areasReader;
        private Mock<IPholioReader> _pholioReader;

        [TestInitialize]
        public void TestInitialize()
        {
            _areasReader = new Mock<IAreasReader>(MockBehavior.Strict);
            _pholioReader = new Mock<IPholioReader>(MockBehavior.Strict);
        }

        [TestMethod]
        public void TestValidate_Valid_Data()
        {
            SetValidAreaCode();
            SetValidAgeId();

            var data = new List<CoreDataSet>
            {
                new CoreDataSet { AgeId = ValidAgeId, AreaCode = ValidAreaCode}
            };

            // Act
            var error = GetErrorMessage(data);

            Assert.AreEqual("",error);
            VerifyAll();
        }

        [TestMethod]
        public void TestValidate_With_Invalid_Area_Code()
        {
            _areasReader.Setup(x => x.GetAreaCodesThatDoNotExist(It.IsAny<IList<string>>()))
                .Returns(new List<string> {"x"});
            SetValidAgeId();

            var data = new List<CoreDataSet>
            {
                new CoreDataSet { AgeId = ValidAgeId, AreaCode = "x"}
            };

            // Act
            var error = GetErrorMessage(data);

            Assert.AreEqual("The following area codes do not exist: x ", error);
            VerifyAll();
        }

        [TestMethod]
        public void TestValidate_With_Invalid_Age_Id()
        {
            SetValidAreaCode();
            SetValidAgeId();

            var data = new List<CoreDataSet>
            {
                new CoreDataSet { AgeId = -99, AreaCode = ValidAreaCode}
            };

            // Act
            var error = GetErrorMessage(data);

            Assert.AreEqual("The following age IDs do not exist: -99", error);
            VerifyAll();
        }

        private void SetValidAgeId()
        {
            _pholioReader.Setup(x => x.GetAllAgeIds())
                .Returns(new List<int> {ValidAgeId});
        }

        private string GetErrorMessage(List<CoreDataSet> data)
        {
            var validator = new CoreDataSetValidator(_areasReader.Object, _pholioReader.Object);
            validator.Validate(data);
            var error = validator.GetErrorMessage();
            return error;
        }

        private void SetValidAreaCode()
        {
            _areasReader.Setup(x => x.GetAreaCodesThatDoNotExist(It.IsAny<IList<string>>()))
                .Returns(new List<string>());
        }

        private void VerifyAll()
        {
            _areasReader.VerifyAll();
            _pholioReader.VerifyAll();
        }
    }
}
