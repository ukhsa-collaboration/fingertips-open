using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.UserData.IndicatorLists;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class GroupDataBuilderByIndicatorListTest
    {
        private const string IndicatorListPublicId = "2";
        private const int areaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019;

        private Mock<IIndicatorListRepository> _repo;
        private Mock<SingleGroupingProvider> _singleGroupingProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _repo = new Mock<IIndicatorListRepository>(MockBehavior.Strict);
            _repo.Setup(x => x.GetIndicatorList(IndicatorListPublicId)).Returns(GetIndicatorList());

            _singleGroupingProvider = new Mock<SingleGroupingProvider>();
            _singleGroupingProvider.Setup(x => x.GetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(
                areaTypeId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(GetGrouping());
        }

        [TestMethod]
        public void TestBuild()
        {
            var builder = new GroupDataBuilderByIndicatorList(_repo.Object, _singleGroupingProvider.Object)
            {
                ProfileId = ProfileIds.Undefined,
                IndicatorListPublicId = IndicatorListPublicId,
                ComparatorMap = new ComparatorMapBuilder(areaTypeId).ComparatorMap,
                ChildAreaTypeId = areaTypeId
            };

            var groupData = builder.Build();

            Assert.IsNotNull(groupData);
            VerifyAll();
        }

        private void VerifyAll()
        {
            _repo.VerifyAll();
            _singleGroupingProvider.VerifyAll();
        }

        private IndicatorList GetIndicatorList()
        {
            return new IndicatorList
            {
                PublicId = IndicatorListPublicId,
                IndicatorListItems = new List<IndicatorListItem>
                {
                    new IndicatorListItem
                    {
                        IndicatorId = IndicatorIds.LifeExpectancyAtBirth,
                        SexId = SexIds.Persons,
                        AgeId = AgeIds.AllAges
                    }
                }
            };
        }

        private Grouping GetGrouping()
        {
            return new Grouping();
        }
    }
}
