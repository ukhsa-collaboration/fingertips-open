using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.KeyMessages;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessagesTest
{
    [TestClass]
    public class HealthProfilesKeyMessage1BuilderTest
    {

        #region Setup

        private KeyMessageData Sentence1Data(double green, double red, double amber)
        {
            var mockSig = new Mock<SignificanceCounter>();
            mockSig.SetupProperty(x => x.ProportionGreen, green);
            mockSig.SetupProperty(x => x.ProportionRed, red);
            mockSig.SetupProperty(x => x.ProportionAmber, amber);

            return new KeyMessageData
            {
                AreaName = "Derby",
                SpineChartSignificances = mockSig.Object
            };
        }

        private KeyMessageData Sentence2Data(Significance deprivationSig)
        {
            return new KeyMessageData
            {
                ChildrenInPoverty = new CoreDataSet { Value = 23.7777, Count = 10000, YearRange = 3 },
                DeprivationSig = deprivationSig
            };
        }

        private KeyMessageData Sentence3Data(Significance maleLifeExpectancyAtBirth, Significance femaleLifeExpectancyAtBirth)
        {
            return new KeyMessageData()
            {
                MaleLifeExpectancyAtBirth = maleLifeExpectancyAtBirth,
                FemaleLifeExpectancyAtBirth = femaleLifeExpectancyAtBirth
            };
        }

        #endregion


        #region Sentence-1 Tests
        [TestMethod]
        public void TestSentence1_If_Statement_Block_1_1()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence1Data(.14, .6, 0)
            };

            var sentence1 = builder.GetSentence1();
            Assert.AreEqual("The health of people in Derby is generally worse than the England average.", sentence1);
        }

        [TestMethod]
        public void TestSentence1_If_Statement_Block_1_2()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence1Data(.0, .4, 0)
            };

            var sentence1 = builder.GetSentence1();
            Assert.AreEqual("The health of people in Derby is generally worse than the England average.", sentence1);
        }


        [TestMethod]
        public void TestSentence1_If_Statement_Block_1_3()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence1Data(.15, 4, 0)
            };

            var sentence1 = builder.GetSentence1();
            Assert.AreEqual("The health of people in Derby is generally worse than the England average.", sentence1);
        }


        [TestMethod]
        public void TestSentence1_If_Statement_Block_1_Worng_Values_1()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence1Data(.0, .2, 5)
            };

            var sentence1 = builder.GetSentence1();
            Assert.AreNotEqual("The health of people in Derby is generally worse than the England average.", sentence1);
        }



        [TestMethod]
        public void TestSentence1_If_Statement_Block_1_Worng_Values_2()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence1Data(.2, .4, 5)
            };

            var sentence1 = builder.GetSentence1();
            Assert.AreNotEqual("The health of people in Derby is generally worse than the England average.", sentence1);
        }


        [TestMethod]
        public void TestSentence1_If_Statement_Block_2_1()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence1Data(.5, .11, 0)
            };

            var sentence1 = builder.GetSentence1();
            Assert.AreEqual("The health of people in Derby is generally better than the England average.", sentence1);
        }


        [TestMethod]
        public void TestSentence1_If_Statement_Block_2_2()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence1Data(.4, .0, 0)
            };

            var sentence1 = builder.GetSentence1();
            Assert.AreEqual("The health of people in Derby is generally better than the England average.", sentence1);
        }

        [TestMethod]
        public void TestSentence1_If_Statement_Block_2_3()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence1Data(1, .15, 0)
            };

            var sentence1 = builder.GetSentence1();
            Assert.AreEqual("The health of people in Derby is generally better than the England average.", sentence1);
        }

        [TestMethod]
        public void TestSentence1_If_Statement_Block_2_Worng_Values_1()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence1Data(.3, .15, 0)
            };

            var sentence1 = builder.GetSentence1();
            Assert.AreNotEqual("The health of people in Derby is generally better than the England average.", sentence1);
        }

        [TestMethod]
        public void TestSentence1_If_Statement_Block_3()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence1Data(0, 0, 1)
            };

            var sentence1 = builder.GetSentence1();
            Assert.AreEqual("The health of people in Derby is generally similar to the England average.", sentence1);
        }

        [TestMethod]
        public void TestSentence1_If_Statement_Block_4()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence1Data(0, 0, 0)
            };

            var sentence1 = builder.GetSentence1();
            Assert.AreEqual("The health of people in Derby is varied compared with the England average.", sentence1);
        }

        #endregion


        #region Sentence-2 Tests

        [TestMethod]
        public void TestSentence2Complete()
        {
            var builder = new HealthProfilesKeyMessage1Builder
            {
                data = Sentence2Data(Significance.Better)
            };

            var sentence2 = builder.GetSentence2();
            Assert.AreEqual("Deprivation is lower than average, however about 23.8% (3,300) children live in poverty.",
                sentence2);
        }

        [TestMethod]
        public void TestSentence2WorseSig()
        {
            var builder = new HealthProfilesKeyMessage1Builder
            {
                data = Sentence2Data(Significance.Worse)
            };

            var sentence2 = builder.GetSentence2();
            Assert.AreEqual("Deprivation is higher than average and about 23.8% (3,300) children live in poverty.", sentence2);
        }

        [TestMethod]
        public void TestSentence2SameSig()
        {
            var builder = new HealthProfilesKeyMessage1Builder
            {
                data = Sentence2Data(Significance.Same)
            };

            var sentence2 = builder.GetSentence2();
            Assert.AreEqual("About 23.8% (3,300) children live in poverty.", sentence2);
        }

        [TestMethod]
        public void TestSentence2NoneSig()
        {
            var builder = new HealthProfilesKeyMessage1Builder
            {
                data = Sentence2Data(Significance.None)
            };

            var sentence2 = builder.GetSentence2();
            Assert.AreEqual("", sentence2);
        }

        [TestMethod]
        public void TestSentence2DataIsNull()
        {
            var builder = new HealthProfilesKeyMessage1Builder
            {
                data = new KeyMessageData
                {
                    ChildrenInPoverty = null,
                    DeprivationSig = Significance.Better
                }
            };

            var sentence2 = builder.GetSentence2();
            Assert.AreEqual("", sentence2);
        }

        [TestMethod]
        public void TestSentence2DataIsInvalid()
        {
            var builder = new HealthProfilesKeyMessage1Builder
            {
                data = new KeyMessageData
                {
                    ChildrenInPoverty = new CoreDataSet { Value = ValueData.NullValue },
                    DeprivationSig = Significance.Better
                }
            };

            var sentence2 = builder.GetSentence2();
            Assert.AreEqual("", sentence2);
        }

        #endregion


        #region Sentence-3 Tests

        [TestMethod]
        public void TestSentence3BetterBetter()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence3Data(Significance.Better, Significance.Better)
            };

            var sentence3 = builder.GetSentence3();
            Assert.AreEqual("Life expectancy for both men and women is higher than the England average.", sentence3);
        }

        [TestMethod]
        public void TestSentence3WorseWorse()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence3Data(Significance.Worse, Significance.Worse)
            };

            var sentence3 = builder.GetSentence3();
            Assert.AreEqual("Life expectancy for both men and women is lower than the England average.", sentence3);
        }

        [TestMethod]
        public void TestSentence3SameSame()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence3Data(Significance.Same, Significance.Same)
            };

            var sentence3 = builder.GetSentence3();
            Assert.AreEqual("Life expectancy for both men and women is similar to the England average.", sentence3);
        }

        [TestMethod]
        public void TestSentence3WorseBetter()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence3Data(Significance.Worse, Significance.Better)
            };

            var sentence3 = builder.GetSentence3();
            Assert.AreEqual("Life expectancy for men is lower and for women higher than the England average.", sentence3);
        }

        [TestMethod]
        public void TestSentence3BetterWorse()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence3Data(Significance.Better, Significance.Worse)
            };

            var sentence3 = builder.GetSentence3();
            Assert.AreEqual("Life expectancy for men is higher and for women lower than the England average.", sentence3);
        }

        [TestMethod]
        public void TestSentence3WorseSame()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence3Data(Significance.Worse, Significance.Same)
            };

            var sentence3 = builder.GetSentence3();
            Assert.AreEqual("Life expectancy for men is lower than the England average.", sentence3);
        }

        [TestMethod]
        public void TestSentence3SameWorse()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence3Data(Significance.Same, Significance.Worse)
            };

            var sentence3 = builder.GetSentence3();
            Assert.AreEqual("Life expectancy for women is lower than the England average.", sentence3);
        }

        [TestMethod]
        public void TestSentence3BetterSame()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence3Data(Significance.Better, Significance.Same)
            };

            var sentence3 = builder.GetSentence3();
            Assert.AreEqual("Life expectancy for men is higher than the England average.", sentence3);
        }

        [TestMethod]
        public void TestSentence3SameBetter()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence3Data(Significance.Same, Significance.Better)
            };

            var sentence3 = builder.GetSentence3();
            Assert.AreEqual("Life expectancy for women is higher than the England average.", sentence3);
        }

        [TestMethod]
        public void TestSentence3NoneNone()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence3Data(Significance.None, Significance.None)
            };

            var sentence3 = builder.GetSentence3();
            Assert.AreEqual("", sentence3);
        }

        [TestMethod]
        public void TestSentence3BetterNone()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence3Data(Significance.Better, Significance.None)
            };

            var sentence3 = builder.GetSentence3();
            Assert.AreEqual("", sentence3);
        }

        [TestMethod]
        public void TestSentence3NoneBetter()
        {
            var builder = new HealthProfilesKeyMessage1Builder()
            {
                data = Sentence3Data(Significance.None, Significance.Better)
            };

            var sentence3 = builder.GetSentence3();
            Assert.AreEqual("", sentence3);
        }

        #endregion

    }
}
