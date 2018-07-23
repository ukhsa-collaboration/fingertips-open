using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.SearchQuerying;

namespace PholioVisualisation.SearchQueryingTest
{
    [TestClass]
    public class IndicatorFilterTest
    {
        private Mock<IIndicatorMetadataRepository> _repo;
        private IndicatorKnowledgeFilter _filter;

        [TestInitialize]
        public void TestInitialize()
        {
            _repo = new Mock<IIndicatorMetadataRepository>(MockBehavior.Strict);
            _filter = new IndicatorKnowledgeFilter(_repo.Object);
        }

        [TestMethod]
        public void Test_No_Matching_Expectations()
        {
            _repo.Setup(x => x.GetIndicatorMetadataSearchExpectations("a"))
                .Returns(new List<IndicatorMetadataSearchExpectation>());

            var ids = _filter.FilterIndicatorIdsForSearchTermExpectations("a", new List<int> { 1 });

            // Assert: id list unchanged
            Assert.AreEqual(1, ids.Count);
        }

        [TestMethod]
        public void Test_Expected_Indicator_Id_Added()
        {
            _repo.Setup(x => x.GetIndicatorMetadataSearchExpectations("a"))
                .Returns(new List<IndicatorMetadataSearchExpectation>
                {
                    new IndicatorMetadataSearchExpectation { Expectation = true, IndicatorId = 2}
                });

            var ids = _filter.FilterIndicatorIdsForSearchTermExpectations("a", new List<int> { 1 });

            // Assert: Expected id added
            Assert.AreEqual(2, ids.Count);
        }

        [TestMethod]
        public void Test_Expected_Excluded_Indicator_Id_Removed()
        {
            _repo.Setup(x => x.GetIndicatorMetadataSearchExpectations("a"))
                .Returns(new List<IndicatorMetadataSearchExpectation>
                {
                    new IndicatorMetadataSearchExpectation { Expectation = false, IndicatorId = 1}
                });

            var ids = _filter.FilterIndicatorIdsForSearchTermExpectations("a", new List<int> { 1 });

            // Assert: Indicator id removed
            Assert.IsFalse(ids.Any());
        }
    }
}

