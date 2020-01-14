using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class IndicatorMetaDataTextValueValidatorTest
    {
        [TestMethod]
        public void TestIndicatorMetadataTextValueProfileIdInLive()
        {
            var indicatorMetadataTextValue = new IndicatorMetadataTextValue()
            {
                IndicatorId = IndicatorIds.DeprivationScoreIMD2010,
                ProfileId = ProfileIds.Phof,
                Name = "Deprivation score (IMD 2010)",
                Definition = "<p>The English Indices of Deprivation 2010 use 38 separate indicators, organised across seven distinct domains of deprivation which can be combined, using appropriate weights, to calculate the Index of Multiple Deprivation 2010 (IMD 2010). This is an overall measure of multiple dperivation experienced by people living in an area.</p>",
                Rationale = "Deprivation covers a broad range of issues and refers to unmet needs caused by a lack of resources of all kinds, not just financial. The English Indices of Deprivation attempt to measure a broarder concept of multiple deprivation, made up of several distinct dimensions, or domains, of deprivation."
            };

            Assert.IsTrue(Profiles().Contains(indicatorMetadataTextValue.ProfileId ?? ProfileIds.Undefined));
        }

        [TestMethod]
        public void TestIndicatorMetadataTextValueProfileIdNotInLive()
        {
            var indicatorMetadataTextValue = new IndicatorMetadataTextValue()
            {
                IndicatorId = IndicatorIds.DeprivationScoreIMD2010,
                ProfileId = ProfileIds.Undefined,
                Name = "Deprivation score (IMD 2010)",
                Definition = "<p>The English Indices of Deprivation 2010 use 38 separate indicators, organised across seven distinct domains of deprivation which can be combined, using appropriate weights, to calculate the Index of Multiple Deprivation 2010 (IMD 2010). This is an overall measure of multiple dperivation experienced by people living in an area.</p>",
                Rationale = "Deprivation covers a broad range of issues and refers to unmet needs caused by a lack of resources of all kinds, not just financial. The English Indices of Deprivation attempt to measure a broarder concept of multiple deprivation, made up of several distinct dimensions, or domains, of deprivation."
            };

            Assert.IsFalse(Profiles().Contains(indicatorMetadataTextValue.ProfileId ?? ProfileIds.Undefined));
        }

        private IList<int> Profiles()
        {
            return ReaderFactory.GetProfileReader().GetAllProfileIds();
        }
    }
}
