using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class LookUpManagerTest
    {
        [TestMethod]
        public void TestLookUpMethods()
        {
            var lookUpManager = new LookUpManager(ReaderFactory.GetPholioReader(),
                ReaderFactory.GetAreasReader(),
                new List<int>{AreaTypeIds.Ccg},
                new List<int>{CategoryTypeIds.EthnicGroups5});

            // Assert property names
            Assert.AreEqual("18+ yrs", lookUpManager.GetAgeName(AgeIds.Over18));
            Assert.AreEqual("Male", lookUpManager.GetSexName(SexIds.Male));
            Assert.AreEqual("Aggregated from all known lower geography values", lookUpManager.GetValueNoteText(ValueNoteIds.ValueAggregatedFromAllKnownGeographyValues));

            // Assert area look ups 
            Assert.AreEqual("CCG", lookUpManager.GetAreaTypeName(AreaCodes.Ccg_Barnet));
            Assert.AreEqual("NHS Barnet CCG", lookUpManager.GetAreaName(AreaCodes.Ccg_Barnet));

            // Assert category names
            Assert.AreEqual("Ethnic groups", lookUpManager.GetCategoryTypeName(CategoryTypeIds.EthnicGroups5));
            Assert.AreEqual("Black / African / Caribbean / Black British", lookUpManager.GetCategoryName(CategoryTypeIds.EthnicGroups5,
                CategoryIds.EthnicityBlack));
        }
    }
}
