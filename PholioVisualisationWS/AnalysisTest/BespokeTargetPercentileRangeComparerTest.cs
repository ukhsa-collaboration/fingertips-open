using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class BespokeTargetPercentileRangeComparerTest
    {
        private TargetConfig _targetConfig;
        private BespokeTargetPercentileRangeComparer _comparer;
        private double _lowerTargetPercentileValue;
        private double _upperTargetPercentileValue;


        [TestInitialize]
        public void Init()
        {
            _targetConfig = new TargetConfig() { LowerLimit = 50, UpperLimit = 90, PolarityId = 1 };
            _lowerTargetPercentileValue = 9.3;
            _upperTargetPercentileValue = 37.2;
        }


        [TestMethod]
        public void CompareAgainstTarget_Returns_SignificanceWorse_When_Polarity_Is_One()
        {
            // Arrange
            _comparer = GetComparer(_targetConfig);
            
            // Act
            var result = _comparer.CompareAgainstTarget(new CoreDataSet { Value = 7.3});

            // Assert
            Assert.AreEqual(Significance.Worse, result);
        }

        [TestMethod]
        public void CompareAgainstTarget_Returns_SignificanceSame_When_Polarity_Is_One()
        {

            // Arrange
            _comparer = GetComparer(_targetConfig);

            // Act
            var result = _comparer.CompareAgainstTarget(new CoreDataSet { Value = 12});

            // Assert
            Assert.AreEqual(Significance.Same, result);
        }
        
        [TestMethod]
        public void CompareAgainstTarget_Returns_SignificanceBetter_When_Polarity_Is_One()
        {
            // Arrange
            _comparer = GetComparer(_targetConfig);

            // Act
            var result = _comparer.CompareAgainstTarget(new CoreDataSet { Value = 41.2 });

            // Assert
            Assert.AreEqual(Significance.Better, result);
        }


        [TestMethod]
        public void CompareAgainstTarget_Returns_SignificanceBetter_When_Polarity_Is_Zero()
        {
            // Arrange
            _targetConfig.PolarityId = 0;
            _comparer = GetComparer(_targetConfig);

            // Act
            var result = _comparer.CompareAgainstTarget(new CoreDataSet { Value = 7.3 });

            // Assert
            Assert.AreEqual(Significance.Better, result);
        }

        [TestMethod]
        public void CompareAgainstTarget_Returns_SignificanceWorse_When_Polarity_Is_Zero()
        {
            // Arrange
            _targetConfig.PolarityId = 0;
            _comparer = GetComparer(_targetConfig);

            // Act
            var result = _comparer.CompareAgainstTarget(new CoreDataSet { Value = 41.2 });

            // Assert
            Assert.AreEqual(Significance.Worse, result);
        }


        [TestMethod]
        public void CompareAgainstTarget_Returns_SignificanceNone_When_Polarity_Is_InValid()
        {
            // Arrange
            _targetConfig.PolarityId = -1;
            _comparer = GetComparer(_targetConfig);

            // Act
            var result = _comparer.CompareAgainstTarget(new CoreDataSet { Value = 41.2 });

            // Assert
            Assert.AreEqual(Significance.None, result);
        }

        [TestMethod]
        public void CompareAgainstTarget_Returns_SignificanceNone_When_Targets_AreNull()
        {
            // Arrange
            _comparer = GetComparer(_targetConfig);
            _comparer.LowerTargetPercentileBenchmarkData = null;
            _comparer.UpperTargetPercentileBenchmarkData = null;

            // Act
            var result = _comparer.CompareAgainstTarget(new CoreDataSet { Value = 41.2 });

            // Assert
            Assert.AreEqual(Significance.None, result);
        }


        private BespokeTargetPercentileRangeComparer GetComparer(TargetConfig targetConfig)
        {
            return new BespokeTargetPercentileRangeComparer(_targetConfig)
            {
                LowerTargetPercentileBenchmarkData = new CoreDataSet() { Value = _lowerTargetPercentileValue },
                UpperTargetPercentileBenchmarkData = new CoreDataSet() { Value = _upperTargetPercentileValue }
            };
        }

    }
}
