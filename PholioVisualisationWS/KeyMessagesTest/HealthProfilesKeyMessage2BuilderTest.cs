
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.KeyMessages;

namespace KeyMessagesTest
{
    [TestClass]
    public class HealthProfilesKeyMessage2BuilderTest
    {
        private const string AreaName = "Derby";

        [TestMethod]
        public void TestSentence2_MaleSlope_True_FemaleSlope_True()
        {
            var builder = new HealthProfilesKeyMessage2Builder
            {
                data = new KeyMessageData 
                { 
                    AreaName = AreaName,
                    IsMaleSlopeIndexOfInequalitySignificant = true,
                    IsFemaleSlopeIndexOfInequalitySignificant = true,
                    MaleSlopeIndexOfInequalityForLifeExpectancy = "7.5",
                    FemaleSlopeIndexOfInequalityForLifeExpectancy = "4.0"
                }
            };

            var sentence = builder.GetSentence();
            Assert.AreEqual("Life expectancy is 7.5 years lower for men and 4.0 years lower for women in the most deprived areas of Derby than in the least deprived areas.", sentence);
        }

        [TestMethod]
        public void TestSentence2_MaleSlope_False_FemaleSlope_True()
        {
            var builder = new HealthProfilesKeyMessage2Builder()
            {
                data = new KeyMessageData
                {
                    AreaName = AreaName,
                    IsMaleSlopeIndexOfInequalitySignificant = false,
                    IsFemaleSlopeIndexOfInequalitySignificant = true,
                    MaleSlopeIndexOfInequalityForLifeExpectancy = "7.5",
                    FemaleSlopeIndexOfInequalityForLifeExpectancy = "4.0"
                }
            };

            var sentence = builder.GetSentence();
            Assert.AreEqual("Life expectancy is 4.0 years lower for women in the most deprived areas of Derby than in the least deprived areas.", sentence);
        }

        [TestMethod]
        public void TestSentence2_MaleSlope_True_FemaleSlope_False()
        {
            var builder = new HealthProfilesKeyMessage2Builder()
            {
                data = new KeyMessageData
                {
                    AreaName = AreaName,
                    IsMaleSlopeIndexOfInequalitySignificant = true,
                    IsFemaleSlopeIndexOfInequalitySignificant = false,
                    MaleSlopeIndexOfInequalityForLifeExpectancy = "7.5",
                    FemaleSlopeIndexOfInequalityForLifeExpectancy = "4.0"
                }
            };

            var sentence = builder.GetSentence();
            Assert.AreEqual("Life expectancy is 7.5 years lower for men in the most deprived areas of Derby than in the least deprived areas.", sentence);
        }

        [TestMethod]
        public void TestSentence2_MaleSlope_False_FemaleSlope_False()
        {
            var builder = new HealthProfilesKeyMessage2Builder()
            {
                data = new KeyMessageData
                {
                    AreaName = AreaName,
                    IsMaleSlopeIndexOfInequalitySignificant = false,
                    IsFemaleSlopeIndexOfInequalitySignificant = false,
                    MaleSlopeIndexOfInequalityForLifeExpectancy = "7.5",
                    FemaleSlopeIndexOfInequalityForLifeExpectancy = "4.0"
                }
            };

            var sentence = builder.GetSentence();
            Assert.AreEqual("Life expectancy is not significantly different for people in the most deprived areas of Derby than in the least deprived areas.", sentence);
        }

        [TestMethod]
        public void TestSentence2_Wrong_Value()
        {
            var builder = new HealthProfilesKeyMessage2Builder()
            {
                data = new KeyMessageData
                {
                    AreaName = AreaName,
                    IsMaleSlopeIndexOfInequalitySignificant = true,
                    IsFemaleSlopeIndexOfInequalitySignificant = true,
                    MaleSlopeIndexOfInequalityForLifeExpectancy = "4.0",
                    FemaleSlopeIndexOfInequalityForLifeExpectancy = "7.5"
                }
            };

            var sentence = builder.GetSentence();
            Assert.AreNotEqual("Life expectancy is 7.5 years lower for men and 4 years lower for women in the most deprived areas of Derby than in the least deprived areas.", sentence);
        }

        [TestMethod]
        public void TestSentence2_NullMaleCount()
        {
            var builder = new HealthProfilesKeyMessage2Builder
            {
                data = new KeyMessageData
                {
                    AreaName = AreaName,
                    IsMaleSlopeIndexOfInequalitySignificant = true,
                    IsFemaleSlopeIndexOfInequalitySignificant = true,
                    MaleSlopeIndexOfInequalityForLifeExpectancy = null,
                    FemaleSlopeIndexOfInequalityForLifeExpectancy = "1"
                }
            };

            Assert.AreEqual(string.Empty, builder.GetSentence());
        }

        [TestMethod]
        public void TestSentence2_NullFemaleCount()
        {
            var builder = new HealthProfilesKeyMessage2Builder()
            {
                data = new KeyMessageData
                {
                    AreaName = AreaName,
                    IsMaleSlopeIndexOfInequalitySignificant = true,
                    IsFemaleSlopeIndexOfInequalitySignificant = true,
                    MaleSlopeIndexOfInequalityForLifeExpectancy = "1",
                    FemaleSlopeIndexOfInequalityForLifeExpectancy = null
                }
            };

            Assert.AreEqual(string.Empty, builder.GetSentence());
        }
    }
}
