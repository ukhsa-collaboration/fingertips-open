using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class AgeSorterTest
    {
        [TestMethod]
        public void TestSortByAge()
        {
            var ageLabel1 ="10-14 yrs";
            var ageLabel2 ="25-29 yrs";
            var ageLabel3 ="30-34 yrs";

            var ages = new List<Age>
            {
                new Age {Name = ageLabel2},
                new Age {Name = ageLabel3},
                new Age {Name = ageLabel1},
            };

            var sortedAges = new AgeSorter().SortByAge(ages);

            Assert.AreEqual(ageLabel1, sortedAges[0].Name);
            Assert.AreEqual(ageLabel2, sortedAges[1].Name);
            Assert.AreEqual(ageLabel3, sortedAges[2].Name);
        }

        [TestMethod]
        public void TestSortByAgeWithGreaterLesserSymbols()
        {
            var ageLabel1 = "<1";
            var ageLabel2 = "2+";

            var ages = new List<Age>
            {
                new Age {Name = ageLabel2},
                new Age {Name = ageLabel1},
            };

            var sortedAges = new AgeSorter().SortByAge(ages);

            Assert.AreEqual(ageLabel1, sortedAges[0].Name);
            Assert.AreEqual(ageLabel2, sortedAges[1].Name);
        }

        [TestMethod]
        public void TestSortByAgeWhereSpaceBeforeNumber()
        {
            var ageLabel1 = "< 1 yr";
            var ageLabel2 = "2+";

            var ages = new List<Age>
            {
                new Age {Name = ageLabel2},
                new Age {Name = ageLabel1},
            };

            var sortedAges = new AgeSorter().SortByAge(ages);

            Assert.AreEqual(ageLabel1, sortedAges[0].Name);
            Assert.AreEqual(ageLabel2, sortedAges[1].Name);
        }

        [TestMethod]
        public void TestSortByAgeWithNamesThatDoNotContainNumbers()
        {
            var ageLabel1 = "School age";
            var ageLabel2 = "Adult";

            var ages = new List<Age>
            {
                new Age {Name = ageLabel1},
                new Age {Name = ageLabel2},
            };

            var sortedAges = new AgeSorter().SortByAge(ages);

            Assert.AreEqual(ageLabel1, sortedAges[0].Name);
            Assert.AreEqual(ageLabel2, sortedAges[1].Name);
        }

        [TestMethod]
        public void TestSortByAgeWhereSameNumberContainedInLabel()
        {
            var ageLabel1 = "<18";
            var ageLabel2 = "18+";

            var ages = new List<Age>
            {
                new Age {Name = ageLabel1},
                new Age {Name = ageLabel2}
            };

            var sortedAges = new AgeSorter().SortByAge(ages);

            Assert.AreEqual(ageLabel1, sortedAges[0].Name);
            Assert.AreEqual(ageLabel2, sortedAges[1].Name);
        }
    }
}
