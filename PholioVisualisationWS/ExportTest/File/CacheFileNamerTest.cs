using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export;
using PholioVisualisation.Export.File;
using PholioVisualisation.Export.FileBuilder.Containers;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.Export.FileBuilder.Wrappers;

namespace PholioVisualisation.ExportTest.File
{
    [TestClass]
    public class CacheFileNamerTest
    {
        public CsvBuilderAttributesForBodyContainer Parameters;
        public IndicatorExportParameters IndicatorExportParameters;
        public OnDemandQueryParametersWrapper OnDemandParameters;

        public int ProfileId;
        public List<int> IndicatorsId;
        public Dictionary<int, IList<InequalitySearch>> IndicatorInequalitiesDictionary;
        public List<string> ChildAreaCodeList;
        public List<int> GroupIdList;
        public bool AllPeriods;

        [TestInitialize]
        public void StartUp()
        {
            ProfileId = 1;
            IndicatorsId = new List<int>
            {
                000000, 000001, 000002, 000003, 000004, 000005, 000006, 000007, 000008, 000009, 0000010, 0000011, 0000012,
                0000013, 0000014, 0000015, 0000016, 0000017, 0000018, 0000019, 0000020, 0000021, 0000022, 0000023, 0000024,
                0000025, 0000026, 0000027, 0000028, 0000029, 0000030, 0000031, 0000032, 0000033, 0000034, 0000035, 0000036,
                0000037, 0000038, 0000039, 0000040, 0000041, 0000042, 0000043, 0000044, 0000045, 0000046, 0000047, 0000048,
                0000049, 0000050, 0000051, 0000052, 0000053, 0000054, 0000055, 0000056, 0000057, 0000058, 0000059, 0000060
            };

            var inequalityList = new List<InequalitySearch>
            {
                new InequalitySearch(1, 1), new InequalitySearch(1, 1, 1, 1), new InequalitySearch(1, 1), new InequalitySearch(1, 1, 1, 1),
                new InequalitySearch(1, 1), new InequalitySearch(1, 1, 1, 1), new InequalitySearch(1, 1, 1, 1), new InequalitySearch(1, 1, 1, 1)
            };

            IndicatorInequalitiesDictionary = new Dictionary<int, IList<InequalitySearch>>();
            IndicatorsId.ForEach(id => IndicatorInequalitiesDictionary.Add(id,inequalityList));

           
            IndicatorExportParameters =  new IndicatorExportParameters { ChildAreaTypeId = 1, ParentAreaCode = "E00000001", ParentAreaTypeId = 1, ProfileId = ProfileId };

            ChildAreaCodeList = new List<string>
            {
                "E00000001","E00000002","E00000003", "E00000004", "E00000005", "E00000006",
                "E00000007", "E00000008", "E00000009", "E000000010", "E000000011", "E000000012",
                "E000000013", "E000000014", "E000000015", "E000000016", "E000000017","E000000018",
                "E000000019","E000000020","E00000001","E00000002","E00000003", "E00000004", "E00000005", "E00000006",
                "E00000007", "E00000008", "E00000009", "E000000010", "E000000011", "E000000012",
                "E000000013", "E000000014", "E000000015", "E000000016", "E000000017","E000000018",
                "E000000019","E000000020","E00000001","E00000002","E00000003", "E00000004", "E00000005", "E00000006",
                "E00000007", "E00000008", "E00000009", "E000000010", "E000000011", "E000000012",
                "E000000013", "E000000014", "E000000015", "E000000016", "E000000017","E000000018",
                "E000000019","E000000020"
            };

            GroupIdList = new List<int> { 2000000000, 2000000000, 2000000000, 2000000000, 2000000000, 2000000000, 2000000000 };
            AllPeriods = true;

            OnDemandParameters = new OnDemandQueryParametersWrapper(ProfileId, IndicatorsId, IndicatorInequalitiesDictionary, ChildAreaCodeList, GroupIdList, AllPeriods);

            Parameters = new CsvBuilderAttributesForBodyContainer(IndicatorExportParameters, OnDemandParameters);
        }

        [TestMethod]
        public void TestGetAddressFileName()
        {
            Assert.AreEqual("a-1.addresses.csv", CacheFileNamer.GetAddressFileName("a", 1));
        }

        [TestMethod]
        public void TestGetIndicatorFileName()
        {
            var expected = "25-ad-1RAB3Qwm_gS_QPCNnkJCQ4GiczDozLeA2A6__tr9cCFqjABSmYBatgF5wWjnepqeVoO1qP9qMD6AI6gW5w_M7_5vkPpZy59sHznwjBCCYWNg4HQVI0w8nFzaMhiJKsaGpp63gYpmU7nl7ePhlBGMVJZlZ2znwW3-2P6Q4B9iQfAwMAAA.data.csv";
            Assert.AreEqual(expected, CacheFileNamer.GetIndicatorFileName(0000025, Parameters));
        }

        [TestMethod]
        public void TestGetIndicatorFileNameWithNullChildAreaCode()
        {
            var expected = "25-zwAAAB-LCAAAAAAABAClybsNwkAQRdGW5v1md0MHdECOCJw5skX_CCEq4ERXurjVF4D7-dq343hs5_68CpTTYy58HiEYQWNgYrEIkqIZNgcnl0oQJVlRa2hquQzTsh23h6dXKgijOElnZGZ1sX7-qTciTGaszwAAAA.data.csv";

            var OnDemandParametersWithNullChildAreaCode = new OnDemandQueryParametersWrapper(ProfileId, IndicatorsId, IndicatorInequalitiesDictionary, null, GroupIdList, AllPeriods);
            var ParametersWithNullChildAreaCode = new CsvBuilderAttributesForBodyContainer(IndicatorExportParameters, OnDemandParametersWithNullChildAreaCode);

            Assert.AreEqual(expected, CacheFileNamer.GetIndicatorFileName(0000025, ParametersWithNullChildAreaCode));
        }

        [TestMethod]
        public void TestNoneTruncateString()
        {
            var expected = "25-JwAAAB-LCAAAAAAABAAzdDWAAENDw5Ci0lTHnJx4x6LUxGIDQyNjIwMYAACjCdHEJwAAAA.data.csv";

            var IndicatorsId = new List<int> { 000000, 000001, 000002, 000003 };
            var GroupIdList = new List<int> { 2000000000 };
            var inequalityList = new List<InequalitySearch> { new InequalitySearch(1, 1) };

            var IndicatorInequalitiesDictionary = new Dictionary<int, IList<InequalitySearch>>();
            IndicatorsId.ForEach(id => IndicatorInequalitiesDictionary.Add(id, inequalityList));

            var OnDemandParametersWithNullChildAreaCode = new OnDemandQueryParametersWrapper(ProfileId, IndicatorsId, IndicatorInequalitiesDictionary, null, GroupIdList, AllPeriods);
            var ParametersWithNullChildAreaCode = new CsvBuilderAttributesForBodyContainer(IndicatorExportParameters, OnDemandParametersWithNullChildAreaCode);

            Assert.AreEqual(expected, CacheFileNamer.GetIndicatorFileName(0000025, ParametersWithNullChildAreaCode));
        }


    }
}
