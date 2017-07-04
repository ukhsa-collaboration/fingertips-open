using FingertipsUploadService.ProfileData.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FingertipsUploadService.ProfileDataTest.Helpers
{
    [TestClass]
    public class IndicatorDisclosureControlMApperTest
    {
        private IndicatorDisclosureControlMapper _mapper;
        [TestInitialize]
        public void Setup()
        {
            _mapper = new IndicatorDisclosureControlMapper();
        }

        [TestMethod]
        public void TestSet()
        {
            var indicatorOneId = 10001;
            var disclosureControlIdOne = 1;
            _mapper.Set(indicatorOneId, disclosureControlIdOne);

            Assert.AreEqual(disclosureControlIdOne, _mapper.GetDisclosureControlId(indicatorOneId));
        }

        [TestMethod]
        public void TestGetDisclosureControlIdWithNoRecord()
        {
            var indicatorOneId = 10001;
            Assert.IsNull(_mapper.GetDisclosureControlId(indicatorOneId));
        }
    }
}

