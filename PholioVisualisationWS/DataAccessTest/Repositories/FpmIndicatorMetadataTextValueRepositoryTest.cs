using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest.Repositories
{
    [TestClass]
    public class FpmIndicatorMetadataTextValueRepositoryTest
    {
        private FpmIndicatorMetadataTextValueRepository _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new FpmIndicatorMetadataTextValueRepository();
        }

        [TestMethod]
        public void Test_Can_Replace_Indicator_Metadata_TextValues_Object()
        {
            IndicatorMetadataTextValue indicatorMetadataTextValues = GetIndicatorMetadataTextValues();


        }

        private IndicatorMetadataTextValue GetIndicatorMetadataTextValues()
        {
            IndicatorMetadataTextValue indicatorMetadataTextValue = new IndicatorMetadataTextValue();

            indicatorMetadataTextValue.IndicatorId = 20601;
            indicatorMetadataTextValue.ProfileId = null;
            indicatorMetadataTextValue.Name = "Reception: Prevalence of overweight(including obese)";
            indicatorMetadataTextValue.NameLong = "Prevalence of overweight (including obese) among children in Reception";
            indicatorMetadataTextValue.Definition = "Proportion of children aged 4-5 years classified as overweight or obese. Children are classified as overweight (including obese) if their BMI is on or above the 85th centile of the British 1990 growth reference (UK90) according to age and sex.";
            indicatorMetadataTextValue.Rationale = "Test Rationale";


            return indicatorMetadataTextValue;
        }
    }
}
