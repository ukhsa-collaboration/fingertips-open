using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesTest;
using PholioVisualisation.ServicesWebTest.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.ExportTest.FileBuilder.Wrappers
{
    [TestClass]
    public class InequalitySearchTest
    {
        private GroupingInequality _groupingInequality;
        private GroupingInequality _groupingInequalityWithNullGroupId;
        private GroupingInequality _groupingInequalityDummy;
        private static int IndicatorId;
        private static int AreaTypeId;
        private static int GroupId;
        private IEnumerable<int> _indicatorIdList;
        private string[] _parentCategoryAreaCode;

        private static int IncrementingValue;
        private static int SexId;
        private static int AgeId;
        private static int DifferentAgeTestId;
        private static int DifferentSexTestId;
        private static int CategoryTypeTestId;
        private static int CategoryTestId;
        private static int ProfileId;
        private static Grouping _grouping;
        private static Grouping _groupingPerson;

        private Mock<IGroupDataReader> _groupDataReaderMock;
        private Mock<IProfileReader> _profileReaderMock;
        private Mock<IAreasReader> _areasReaderMock;
        private IList<Grouping> _groupingList;
        private IList<Grouping> _groupingPersonList;
        private IList<GroupingMetadata> _groupingMetadataList;

        private Category _category;

        [TestInitialize]
        public void Start()
        {
            IndicatorId = TestHelper.GetRandomInt();
            AreaTypeId = TestHelper.GetRandomInt();
            GroupId = TestHelper.GetRandomInt();
            IncrementingValue = TestHelper.GetRandomInt();
            SexId = TestHelper.GetRandomInt();
            AgeId = TestHelper.GetRandomInt();
            DifferentAgeTestId = AgeId + IncrementingValue;
            DifferentSexTestId = SexId + IncrementingValue;
            CategoryTypeTestId = TestHelper.GetRandomInt();
            CategoryTestId = TestHelper.GetRandomInt();
            ProfileId = TestHelper.GetRandomInt();

            _groupingInequality = new GroupingInequality(IndicatorId, AreaTypeId, GroupId, SexId, AgeId);
            _groupingInequalityWithNullGroupId = new GroupingInequality(IndicatorId, AreaTypeId, null, SexId, AgeId);
            _groupingInequalityDummy = new GroupingInequality(IndicatorId, AreaTypeId, null, SexId, AgeId);
            _indicatorIdList = new List<int>{ IndicatorId };
            _parentCategoryAreaCode = new [] {"cat-01-01"};

            _groupDataReaderMock = new Mock<IGroupDataReader>();
            _profileReaderMock = new Mock<IProfileReader>();
            _areasReaderMock = new Mock<IAreasReader>();

            _grouping = new Grouping { AgeId = AgeId, AreaTypeId = AreaTypeId, IndicatorId = IndicatorId, SexId = SexId };
            _groupingPerson = new Grouping { AgeId = AgeId, AreaTypeId = AreaTypeId, IndicatorId = IndicatorId, SexId = SexIds.Persons };
            _groupingMetadataList = new List<GroupingMetadata> { new GroupingMetadata { Id = 1, Name = "DevelopmentTest", ProfileId = ProfileId, Sequence = 1 } };
            _groupingList = new List<Grouping> { _grouping, _grouping };
            _groupingPersonList = new List<Grouping> { _grouping, _groupingPerson };

            _category = new Category {CategoryTypeId = 1, Id = 1, Name = "testName", ShortName = "shortTestName"};
        }

        [TestMethod]
        public void ShouldCloneInequalitySearch()
        {
            var expectedInequalitySearch = new InequalitySearch(1, 1, 1, 1);
            var targetInequalitySearch = new InequalitySearch(1, 1, 1, 1);
            var cloneInequalitySearch = InequalitySearch.CloneInequalitySearch(targetInequalitySearch);

            Assert.IsTrue(cloneInequalitySearch != expectedInequalitySearch);
            AssertHelper.AreEqual(cloneInequalitySearch, expectedInequalitySearch);
        }

        [TestMethod]
        public void ShouldBeContained()
        {
            var categoryInequalitySearchUndefinedValueTest = CategoryInequalitySearch.GetUndefinedCategoryInequality();
            var categoryInequalitySearchValueTest = new CategoryInequalitySearch { CategoryTypeId = CategoryTypeTestId, CategoryId = CategoryTestId };
            var sexAgeInequalitySearchValueTest = new SexAgeInequalitySearch { AgeId = AgeId, SexId = AgeId };
            var sexAgeInequalitySearchDifferentValueTest = new SexAgeInequalitySearch { AgeId = DifferentAgeTestId, SexId = DifferentSexTestId };

            var inequalitySearch = new InequalitySearch { CategoryInequalitySearch = categoryInequalitySearchValueTest, SexAgeInequalitySearch = sexAgeInequalitySearchDifferentValueTest };

            var inequalitySearchList = new List<InequalitySearch>
            {
                new InequalitySearch { CategoryInequalitySearch = categoryInequalitySearchUndefinedValueTest, SexAgeInequalitySearch = sexAgeInequalitySearchValueTest },
                new InequalitySearch { CategoryInequalitySearch = categoryInequalitySearchUndefinedValueTest, SexAgeInequalitySearch = sexAgeInequalitySearchDifferentValueTest },
                new InequalitySearch { CategoryInequalitySearch = categoryInequalitySearchValueTest, SexAgeInequalitySearch = sexAgeInequalitySearchValueTest },
                inequalitySearch
            };
            Assert.IsTrue(InequalitySearch.ContainsInequality(inequalitySearchList, inequalitySearch));
        }

        [TestMethod]
        public void ShouldBeNotContained()
        {
            var categoryInequalitySearchUndefinedValueTest = CategoryInequalitySearch.GetUndefinedCategoryInequality();
            var categoryInequalitySearchValueTest = new CategoryInequalitySearch { CategoryTypeId = CategoryTypeTestId, CategoryId = CategoryTestId };
            var sexAgeInequalitySearchValueTest = new SexAgeInequalitySearch { AgeId = AgeId, SexId = SexId };
            var sexAgeInequalitySearchDifferentValueTest = new SexAgeInequalitySearch { AgeId = DifferentAgeTestId, SexId = DifferentSexTestId };

            var inequalitySearch = new InequalitySearch { CategoryInequalitySearch = categoryInequalitySearchValueTest, SexAgeInequalitySearch = sexAgeInequalitySearchDifferentValueTest };

            var inequalitySearchList = new List<InequalitySearch>
            {
                new InequalitySearch { CategoryInequalitySearch = categoryInequalitySearchUndefinedValueTest, SexAgeInequalitySearch = sexAgeInequalitySearchValueTest },
                new InequalitySearch { CategoryInequalitySearch = categoryInequalitySearchUndefinedValueTest, SexAgeInequalitySearch = sexAgeInequalitySearchDifferentValueTest },
                new InequalitySearch { CategoryInequalitySearch = categoryInequalitySearchValueTest, SexAgeInequalitySearch = sexAgeInequalitySearchValueTest }
            };
            Assert.IsFalse(InequalitySearch.ContainsInequality(inequalitySearchList, inequalitySearch));
        }

        [TestMethod]
        public void ShouldGetCategoryInequalitySearch()
        {
            _areasReaderMock.Setup(x => x.GetCategory(It.IsAny<int>(), It.IsAny<int>())).Returns(_category);

            var expectedCategoryInequality = new CategoryInequalitySearch(1, 1);
            var resultCategoryInequality = InequalitySearch.GetCategoryInequalitySearch(_parentCategoryAreaCode.First(), _areasReaderMock.Object);
            Assert.IsTrue(expectedCategoryInequality.CategoryTypeId == resultCategoryInequality.CategoryTypeId &&
                          expectedCategoryInequality.CategoryId == resultCategoryInequality.CategoryId);
        }

        [TestMethod]
        public void ShouldThrowAnExceptionInGetCategoryInequalitySearchWhenCodeIsNull()
        {
            try
            {
                InequalitySearch.GetCategoryInequalitySearch(null, _areasReaderMock.Object);
                Assert.Fail("An exception should be thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true);
                Assert.AreEqual(e.Message, "The areaCode must contains a value");
            }
            catch (Exception)
            {
                Assert.Fail("The exception should be an argumentException");
            }
        }

        [TestMethod]
        public void ShouldThrowAnExceptionInGetCategoryInequalitySearchWhenCodeIsEmpty()
        {
            try
            {
                InequalitySearch.GetCategoryInequalitySearch("", _areasReaderMock.Object);
                Assert.Fail("An exception should be thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true);
                Assert.AreEqual(e.Message, "The areaCode must contains a value");
            }
            catch (Exception)
            {
                Assert.Fail("The exception should be an argumentException");
            }
        }

        [TestMethod]
        public void ShouldThrowAnExceptionInGetCategoryInequalitySearchWhenCodeIsInValid()
        {
            try
            {
                InequalitySearch.GetCategoryInequalitySearch("InvalidCategoryCode", _areasReaderMock.Object);
                Assert.Fail("An exception should be thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true);
                Assert.AreEqual(e.Message, "The areaCode must be a categoryAreaCode");
            }
            catch (Exception)
            {
                Assert.Fail("The exception should be an argumentException");
            }
        }

        [TestMethod]
        public void ShouldGetGroupingSexAgeInequalityByIndicatorGroupAreaType()
        {
            _groupDataReaderMock.Setup(x => x.GetGroupingListByGroupIdIndicatorIdAreaType(GroupId, IndicatorId, AreaTypeId)).Returns(_groupingList);

            var sexAgeInequalityByIndicatorGroupAreaTypeList = InequalitySearch.GetGroupingSexAgeInequalityByIndicatorGroupAreaType(_groupingInequality, _groupDataReaderMock.Object);

            Assert.IsTrue(sexAgeInequalityByIndicatorGroupAreaTypeList.Count == 1);
            Assert.IsTrue(sexAgeInequalityByIndicatorGroupAreaTypeList.Any(elem => elem.SexId == SexId && elem.AgeId == AgeId));
        }

        [TestMethod]
        public void ShouldGetGroupingSexAgeInequalityByIndicatorGroupAreaTypeWithPerson()
        {
            _groupDataReaderMock.Setup(x => x.GetGroupingListByGroupIdIndicatorIdAreaType(GroupId, IndicatorId, AreaTypeId)).Returns(_groupingPersonList);

            var sexAgeInequalityByIndicatorGroupAreaTypeList = InequalitySearch.GetGroupingSexAgeInequalityByIndicatorGroupAreaType(_groupingInequality, _groupDataReaderMock.Object);

            Assert.IsTrue(sexAgeInequalityByIndicatorGroupAreaTypeList.Count == 2);
            Assert.IsTrue(sexAgeInequalityByIndicatorGroupAreaTypeList.Any(elem => elem.SexId == SexIds.Persons && elem.AgeId == AgeId));
        }

        [TestMethod]
        public void ShouldGetListCategoryInequalities()
        {
            _areasReaderMock.Setup(x => x.GetCategory(It.IsAny<int>(), It.IsAny<int>())).Returns(_category);

            var expectedCategoryInequality1 = new CategoryInequalitySearch(-1, -1);
            var expectedCategoryInequality2 = new CategoryInequalitySearch(1, 1);
            var categoryInequalityList = InequalitySearch.GetListCategoryInequalities(_parentCategoryAreaCode, _areasReaderMock.Object);

            Assert.IsTrue(categoryInequalityList.Count == 2);
            Assert.IsTrue(categoryInequalityList.Any(categoryElement => categoryElement.CategoryTypeId == expectedCategoryInequality1.CategoryTypeId &&
                                                                        categoryElement.CategoryId == expectedCategoryInequality1.CategoryId));
            Assert.IsTrue(categoryInequalityList.Any(categoryElement => categoryElement.CategoryTypeId == expectedCategoryInequality2.CategoryTypeId &&
                                                                        categoryElement.CategoryId == expectedCategoryInequality2.CategoryId));
        }

        [TestMethod]
        public void ShouldAddInequalitiesToDictionary()
        {
            _areasReaderMock.Setup(x => x.GetCategory(It.IsAny<int>(), It.IsAny<int>())).Returns(_category);
            _groupDataReaderMock.Setup(x => x.GetGroupingListByGroupIdIndicatorIdAreaType(GroupId, IndicatorId, AreaTypeId)).Returns(_groupingList);

            IList<InequalitySearch> listInequality;
            var expectedCategoryIdInequality = new InequalitySearch(-1, -1, _groupingList[0].SexId, _groupingList[0].AgeId);
            var expectedCategoryIdInequality2 = new InequalitySearch( 1, 1, _groupingList[1].SexId, _groupingList[1].AgeId);
            IDictionary<int, IList<InequalitySearch>> inequalitiesDictionary = new Dictionary<int, IList<InequalitySearch>>();

            inequalitiesDictionary = InequalitySearch.AddInequalitiesToDictionary(_groupDataReaderMock.Object, _areasReaderMock.Object, _groupingInequality, _parentCategoryAreaCode, inequalitiesDictionary);

            inequalitiesDictionary.TryGetValue(IndicatorId, out listInequality);

            Assert.IsTrue(inequalitiesDictionary.Count == 1);
            Assert.IsTrue(_indicatorIdList.All(indicator => inequalitiesDictionary.ContainsKey(IndicatorId)));
            
            Assert.IsTrue(listInequality != null && listInequality.Count == 2);
            AssertHelper.DoesContain(listInequality, expectedCategoryIdInequality);
            AssertHelper.DoesContain(listInequality, expectedCategoryIdInequality2);
        }

        [TestMethod]
        public void ShouldAddInequalitiesToDictionaryWithNullGroupId()
        {
            _areasReaderMock.Setup(x => x.GetCategory(It.IsAny<int>(), It.IsAny<int>())).Returns(_category);
            _groupDataReaderMock.Setup(x => x.GetGroupingsByGroupId(IndicatorId)).Returns(_groupingList);
            _groupDataReaderMock.Setup(x => x.GetGroupingListByIndicatorIdAreaType(IndicatorId, AreaTypeId)).Returns(_groupingList);

            IList<InequalitySearch> listInequality;
            var expectedCategoryIdInequality = new InequalitySearch(-1, -1, _groupingList[0].SexId, _groupingList[0].AgeId);
            var expectedCategoryIdInequality2 = new InequalitySearch(1, 1, _groupingList[1].SexId, _groupingList[1].AgeId);
            IDictionary<int, IList<InequalitySearch>> inequalitiesDictionary = new Dictionary<int, IList<InequalitySearch>>();

            inequalitiesDictionary = InequalitySearch.AddInequalitiesToDictionary(_groupDataReaderMock.Object, _areasReaderMock.Object, _groupingInequalityWithNullGroupId, _parentCategoryAreaCode, inequalitiesDictionary);

            inequalitiesDictionary.TryGetValue(IndicatorId, out listInequality);

            Assert.IsTrue(inequalitiesDictionary.Count == 1);
            Assert.IsTrue(_indicatorIdList.All(indicator => inequalitiesDictionary.ContainsKey(IndicatorId)));

            Assert.IsTrue(listInequality != null && listInequality.Count == 2);
            AssertHelper.DoesContain(listInequality, expectedCategoryIdInequality);
            AssertHelper.DoesContain(listInequality, expectedCategoryIdInequality2);
        }

        [TestMethod]
        public void ShouldOnlyInitializedInequalitiesInDictionary()
        {
            _areasReaderMock.Setup(x => x.GetCategory(It.IsAny<int>(), It.IsAny<int>())).Returns(_category);
            _groupDataReaderMock.Setup(x => x.GetGroupingListByIndicatorIdAreaType(IndicatorId, AreaTypeId)).Returns(_groupingList);

            IList<InequalitySearch> listInequality;
            IDictionary<int, IList<InequalitySearch>> inequalitiesDictionary = _indicatorIdList.ToDictionary<int, int, IList<InequalitySearch>>(indicator => indicator, indicator => null);

            inequalitiesDictionary = InequalitySearch.AddInequalitiesToDictionary(_groupDataReaderMock.Object, _areasReaderMock.Object, _groupingInequalityDummy, _parentCategoryAreaCode, inequalitiesDictionary);

            Assert.IsTrue(inequalitiesDictionary.Count == _indicatorIdList.Count());
            Assert.IsTrue(_indicatorIdList.All(indicator => inequalitiesDictionary.ContainsKey(indicator)));
            Assert.IsTrue(_indicatorIdList.All(indicator =>
            {
                inequalitiesDictionary.TryGetValue(indicator, out listInequality);
                return listInequality != null && listInequality.Count == 2;
            }));
        }
        
        [TestMethod]
        public void ShouldCombineTwoListsIntoInequalitySearchList()
        {
            var categoryInequalitySearchUndefinedValueTest = CategoryInequalitySearch.GetUndefinedCategoryInequality();
            var categoryInequalitySearchValueTest = new CategoryInequalitySearch { CategoryTypeId = CategoryTypeTestId, CategoryId = CategoryTestId };
            var sexAgeInequalitySearchValueTest = new SexAgeInequalitySearch {AgeId = AgeId, SexId = SexId};
            var sexAgeInequalitySearchDifferentValueTest = new SexAgeInequalitySearch { AgeId = DifferentAgeTestId, SexId = DifferentSexTestId };

            var expectedCombinedList = new List<InequalitySearch>
            {
                new InequalitySearch { CategoryInequalitySearch = categoryInequalitySearchUndefinedValueTest, SexAgeInequalitySearch = sexAgeInequalitySearchValueTest },
                new InequalitySearch { CategoryInequalitySearch = categoryInequalitySearchUndefinedValueTest, SexAgeInequalitySearch = sexAgeInequalitySearchDifferentValueTest },
                new InequalitySearch { CategoryInequalitySearch = categoryInequalitySearchValueTest, SexAgeInequalitySearch = sexAgeInequalitySearchValueTest },
                new InequalitySearch { CategoryInequalitySearch = categoryInequalitySearchValueTest, SexAgeInequalitySearch = sexAgeInequalitySearchDifferentValueTest }
            };
            
            var categoryInequalitySearchList = new List<CategoryInequalitySearch>
            {
                categoryInequalitySearchUndefinedValueTest,
                categoryInequalitySearchValueTest
            };
            var sexAgeInequalitySearchList = new List<SexAgeInequalitySearch>
            {
                sexAgeInequalitySearchValueTest,
                sexAgeInequalitySearchDifferentValueTest
        };

            var resultCombinedLists = InequalitySearch.CombineTwoListsIntoInequalitySearchList(categoryInequalitySearchList, sexAgeInequalitySearchList);

            Assert.IsNotNull(resultCombinedLists);
            Assert.AreEqual(expectedCombinedList.Count, resultCombinedLists.Count);
            AssertHelper.AreEqual(expectedCombinedList, resultCombinedLists);
        }

        [TestMethod]
        public void ShouldBeContainedInequalityInTheList()
        {
            var categoryIdInequalityComparator = new InequalitySearch
            {
                CategoryInequalitySearch = CategoryInequalitySearch.GetUndefinedCategoryInequality(),
                SexAgeInequalitySearch = new SexAgeInequalitySearch
                {
                    AgeId = AgeId,
                    SexId = SexId
                }
            };
            var inequalities = new List<InequalitySearch> { categoryIdInequalityComparator };

            AssertHelper.DoesContain(inequalities, categoryIdInequalityComparator);
        }

        [TestMethod]
        public void ShouldNotBeContainedTheInequalityInTheList()
        {
            var previousInequality = new InequalitySearch
            {
                CategoryInequalitySearch = CategoryInequalitySearch.GetUndefinedCategoryInequality(),
                SexAgeInequalitySearch =  new SexAgeInequalitySearch
                {
                    AgeId = AgeId,
                    SexId = SexId
                }
            };

            var categoryIdInequalityComparator = new InequalitySearch
            {
                CategoryInequalitySearch = new CategoryInequalitySearch
                {
                    CategoryTypeId = CategoryTypeTestId,
                    CategoryId = CategoryTestId
                },
                SexAgeInequalitySearch = new SexAgeInequalitySearch
                {
                    AgeId = DifferentAgeTestId,
                    SexId = DifferentSexTestId
                }
            };

            var inequalities = new List<InequalitySearch> { previousInequality };

            AssertHelper.DoesNotContain(inequalities, categoryIdInequalityComparator);
        }
    }
}
