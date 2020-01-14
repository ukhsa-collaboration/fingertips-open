
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class GroupRootComparisonManagerTest
    {
        [TestMethod]
        public void TestTargetComparisonsAreMadeForDataIfTargetIsDefined()
        {
            IndicatorMetadata metadata = new IndicatorMetadata();

            GroupRoot root = new GroupRoot();
            AddRegionalData(root);
            AddNationalData(root);
            root.Data.Add(new CoreDataSet { Value = 2 });

            var comparisonManager = GetComparisonManager();
            comparisonManager.TargetComparer = new SingleValueTargetComparer(new TargetConfig
            {
                LowerLimit = 1,
                PolarityId = PolarityIds.RagHighIsGood
            });
            comparisonManager.CompareToCalculateSignificance(root, metadata);

            // Local data is compared against target
            Assert.AreEqual(Significance.Better,
                (Significance)root.Data[0].Significance[ComparatorIds.Target]);

            // Regional and national data is compared against target
            foreach (var grouping in root.Grouping)
            {
                Assert.AreEqual(Significance.Better,
                    (Significance)grouping.ComparatorData.Significance[ComparatorIds.Target]);
            }
        }

        [TestMethod]
        public void TestNoDataDoesNotCauseException()
        {
            IndicatorMetadata metadata = new IndicatorMetadata();
            GroupRoot root = new GroupRoot();
            GetComparisonManager().CompareToCalculateSignificance(root, metadata);
            Assert.AreEqual(0, root.Data.Count);
            Assert.AreEqual(0, root.Grouping.Count);
        }

        [TestMethod]
        public void TestRegionalIsComparedToNational()
        {
            GroupRoot root = new GroupRoot();

            AddRegionalData(root);
            AddNationalData(root);

            GetComparisonManager().CompareToCalculateSignificance(root, GetMetadata());
            Assert.AreEqual((int)Significance.Better,
                root.Grouping[0].ComparatorData.Significance[ComparatorIds.England]);
        }


        [TestMethod]
        public void TestRegionalIsNotComparedToNationalForQuartiles()
        {
            NoRegionalToNationalComparisonMadeForSpecificBenchmarkingMethod(ComparatorMethodIds.Quartiles);
        }

        [TestMethod]
        public void TestRegionalIsNotComparedToNationalForQuintiles()
        {
            NoRegionalToNationalComparisonMadeForSpecificBenchmarkingMethod(ComparatorMethodIds.Quintiles);
        }

        [TestMethod]
        public void TestMinusOneComparatorsAreIgnored()
        {
            GroupRoot root = new GroupRoot();

            AddRegionalData(root);

            root.Grouping[0].ComparatorId = -1;
            root.Data.Add(new CoreDataSet { Value = 10, LowerCI95 = 9, UpperCI95 = 11 });

            GetComparisonManager().CompareToCalculateSignificance(root, GetMetadata());
            Assert.AreEqual(0, root.Data[0].Significance.Count);
        }

        [TestMethod]
        public void TestNoExceptionForRegionalComparisonIfNoNationalGrouping()
        {
            GroupRoot root = new GroupRoot();
            AddRegionalData(root);
            GetComparisonManager().CompareToCalculateSignificance(root, GetMetadata());
        }

        [TestMethod]
        public void TestNoExceptionForRegionalComparisonIfNoRegionalGrouping()
        {
            GroupRoot root = new GroupRoot();
            AddNationalData(root);
            GetComparisonManager().CompareToCalculateSignificance(root, GetMetadata());
        }

        [TestMethod]
        public void TestNoExceptionForRegionalComparisonIfNoNationalData()
        {
            GroupRoot root = new GroupRoot();
            AddRegionalData(root);
            AddNationalData(root);
            root.Grouping[1].ComparatorData = null;
            GetComparisonManager().CompareToCalculateSignificance(root, GetMetadata());
        }

        [TestMethod]
        public void TestNoExceptionForRegionalComparisonIfNoRegionalData()
        {
            GroupRoot root = new GroupRoot();
            AddRegionalData(root);
            AddNationalData(root);
            root.Grouping[0].ComparatorData = null;
            GetComparisonManager().CompareToCalculateSignificance(root, GetMetadata());
        }

        private static void AddNationalData(GroupRoot root)
        {
            root.Grouping.Add(new Grouping
            {
                ComparatorId = ComparatorIds.England,
                ComparatorMethodId = 1,
                ComparatorConfidence = 95,
                ComparatorData = new CoreDataSet { Value = 20, LowerCI95 = 15, UpperCI95 = 25 },
                PolarityId = PolarityIds.RagHighIsGood
            });
        }

        private void NoRegionalToNationalComparisonMadeForSpecificBenchmarkingMethod(
            int benchmarkingMethodId)
        {
            // Arrange
            GroupRoot root = new GroupRoot();
            AddRegionalData(root);
            AddNationalData(root);
            foreach (var grouping in root.Grouping)
            {
                grouping.ComparatorMethodId = benchmarkingMethodId;
            }

            // Act
            GetComparisonManager().CompareToCalculateSignificance(root, GetMetadata());

            // Assert: no comparison made 
            Assert.AreEqual(0, root.Grouping[0].ComparatorData.Significance.Count);
        }

        private static void AddRegionalData(GroupRoot root)
        {
            root.Grouping.Add(new Grouping
            {
                ComparatorId = ComparatorIds.Subnational,
                ComparatorMethodId = 1,
                ComparatorConfidence = 95,
                ComparatorData = new CoreDataSet { Value = 50, LowerCI95 = 45, UpperCI95 = 55 },
                PolarityId = PolarityIds.RagHighIsGood
            });
        }

        private IndicatorMetadata GetMetadata()
        {
            return new IndicatorMetadata { ValueTypeId = 5, Unit = new Unit { Value = 100 } };
        }

        private GroupRootComparisonManager GetComparisonManager()
        {
            var reader = new Moq.Mock<PholioReader>().Object;
            return new GroupRootComparisonManager { PholioReader = reader };
        }
    }
}
