using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace DataAccessTest
{
    [TestClass]
    public class PholioReaderTest
    {

        [TestMethod]
        public void TestGetQuinaryPopulationLabels()
        {
            IList<string> labels = Reader().GetQuinaryPopulationLabels();
            Assert.AreEqual(20, labels.Count);
            Assert.AreEqual("0-4", labels.First());
            Assert.AreEqual("45-49", labels[9]);
            Assert.AreEqual("95+", labels.Last());
        }

        [TestMethod]
        public void TestGetCcgPracticePopulations()
        {
            var map = Reader().GetCcgPracticePopulations(AreaCodes.Ccg_Chiltern);
            Assert.IsTrue(map.Count > 0);
            foreach (var keyValuePair in map)
            {
                Assert.IsTrue(keyValuePair.Value > 0);
            }
        }

        [TestMethod]
        public void TestGetTargetConfig()
        {
            var target = Reader().GetTargetConfig(1);
            Assert.AreEqual(1, target.Id);
            Assert.AreEqual(PolarityIds.RagHighIsGood, target.PolarityId);
            Assert.IsNotNull(target.Description);
        }

        [TestMethod]
        public void TestGetValueNotes()
        {
            var notes = Reader().GetValueNotes();
            Assert.IsTrue(notes.Count > 0);

            var note = notes.First(x => x.Id == 200);
            Assert.AreEqual("Value estimated", note.Text);
        }

        [TestMethod]
        public void TestGetSexesByIds()
        {
            var sexes = Reader().GetSexesByIds(new List<int>
            {
                SexIds.Male,
                SexIds.Female,
                SexIds.Persons,
            });

            // Check sexes are in expected order
            Assert.AreEqual(3, sexes.Count);
            Assert.AreEqual(SexIds.Persons, sexes[0].Id);
            Assert.AreEqual(SexIds.Male, sexes[1].Id);
            Assert.AreEqual(SexIds.Female, sexes[2].Id);
        }

        [TestMethod]
        public void TestGetAllSexes()
        {
            var sexes = Reader().GetAllSexes();
            Assert.IsTrue(sexes.Count > 3);
        } 

        [TestMethod]
        public void TestGetAgeById()
        {
            var age = Reader().GetAgeById(AgeIds.Over18);
            Assert.AreEqual(AgeIds.Over18, age.Id);
        }

        [TestMethod]
        public void TestGetAgesByIds()
        {
            var ages = Reader().GetAgesByIds(new List<int>
            {
                AgeIds.Over18,
                AgeIds.Over60
            });

            // Check ages are those requested
            Assert.AreEqual(2, ages.Count);
            Assert.AreEqual(AgeIds.Over18, ages[0].Id);
            Assert.AreEqual(AgeIds.Over60, ages[1].Id);
        }

        [TestMethod]
        public void TestGetAllAges()
        {
            var ages = Reader().GetAllAges();
            Assert.IsTrue(ages.Count > 3);
        } 

        [TestMethod]
        public void TestKeyMessageOverride()
        {
            var messages = Reader().GetKeyMessageOverrides(ProfileIds.Phof, AreaCodes.CountyUa_Cambridgeshire);
            Assert.AreEqual(0, messages.Count);
        }

        [TestMethod]
        public void TestGetComparatorConfidence()
        {
            PholioReader reader = Reader();
            var comparatorConfidence = reader.GetComparatorConfidence(ComparatorMethodId.SpcForProportions, 95);
            Assert.AreEqual(ComparatorMethodId.SpcForProportions, comparatorConfidence.ComparatorMethodId);
            Assert.AreEqual(95, comparatorConfidence.ConfidenceValue);
            Assert.AreEqual(1.96, comparatorConfidence.ConfidenceVariable);
        }

        [TestMethod]
        public void TestGetComparatorConfidenceThrowsExceptionIfObjectNotInDatabase()
        {
            try
            {
                Reader().GetComparatorConfidence(1, 2);
            }
            catch (FingertipsException ex)
            {
                Assert.AreEqual(ex.Message, "Invalid comparator method ID (1) and comparator confidence (2) combination");
                return;
            }
            Assert.Fail();
        }

        private static PholioReader Reader()
        {
            PholioReader reader = ReaderFactory.GetPholioReader();
            return reader;
        }
    }
}
