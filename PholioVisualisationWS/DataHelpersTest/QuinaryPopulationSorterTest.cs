using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataSortingTest
{
    [TestClass]
    public class QuinaryPopulationSorterTest
    {
        [TestMethod]
        public void TestFromZeroToOver95()
        {
            var data = GetCoreDataFromAgeIds(
                AgeIds.From0To4,
                AgeIds.From5To9,
                AgeIds.From10To14,
                AgeIds.From15To19,
                AgeIds.From20To24,
                AgeIds.From25To29,
                AgeIds.From30To34,
                AgeIds.From35To39,
                AgeIds.From40To44,
                AgeIds.From45To49,
                AgeIds.From50To54,
                AgeIds.From55To59,
                AgeIds.From60To64,
                AgeIds.From65To69,
                AgeIds.From70To74,
                AgeIds.From75To79,
                AgeIds.From80To84,
                AgeIds.From85To89,
                AgeIds.From90To95,
                AgeIds.Over95
                );
            Shuffle(data);

            var sortedValues = new QuinaryPopulationSorter(data).SortedValues;

            var index = 0;
            Assert.AreEqual(AgeIds.From0To4, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From5To9, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From10To14, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From15To19, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From20To24, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From25To29, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From30To34, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From35To39, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From40To44, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From45To49, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From50To54, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From55To59, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From60To64, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From65To69, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From70To74, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From75To79, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From80To84, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From85To89, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From90To95, sortedValues[index++]);
            Assert.AreEqual(AgeIds.Over95, sortedValues[index]);
        }

        [TestMethod]
        public void TestFromZeroToOver90()
        {
            var data = GetCoreDataFromAgeIds(
                AgeIds.From0To4,
                AgeIds.From5To9,
                AgeIds.From10To14,
                AgeIds.From15To19,
                AgeIds.From20To24,
                AgeIds.From25To29,
                AgeIds.From30To34,
                AgeIds.From35To39,
                AgeIds.From40To44,
                AgeIds.From45To49,
                AgeIds.From50To54,
                AgeIds.From55To59,
                AgeIds.From60To64,
                AgeIds.From65To69,
                AgeIds.From70To74,
                AgeIds.From75To79,
                AgeIds.From80To84,
                AgeIds.From85To89,
                AgeIds.Over90
                );
            Shuffle(data);

            var sortedValues = new QuinaryPopulationSorter(data).SortedValues;

            var index = 0;
            Assert.AreEqual(AgeIds.From0To4, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From5To9, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From10To14, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From15To19, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From20To24, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From25To29, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From30To34, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From35To39, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From40To44, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From45To49, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From50To54, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From55To59, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From60To64, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From65To69, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From70To74, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From75To79, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From80To84, sortedValues[index++]);
            Assert.AreEqual(AgeIds.From85To89, sortedValues[index++]);
            Assert.AreEqual(AgeIds.Over90, sortedValues[index]);
        }

        public IList<CoreDataSet> GetCoreDataFromAgeIds(params int[] ageIds)
        {
            var data = new List<CoreDataSet>();
            foreach (var ageId in ageIds)
            {
                data.Add(new CoreDataSet
                {
                    Value = ageId,
                    AgeId = ageId
                });
            }
            return data;
        }

        public static void Shuffle(IList<CoreDataSet> list)
        {
            Random random = new Random();
            int count = list.Count;
            while (count > 1)
            {
                count--;
                int k = random.Next(count + 1);
                var value = list[k];
                list[k] = list[count];
                list[count] = value;
            }
        }
    }
}
