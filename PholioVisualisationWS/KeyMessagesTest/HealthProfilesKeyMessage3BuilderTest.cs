
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.KeyMessages;
using PholioVisualisation.PholioObjects;


namespace PholioVisualisation.KeyMessagesTest
{
    [TestClass]
    public class HealthProfilesKeyMessage3BuilderTest
    {

        #region Sentence-1 Tests

        [TestMethod]
        public void TestSentence1_Significance_Better()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    ObesityYear6Percentage = "19.2",
                    ObesityYear6Count = "12,999",
                    ObesityYear6Significance = Significance.Better
                }
            };

            var sentence = builder.GetSentence1();
            Assert.AreEqual(
                "In Year 6, 19.2% (12,999) of children are classified as obese, better than the average for England.",
                sentence);
        }

        [TestMethod]
        public void TestSentence1_Significance_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    ObesityYear6Percentage = "19.2",
                    ObesityYear6Count = "12,999",
                    ObesityYear6Significance = Significance.Worse
                }
            };

            var sentence = builder.GetSentence1();
            Assert.AreEqual(
                "In Year 6, 19.2% (12,999) of children are classified as obese, worse than the average for England.",
                sentence);
        }

        [TestMethod]
        public void TestSentence1_Significance_Same()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    ObesityYear6Percentage = "19.2",
                    ObesityYear6Count = "12,999",
                    ObesityYear6Significance = Significance.Same
                }
            };

            var sentence = builder.GetSentence1();
            Assert.AreEqual("In Year 6, 19.2% (12,999) of children are classified as obese.", sentence);
        }

        [TestMethod]
        public void TestSentence1_Significance_None()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    ObesityYear6Percentage = "19.2",
                    ObesityYear6Count = "12,999",
                    ObesityYear6Significance = Significance.None
                }
            };

            var sentence = builder.GetSentence1();
            Assert.AreEqual("In Year 6, 19.2% (12,999) of children are classified as obese.", sentence);
        }

        [TestMethod]
        public void TestSentence1_Wrong_Data()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    ObesityYear6Percentage = "30.2",
                    ObesityYear6Count = "12,000",
                    ObesityYear6Significance = Significance.Worse
                }
            };

            var sentence = builder.GetSentence1();
            Assert.AreNotEqual(
                "In Year 6, 19.2% (12,999) of children are classified as obese, worse than the average for England.",
                sentence);
        }

        #endregion

        #region Sentence-2 Tests

        [TestMethod]
        public void TestSentence2_NullValue()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    Under18AlcoholSpecificHospitalStays = null,
                    Under18AlcoholSpecificSignificance = Significance.None,
                    Under18AlcoholSpecificHospitalStaysPerYear = null
                }
            };

            var sentence = builder.GetSentence2();
            Assert.AreEqual(string.Empty, sentence);
        }

        [TestMethod]
        public void TestSentence2_Significance_Better()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    Under18AlcoholSpecificHospitalStays = 20.0,
                    Under18AlcoholSpecificSignificance = Significance.Better,
                    Under18AlcoholSpecificHospitalStaysPerYear = "10,001"
                }
            };

            var sentence = builder.GetSentence2();
            Assert.AreEqual(
                "The rate of alcohol-specific hospital stays among those under 18 is 20*, better than the average for England. This represents 10,001 stays per year.",
                sentence);
        }

        [TestMethod]
        public void TestSentence2_Significance_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    Under18AlcoholSpecificHospitalStays = 20.0,
                    Under18AlcoholSpecificSignificance = Significance.Worse,
                    Under18AlcoholSpecificHospitalStaysPerYear = "10,001"
                }
            };

            var sentence = builder.GetSentence2();
            Assert.AreEqual(
                "The rate of alcohol-specific hospital stays among those under 18 is 20*, worse than the average for England. This represents 10,001 stays per year.",
                sentence);
        }

        [TestMethod]
        public void TestSentence2_Significance_None()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    Under18AlcoholSpecificHospitalStays = 20.0,
                    Under18AlcoholSpecificSignificance = Significance.None,
                    Under18AlcoholSpecificHospitalStaysPerYear = "10,001"
                }
            };

            var sentence = builder.GetSentence2();
            Assert.AreEqual(
                "The rate of alcohol-specific hospital stays among those under 18 is 20*. This represents 10,001 stays per year.",
                sentence);
        }

        [TestMethod]
        public void TestSentence2_Significance_Same()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    Under18AlcoholSpecificHospitalStays = 20.0,
                    Under18AlcoholSpecificSignificance = Significance.Same,
                    Under18AlcoholSpecificHospitalStaysPerYear = "10,001"
                }
            };

            var sentence = builder.GetSentence2();
            Assert.AreEqual(
                "The rate of alcohol-specific hospital stays among those under 18 is 20*. This represents 10,001 stays per year.",
                sentence);
        }

        [TestMethod]
        public void TestSentence2_Wrong_Data()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    Under18AlcoholSpecificHospitalStays = 89.0,
                    Under18AlcoholSpecificSignificance = Significance.Same,
                    Under18AlcoholSpecificHospitalStaysPerYear = "50,101"
                }
            };

            var sentence = builder.GetSentence2();
            Assert.AreNotEqual(
                "The rate of alcohol-specific hospital stays among those under 18 was 20.0*. This represents 10,001 stays per year.",
                sentence);
        }

        #endregion

        #region Sentence-3 Tests

        [TestMethod]
        public void TestSentence3_All_Better()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Better,
                    GcseAchievementSig = Significance.Better,
                    BreastfeedingInitiationSig = Significance.Better,
                    SmokingAtTimeOfDeliverySig = Significance.Better
                }
            };

            var sentence = builder.GetSentence3();
            Assert.AreEqual(string.Empty, sentence);
        }

        [TestMethod]
        public void TestSentence3_All_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Worse,
                    GcseAchievementSig = Significance.Worse,
                    BreastfeedingInitiationSig = Significance.Worse,
                    SmokingAtTimeOfDeliverySig = Significance.Worse
                }
            };

            var sentence = builder.GetSentence3();
            Assert.AreEqual(
                "Levels of teenage pregnancy, GCSE attainment, breastfeeding initiation and smoking at time of delivery are worse than the England average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence3_TeenagePregnancySig_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Worse,
                    GcseAchievementSig = Significance.Better,
                    BreastfeedingInitiationSig = Significance.Better,
                    SmokingAtTimeOfDeliverySig = Significance.Better
                }
            };

            var sentence = builder.GetSentence3();
            Assert.AreEqual("Levels of teenage pregnancy are worse than the England average.", sentence);
        }

        [TestMethod]
        public void TestSentence3_GCSEAchievementSig_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Better,
                    GcseAchievementSig = Significance.Worse,
                    BreastfeedingInitiationSig = Significance.Better,
                    SmokingAtTimeOfDeliverySig = Significance.Better
                }
            };

            var sentence = builder.GetSentence3();
            Assert.AreEqual("Levels of GCSE attainment are worse than the England average.", sentence);
        }

        [TestMethod]
        public void TestSentence3_BreastfeedingInitiationSig_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Better,
                    GcseAchievementSig = Significance.Better,
                    BreastfeedingInitiationSig = Significance.Worse,
                    SmokingAtTimeOfDeliverySig = Significance.Better
                }
            };

            var sentence = builder.GetSentence3();
            Assert.AreEqual("Levels of breastfeeding initiation are worse than the England average.", sentence);
        }

        [TestMethod]
        public void TestSentence3_SmokingAtTimeOfDeliverySig_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Better,
                    GcseAchievementSig = Significance.Better,
                    BreastfeedingInitiationSig = Significance.Better,
                    SmokingAtTimeOfDeliverySig = Significance.Worse
                }
            };

            var sentence = builder.GetSentence3();
            Assert.AreEqual("Levels of smoking at time of delivery are worse than the England average.", sentence);
        }

        [TestMethod]
        public void TestSentence3_TeenagePregnancySig_SmokingAtTimeOfDeliverySig_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Worse,
                    GcseAchievementSig = Significance.Better,
                    BreastfeedingInitiationSig = Significance.Better,
                    SmokingAtTimeOfDeliverySig = Significance.Worse
                }
            };

            var sentence = builder.GetSentence3();
            Assert.AreEqual(
                "Levels of teenage pregnancy and smoking at time of delivery are worse than the England average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence3_TeenagePregnancySig_BreastfeedingInitiationSig_SmokingAtTimeOfDeliverySig_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Worse,
                    GcseAchievementSig = Significance.Better,
                    BreastfeedingInitiationSig = Significance.Worse,
                    SmokingAtTimeOfDeliverySig = Significance.Worse
                }
            };

            var sentence = builder.GetSentence3();
            Assert.AreEqual(
                "Levels of teenage pregnancy, breastfeeding initiation and smoking at time of delivery are worse than the England average.",
                sentence);
        }

        #endregion

        #region Sentence-4 Tests

        [TestMethod]
        public void TestSentence4_All_Better()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Better,
                    GcseAchievementSig = Significance.Better,
                    BreastfeedingInitiationSig = Significance.Better,
                    SmokingAtTimeOfDeliverySig = Significance.Better
                }
            };

            var sentence = builder.GetSentence4();
            Assert.AreEqual("Levels of teenage pregnancy, GCSE attainment, breastfeeding initiation and smoking at time of delivery are better than the England average.", sentence);
        }

        [TestMethod]
        public void TestSentence4_All_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Worse,
                    GcseAchievementSig = Significance.Worse,
                    BreastfeedingInitiationSig = Significance.Worse,
                    SmokingAtTimeOfDeliverySig = Significance.Worse
                }
            };

            var sentence = builder.GetSentence4();
            Assert.AreEqual(string.Empty, sentence);
        }

        [TestMethod]
        public void TestSentence4_TeenagePregnancySig_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Worse,
                    GcseAchievementSig = Significance.Better,
                    BreastfeedingInitiationSig = Significance.Better,
                    SmokingAtTimeOfDeliverySig = Significance.Better
                }
            };

            var sentence = builder.GetSentence4();
            Assert.AreEqual("Levels of GCSE attainment, breastfeeding initiation and smoking at time of delivery are better than the England average.", sentence);
        }

        [TestMethod]
        public void TestSentence4_GCSEAchievementSig_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Better,
                    GcseAchievementSig = Significance.Worse,
                    BreastfeedingInitiationSig = Significance.Better,
                    SmokingAtTimeOfDeliverySig = Significance.Better
                }
            };

            var sentence = builder.GetSentence4();
            Assert.AreEqual("Levels of teenage pregnancy, breastfeeding initiation and smoking at time of delivery are better than the England average.", sentence);
        }

        [TestMethod]
        public void TestSentence4_BreastfeedingInitiationSig_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Better,
                    GcseAchievementSig = Significance.Better,
                    BreastfeedingInitiationSig = Significance.Worse,
                    SmokingAtTimeOfDeliverySig = Significance.Better
                }
            };

            var sentence = builder.GetSentence4();
            Assert.AreEqual("Levels of teenage pregnancy, GCSE attainment and smoking at time of delivery are better than the England average.", sentence);
        }

        [TestMethod]
        public void TestSentence4_SmokingAtTimeOfDeliverySig_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Better,
                    GcseAchievementSig = Significance.Better,
                    BreastfeedingInitiationSig = Significance.Better,
                    SmokingAtTimeOfDeliverySig = Significance.Worse
                }
            };

            var sentence = builder.GetSentence4();
            Assert.AreEqual("Levels of teenage pregnancy, GCSE attainment and breastfeeding initiation are better than the England average.", sentence);
        }

        [TestMethod]
        public void TestSentence4_TeenagePregnancySig_SmokingAtTimeOfDeliverySig_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Worse,
                    GcseAchievementSig = Significance.Better,
                    BreastfeedingInitiationSig = Significance.Better,
                    SmokingAtTimeOfDeliverySig = Significance.Worse
                }
            };

            var sentence = builder.GetSentence4();
            Assert.AreEqual(
                "Levels of GCSE attainment and breastfeeding initiation are better than the England average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence4_TeenagePregnancySig_BreastfeedingInitiationSig_SmokingAtTimeOfDeliverySig_Worse()
        {
            var builder = new HealthProfilesKeyMessage3Builder
            {
                data = new KeyMessageData
                {
                    TeenagePregnancySig = Significance.Worse,
                    GcseAchievementSig = Significance.Better,
                    BreastfeedingInitiationSig = Significance.Worse,
                    SmokingAtTimeOfDeliverySig = Significance.Worse
                }
            };

            var sentence = builder.GetSentence4();
            Assert.AreEqual(
                "Levels of GCSE attainment are better than the England average.",
                sentence);
        }

        #endregion

    }
}
