using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class PartitionTrendDataDictionaryBuilderTest
    {
        public const int Sex1 = SexIds.Male;
        public const int Sex2 = SexIds.Female;

        [TestMethod]
        public void Test_Build_Dictionary()
        {
            var builder = GetPartitionTrendDataDictionaryBuilderWithDataAdded();

            var dictionary = builder.Dictionary;

            // Assert
            Assert.AreEqual(2, dictionary.Keys.Count, "Expected same number of keys as sexes");
            Assert.AreEqual(2, dictionary[Sex1].Count);
            Assert.AreEqual(2, dictionary[Sex2].Count);
            Assert.AreEqual(4, builder.AllDataAsList.Count);
        }

        [TestMethod]
        public void Test_Entity_Can_Be_Removed()
        {
            var builder = GetPartitionTrendDataDictionaryBuilderWithDataAdded();
            builder.RemoveEntity(1);

            var dictionary = builder.Dictionary;

            // Assert
            Assert.AreEqual(1, dictionary.Keys.Count, "Expected only one sex to be left");
            Assert.IsFalse(dictionary.ContainsKey(1));
            Assert.AreEqual(2, dictionary[Sex2].Count);
        }

        [TestMethod]
        public void When_Valid_Early_Data_Then_Remove_Index_Is_Minus_One()
        {
            var builder = GetPartitionTrendDataDictionaryBuilderWithDataAdded();
            var index = builder.RemoveEarlyEmptyYears();

            // Assert
            Assert.AreEqual(-1, index);
            Assert.AreEqual(2, builder.Dictionary[Sex2].Count, "Expect 2 data points");
        }

        [TestMethod]
        public void When_Non_Valid_Early_Data_Then_Remove_Index_Is_First_With_Data()
        {
            var entities = new List<INamedEntity>
            {
                new Sex {Id = Sex1},
                new Sex {Id = Sex2},
            };

            var builder = new PartitionTrendDataDictionaryBuilder(entities, PartitionDataType.Sex);

            AddNullData(builder);
            AddNullData(builder);
            AddValidData(builder);

            // Act
            var index = builder.RemoveEarlyEmptyYears();

            // Assert
            Assert.AreEqual(1, builder.Dictionary[Sex2].Count, "Expect 1 data point because two were removed");
            Assert.AreEqual(1, index);
        }

        private static PartitionTrendDataDictionaryBuilder GetPartitionTrendDataDictionaryBuilderWithDataAdded()
        {
            var entities = new List<INamedEntity>
            {
                new Sex {Id = Sex1},
                new Sex {Id = Sex2},
            };

            var builder = new PartitionTrendDataDictionaryBuilder(entities, PartitionDataType.Sex);

            AddValidData(builder);
            AddValidData(builder);

            return builder;
        }

        private static void AddValidData(PartitionTrendDataDictionaryBuilder builder)
        {
            builder.AddDataForNextTimePeriod(new List<CoreDataSet>
            {
                new CoreDataSet {SexId = Sex1,Value = 1},
                new CoreDataSet {SexId = Sex2,Value = 1},
            });
        }

        private static void AddNullData(PartitionTrendDataDictionaryBuilder builder)
        {
            builder.AddDataForNextTimePeriod(new List<CoreDataSet>
            {
                new CoreDataSet {SexId = SexIds.NotApplicable,Value = 1},
                new CoreDataSet {SexId = Sex2,Value = ValueData.NullValue},
            });
        }
    }
}
