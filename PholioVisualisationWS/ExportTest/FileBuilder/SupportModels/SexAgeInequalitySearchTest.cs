using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.ServicesWebTest.Helpers;
using System.Collections.Generic;

namespace PholioVisualisation.ExportTest.FileBuilder.SupportModels
{
    [TestClass]
    public class SexAgeInequalitySearchTest
    {
        [TestMethod]
        public void ShouldCombineSexAndAgeInSexAgeInequalitySearchList()
        {
            var listSexId = new List<int> { 1 , 2 };
            var listAgeId = new List<int> { 1 , 2 };
            var sexAgeInequalitySearchTest1 = new SexAgeInequalitySearch { AgeId = 1, SexId = 1 };
            var sexAgeInequalitySearchTest2 = new SexAgeInequalitySearch { AgeId = 2, SexId = 1 };
            var sexAgeInequalitySearchTest3 = new SexAgeInequalitySearch { AgeId = 1, SexId = 2 };
            var sexAgeInequalitySearchTest4 = new SexAgeInequalitySearch { AgeId = 2, SexId = 2 };

            var sexAgeInequalitySearchListExpected = new List<SexAgeInequalitySearch>
            {
                sexAgeInequalitySearchTest1,
                sexAgeInequalitySearchTest2,
                sexAgeInequalitySearchTest3,
                sexAgeInequalitySearchTest4
            };

            var sexAgeInequalitySearchListResult = SexAgeInequalitySearch.CombineSexAndAgeInSexAgeInequalitySearchList(listSexId, listAgeId);

            Assert.IsNotNull(sexAgeInequalitySearchListResult);
            AssertHelper.IsType(sexAgeInequalitySearchListExpected.GetType(), sexAgeInequalitySearchListResult);

            Assert.AreEqual(sexAgeInequalitySearchListExpected.Count, sexAgeInequalitySearchListResult.Count);

            for (var i = 0; i < sexAgeInequalitySearchListExpected.Count; i++)
            {
                Assert.AreEqual(sexAgeInequalitySearchListExpected[i].AgeId, sexAgeInequalitySearchListResult[i].AgeId);
                Assert.AreEqual(sexAgeInequalitySearchListExpected[i].SexId, sexAgeInequalitySearchListResult[i].SexId);
            }
        }
    }
}
