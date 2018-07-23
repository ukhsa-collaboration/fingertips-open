
using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.DataAccess.Repositories;
using Moq;
using System.Collections.Generic;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class AreaTypeIdSplitterTest
    {
        private const int AreaTypeId1 = 99;
        private const int AreaTypeId2 = 98;

        private Mock<IAreaTypeComponentRepository> _repo;

        [TestInitialize]
        public void TestInitialize()
        {
            _repo = new Mock<IAreaTypeComponentRepository>(MockBehavior.Strict);
        }

        [TestMethod]
        public void Test_Multiple_Area_Types()
        {
            _repo.Setup(x => x.GetAreaTypeComponents(AreaTypeId1))
                .Returns(new List<AreaTypeComponent> {
                    new AreaTypeComponent { ComponentAreaTypeId = 1},
                    new AreaTypeComponent { ComponentAreaTypeId = 2},
                });

            _repo.Setup(x => x.GetAreaTypeComponents(AreaTypeId2))
                .Returns(new List<AreaTypeComponent> {
                    new AreaTypeComponent { ComponentAreaTypeId = 3},
                    new AreaTypeComponent { ComponentAreaTypeId = 4},
                });

            var ids = Splitter().GetComponentAreaTypeIds(
                new[] { AreaTypeId1, AreaTypeId2 });

            Assert.AreEqual(4, ids.Count);
            Assert.IsTrue(ids.Contains(1));
            Assert.IsTrue(ids.Contains(4));
        }

        [TestMethod]
        public void Test_Single_Area_Type()
        {
            _repo.Setup(x => x.GetAreaTypeComponents(AreaTypeId1))
                .Returns(new List<AreaTypeComponent> {
                    new AreaTypeComponent { ComponentAreaTypeId = 1},
                    new AreaTypeComponent { ComponentAreaTypeId = 2},
                });
           
            var ids = Splitter().GetComponentAreaTypeIds(AreaTypeId1);

            Assert.AreEqual(2, ids.Count);
            Assert.IsTrue(ids.Contains(1));
            Assert.IsTrue(ids.Contains(2));
        }

        [TestMethod]
        public void Test_Area_Type_With_No_Components_Return_Input_Id()
        {
            _repo.Setup(x => x.GetAreaTypeComponents(AreaTypeId1))
                .Returns(new List<AreaTypeComponent> {});

            var ids = Splitter().GetComponentAreaTypeIds(AreaTypeId1);

            Assert.AreEqual(1, ids.Count);
            Assert.IsTrue(ids.Contains(AreaTypeId1));
        }

        private IAreaTypeIdSplitter Splitter()
        {
            return new AreaTypeIdSplitter(_repo.Object);
        }
    }
}
