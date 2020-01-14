using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export;
using PholioVisualisation.Export.FileBuilder.Containers;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using System.Collections.Generic;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest.FileBuilder.Containers
{
    [TestClass]
    public class CsvBuilderAttributesForBodyContainerTest
    {
        private IndicatorExportParameters _generalParameters;
        private OnDemandQueryParametersWrapper _onDemandParameters;

        private const int _key = 1;
        private const string _stringTest = "stringTest";
        private const int _indicatorId = IndicatorIds.ChildrenInLowIncomeFamilies;
        private const int _groupId = GroupIds.Phof_WiderDeterminantsOfHealth;

        [TestInitialize]
        public void SetUp()
        {

            _generalParameters = new IndicatorExportParameters { ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 };
            _onDemandParameters = new OnDemandQueryParametersWrapper(_key, new List<int> { _key }, new Dictionary<int, IList<InequalitySearch>> { { _key, null } },  new List<string> { _stringTest }, new List<int> { _key });
        }

        [TestMethod]
        public void ShouldHasGroupingTest()
        {
            var csvBuilderAttributesForBodyContainer = new CsvBuilderAttributesForBodyContainer(_generalParameters, _onDemandParameters);

            Grouping grouping;
            var result = csvBuilderAttributesForBodyContainer.HasGrouping(_indicatorId, new List<int> { _groupId }, out grouping);

            Assert.IsTrue(result);
            Assert.IsNotNull(grouping);
        }

        [TestMethod]
        public void ShouldHasNoGroupingTest()
        {
            var csvBuilderAttributesForBodyContainer = new CsvBuilderAttributesForBodyContainer(_generalParameters, _onDemandParameters);

            Grouping grouping;
            var result = csvBuilderAttributesForBodyContainer.HasGrouping(_key, new List<int> { _key }, out grouping);

            Assert.IsFalse(result);
            Assert.IsNull(grouping);
        }
    }
}
