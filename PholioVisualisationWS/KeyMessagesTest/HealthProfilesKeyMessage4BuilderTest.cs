using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.KeyMessages;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessagesTest
{
    [TestClass]
    public class HealthProfilesKeyMessage4BuilderTest
    {
        #region Sentence-2 Tests

        [TestMethod]
        public void TestSentence2_Sig_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultAlcoholAdmissions = 17.5,
                    AdultAlcoholAdmissionsPerYear = 30456,
                    AdultAlcoholAdmissionsSignificance = Significance.Better,                    
                }
            };
            var sentence = builder.GetSentence2();
            Assert.AreEqual("The rate of alcohol-related harm hospital stays is 18*, better than the average for England. This represents 30,456 stays per year.", sentence);
        }

        [TestMethod]
        public void TestSentence2_Sig_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultAlcoholAdmissions = 17.5,
                    AdultAlcoholAdmissionsPerYear = 30456,
                    AdultAlcoholAdmissionsSignificance = Significance.Worse,
                }
            };
            var sentence = builder.GetSentence2();
            Assert.AreEqual("The rate of alcohol-related harm hospital stays is 18*, worse than the average for England. This represents 30,456 stays per year.", sentence);
        }

        [TestMethod]
        public void TestSentence2_Sig_None()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultAlcoholAdmissions = 17.5,
                    AdultAlcoholAdmissionsPerYear = 30456,
                    AdultAlcoholAdmissionsSignificance = Significance.None,
                }
            };
            var sentence = builder.GetSentence2();
            Assert.AreEqual("The rate of alcohol-related harm hospital stays is 18*. This represents 30,456 stays per year.", sentence);
        }

        [TestMethod]
        public void TestSentence2_Sig_Same()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultAlcoholAdmissions = 17.5,
                    AdultAlcoholAdmissionsPerYear = 30456,
                    AdultAlcoholAdmissionsSignificance = Significance.Same,
                }
            };
            var sentence = builder.GetSentence2();
            Assert.AreEqual("The rate of alcohol-related harm hospital stays is 18*. This represents 30,456 stays per year.", sentence);
        }

        [TestMethod]
        public void TestSentence2_Wrong_Data()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultAlcoholAdmissions = 17.5,
                    AdultAlcoholAdmissionsPerYear = 10456,
                    AdultAlcoholAdmissionsSignificance = Significance.Better,
                }
            };
            var sentence = builder.GetSentence2();
            Assert.AreNotEqual("The rate of alcohol related harm hospital stays is 18*. This represents 30,456 stays per year.", sentence);
        }

        [TestMethod]
        public void TestSentence2_EmptyStringWhenNoData()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData()
            };
            Assert.AreEqual("", builder.GetSentence2());
        }
        #endregion

        #region Sentence-3 Tests

        [TestMethod]
        public void TestSentence3_Sig_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultSelfHarmAdmissions = "30.0",
                    AdultSelfHarmAdmissionsPerYear = "44,567",
                    AdultSelfHarmAdmissionsSignificance = Significance.Better
                }
            };
            var sentence = builder.GetSentence3();
            Assert.AreEqual("The rate of self-harm hospital stays is 30.0*, better than the average for England. This represents 44,567 stays per year.", sentence);
        }

        [TestMethod]
        public void TestSentence3_Sig_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultSelfHarmAdmissions = "30.0",
                    AdultSelfHarmAdmissionsPerYear = "44,567",
                    AdultSelfHarmAdmissionsSignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence3();
            Assert.AreEqual("The rate of self-harm hospital stays is 30.0*, worse than the average for England. This represents 44,567 stays per year.", sentence);
        }

        [TestMethod]
        public void TestSentence3_Sig_None()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultSelfHarmAdmissions = "30.0",
                    AdultSelfHarmAdmissionsPerYear = "44,567",
                    AdultSelfHarmAdmissionsSignificance = Significance.None
                }
            };
            var sentence = builder.GetSentence3();
            Assert.AreEqual("The rate of self-harm hospital stays is 30.0*. This represents 44,567 stays per year.", sentence);
        }

        [TestMethod]
        public void TestSentence3_Sig_Same()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultSelfHarmAdmissions = "30.0",
                    AdultSelfHarmAdmissionsPerYear = "44,567",
                    AdultSelfHarmAdmissionsSignificance = Significance.Same
                }
            };
            var sentence = builder.GetSentence3();
            Assert.AreEqual("The rate of self-harm hospital stays is 30.0*. This represents 44,567 stays per year.", sentence);
        }

        [TestMethod]
        public void TestSentence3_Wrong_Data()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultSelfHarmAdmissions = "30.0",
                    AdultSelfHarmAdmissionsPerYear = "55,567",
                    AdultSelfHarmAdmissionsSignificance = Significance.Same
                }
            };
            var sentence = builder.GetSentence3();
            Assert.AreNotEqual("The rate of self-harm hospital stays is 30.0*. This represents 44,567 stays per year.", sentence);
        }

        #endregion

        #region Sentence-4 Tests

        [TestMethod]
        public void TestSentence4_Sig_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultSmokingRelatedDeaths = 50.1,
                    AdultSmokingRelatedDeathsPerYear = 19000.2,
                    AdultSmokingRelatedDeathsSignificance = Significance.Better,
                }
            };
            var sentence = builder.GetSentence4();
            Assert.AreEqual(
                "The rate of smoking related deaths is 50*, better than the average for England. This represents 19,000 deaths per year.",
                sentence);
        }

        [TestMethod]
        public void TestSentence4_Sig_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultSmokingRelatedDeaths = 50.1,
                    AdultSmokingRelatedDeathsPerYear = 19000.2,
                    AdultSmokingRelatedDeathsSignificance = Significance.Worse,
                }
            };
            var sentence = builder.GetSentence4();
            Assert.AreEqual(
                "The rate of smoking related deaths is 50*, worse than the average for England. This represents 19,000 deaths per year.",
                sentence);
        }

        [TestMethod]
        public void TestSentence4_Sig_None()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultSmokingRelatedDeaths = 50.1,
                    AdultSmokingRelatedDeathsPerYear = 19000.2,
                    AdultSmokingRelatedDeathsSignificance = Significance.None,
                }
            };
            var sentence = builder.GetSentence4();
            Assert.AreEqual(
                "The rate of smoking related deaths is 50*. This represents 19,000 deaths per year.",
                sentence);
        }

        [TestMethod]
        public void TestSentence4_Sig_Same()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultSmokingRelatedDeaths = 50.1,
                    AdultSmokingRelatedDeathsPerYear = 19000.2,
                    AdultSmokingRelatedDeathsSignificance = Significance.Same,
                }
            };
            var sentence = builder.GetSentence4();
            Assert.AreEqual(
                "The rate of smoking related deaths is 50*. This represents 19,000 deaths per year.",
                sentence);
        }

        [TestMethod]
        public void TestSentence4_Wrong_Data()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultSmokingRelatedDeaths = 90,
                    AdultSmokingRelatedDeathsPerYear = 19050,
                    AdultSmokingRelatedDeathsSignificance = Significance.Same,
                }
            };
            var sentence = builder.GetSentence4();
            Assert.AreNotEqual(
                "The rate of smoking related deaths is 50*. This represents 19,000 deaths per year.",
                sentence);
        }

        #endregion

        #region Sentence-5A Tests

        [TestMethod]
        public void TestSentence5A_ExcessWeightSig_Worse_SmokingPrevalenceSig_Worse_PhysicalActivitySig_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultExcessWeightSignificance = Significance.Worse,
                    AdultSmokingPrevalenceSignificance = Significance.Worse,
                    AdultPhysicalActivitySignificance = Significance.Worse                 
                }
            };
            var sentence = builder.GetSentence5(Significance.Worse);
            Assert.AreEqual(
                "Estimated levels of adult excess weight, smoking and physical activity are worse than the England average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence5A_ExcessWeightSig_Worse_SmokingPrevalenceSig_Better_PhysicalActivitySig_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultExcessWeightSignificance = Significance.Worse,
                    AdultSmokingPrevalenceSignificance = Significance.Better,
                    AdultPhysicalActivitySignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence5(Significance.Worse);
            Assert.AreEqual(
                "Estimated levels of adult excess weight and physical activity are worse than the England average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence5A_ExcessWeightSig_Worse_SmokingPrevalenceSig_Worse_PhysicalActivitySig_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultExcessWeightSignificance = Significance.Worse,                                        
                    AdultSmokingPrevalenceSignificance = Significance.Worse,
                    AdultPhysicalActivitySignificance = Significance.Better
                }
            };
            var sentence = builder.GetSentence5(Significance.Worse);
            Assert.AreEqual(
                "Estimated levels of adult excess weight and smoking are worse than the England average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence5A_ExcessWeightSig_Better_SmokingPrevalenceSig_Worse_PhysicalActivitySig_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultExcessWeightSignificance = Significance.Better,
                    AdultSmokingPrevalenceSignificance = Significance.Worse,
                    AdultPhysicalActivitySignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence5(Significance.Worse);
            Assert.AreEqual(
                "Estimated levels of adult smoking and physical activity are worse than the England average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence5A_All_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultExcessWeightSignificance = Significance.Better,
                    AdultSmokingPrevalenceSignificance = Significance.Better,
                    AdultPhysicalActivitySignificance = Significance.Better
                }
            };
            var sentence = builder.GetSentence5(Significance.Worse);
            Assert.AreEqual(string.Empty, sentence);
        }

        [TestMethod]
        public void TestSentence5A_Wrong_Data()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultExcessWeightSignificance = Significance.Same,
                    AdultSmokingPrevalenceSignificance = Significance.Same,
                    AdultPhysicalActivitySignificance = Significance.None
                }
            };
            var sentence = builder.GetSentence5(Significance.Worse);
            Assert.AreNotEqual(
                "Estimated levels of adult excess weight, smoking and physical activity are worse than the England average.",
                sentence);
        }

        #endregion

        #region Sentence-5B Tests

        [TestMethod]
        public void TestSentence5B_ExcessWeightSig_Better_SmokingPrevalenceSig_Better_PhysicalActivitySig_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultExcessWeightSignificance = Significance.Better,
                    AdultSmokingPrevalenceSignificance = Significance.Better,
                    AdultPhysicalActivitySignificance = Significance.Better
                }
            };
            var sentence = builder.GetSentence5(Significance.Better);
            Assert.AreEqual(
                "Estimated levels of adult excess weight, smoking and physical activity are better than the England average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence5B_ExcessWeightSig_Beter_SmokingPrevalenceSig_Worse_PhysicalActivitySig_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultExcessWeightSignificance = Significance.Better,
                    AdultSmokingPrevalenceSignificance = Significance.Worse,
                    AdultPhysicalActivitySignificance = Significance.Better
                }
            };
            var sentence = builder.GetSentence5(Significance.Better);
            Assert.AreEqual(
                "Estimated levels of adult excess weight and physical activity are better than the England average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence5B_ExcessWeightSig_Better_SmokingPrevalenceSig_Better_PhysicalActivitySig_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultExcessWeightSignificance = Significance.Better,
                    AdultSmokingPrevalenceSignificance = Significance.Better,
                    AdultPhysicalActivitySignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence5(Significance.Better);
            Assert.AreEqual(
                "Estimated levels of adult excess weight and smoking are better than the England average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence5B_ExcessWeightSig_Worse_SmokingPrevalenceSig_Better_PhysicalActivitySig_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultExcessWeightSignificance = Significance.Worse,
                    AdultSmokingPrevalenceSignificance = Significance.Better,
                    AdultPhysicalActivitySignificance = Significance.Better
                }
            };
            var sentence = builder.GetSentence5(Significance.Better);
            Assert.AreEqual(
                "Estimated levels of adult smoking and physical activity are better than the England average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence5B_All_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultExcessWeightSignificance = Significance.Worse,
                    AdultSmokingPrevalenceSignificance = Significance.Worse,
                    AdultPhysicalActivitySignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence5(Significance.Better);
            Assert.AreEqual(string.Empty, sentence);
        }

        [TestMethod]
        public void TestSentence5B_Wrong_Data()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultExcessWeightSignificance = Significance.Same,
                    AdultSmokingPrevalenceSignificance = Significance.Same,
                    AdultPhysicalActivitySignificance = Significance.None
                }
            };
            var sentence = builder.GetSentence5(Significance.Better);
            Assert.AreNotEqual(
                "Estimated levels of adult excess weight, smoking and physical activity are worse than the England average.",
                sentence);
        }

        #endregion

        #region Sentence-6A Tests

        [TestMethod]
        public void TestSentence6A_ALL_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Worse,
                    AdultSTISignificance = Significance.Worse,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Worse,
                    AdultIncidenceOfTBSignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence6(Significance.Worse);
            Assert.AreEqual(
                "Rates of hip fractures, sexually transmitted infections, people killed and seriously injured on roads and TB are worse than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6A_Hip_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Worse,
                    AdultSTISignificance = Significance.Better,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Better,
                    AdultIncidenceOfTBSignificance = Significance.Better
                }
            };
            var sentence = builder.GetSentence6(Significance.Worse);
            Assert.AreEqual(
                "The rate of hip fractures is worse than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6A_STI_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Better,
                    AdultSTISignificance = Significance.Worse,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Better,
                    AdultIncidenceOfTBSignificance = Significance.Better
                }
            };
            var sentence = builder.GetSentence6(Significance.Worse);
            Assert.AreEqual(
                "The rate of sexually transmitted infections is worse than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6A_KilledAndSeriouslyInjuredOnRoads_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Better,
                    AdultSTISignificance = Significance.Better,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Worse,
                    AdultIncidenceOfTBSignificance = Significance.Better
                }
            };
            var sentence = builder.GetSentence6(Significance.Worse);
            Assert.AreEqual(
                "The rate of people killed and seriously injured on roads is worse than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6A_IncidenceOfTB_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Better,
                    AdultSTISignificance = Significance.Better,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Better,
                    AdultIncidenceOfTBSignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence6(Significance.Worse);
            Assert.AreEqual(
                "The rate of TB is worse than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6A_HipFractures_Worse_STI_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Worse,
                    AdultSTISignificance = Significance.Worse,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Better,
                    AdultIncidenceOfTBSignificance = Significance.Better
                }
            };
            var sentence = builder.GetSentence6(Significance.Worse);
            Assert.AreEqual(
                "Rates of hip fractures and sexually transmitted infections are worse than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6A_HipFractures_Worse_STI_Worse_KilledAndSeriouslyInjuredOnRoads_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Worse,
                    AdultSTISignificance = Significance.Worse,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Worse,
                    AdultIncidenceOfTBSignificance = Significance.Better
                }
            };
            var sentence = builder.GetSentence6(Significance.Worse);
            Assert.AreEqual(
                "Rates of hip fractures, sexually transmitted infections and people killed and seriously injured on roads are worse than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6A_HipFractures_Worse_STI_Worse_TB_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Worse,
                    AdultSTISignificance = Significance.Worse,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Better,
                    AdultIncidenceOfTBSignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence6(Significance.Worse);
            Assert.AreEqual(
                "Rates of hip fractures, sexually transmitted infections and TB are worse than average.",
                sentence);
        }

        #endregion

        #region Sentence-6B Tests

        [TestMethod]
        public void TestSentence6B_ALL_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Better,
                    AdultSTISignificance = Significance.Better,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Better,
                    AdultIncidenceOfTBSignificance = Significance.Better
                }
            };
            var sentence = builder.GetSentence6(Significance.Better);
            Assert.AreEqual(
                "Rates of hip fractures, sexually transmitted infections, people killed and seriously injured on roads and TB are better than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6B_Hip_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Better,
                    AdultSTISignificance = Significance.Worse,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Worse,
                    AdultIncidenceOfTBSignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence6(Significance.Better);
            Assert.AreEqual(
                "The rate of hip fractures is better than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6B_STI_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Worse,
                    AdultSTISignificance = Significance.Better,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Worse,
                    AdultIncidenceOfTBSignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence6(Significance.Better);
            Assert.AreEqual(
                "The rate of sexually transmitted infections is better than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6B_KilledAndSeriouslyInjuredOnRoads_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Worse,
                    AdultSTISignificance = Significance.Worse,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Better,
                    AdultIncidenceOfTBSignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence6(Significance.Better);
            Assert.AreEqual(
                "The rate of people killed and seriously injured on roads is better than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6B_IncidenceOfTB_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Worse,
                    AdultSTISignificance = Significance.Worse,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Worse,
                    AdultIncidenceOfTBSignificance = Significance.Better
                }
            };
            var sentence = builder.GetSentence6(Significance.Better);
            Assert.AreEqual(
                "The rate of TB is better than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6B_HipFractures_Worse_STI_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Better,
                    AdultSTISignificance = Significance.Better,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Worse,
                    AdultIncidenceOfTBSignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence6(Significance.Better);
            Assert.AreEqual(
                "Rates of hip fractures and sexually transmitted infections are better than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6B_HipFractures_Worse_STI_Worse_KilledAndSeriouslyInjuredOnRoads_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Better,
                    AdultSTISignificance = Significance.Better,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Better,
                    AdultIncidenceOfTBSignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence6(Significance.Better);
            Assert.AreEqual(
                "Rates of hip fractures, sexually transmitted infections and people killed and seriously injured on roads are better than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence6B_Wrong_Data()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultHipFracturesSignificance = Significance.Worse,
                    AdultSTISignificance = Significance.Worse,
                    AdultKilledAndSeriouslyInjuredOnRoadsSignificance = Significance.Better,
                    AdultIncidenceOfTBSignificance = Significance.Worse
                }
            };
            var sentence = builder.GetSentence6(Significance.Better);
            Assert.AreNotEqual(
                "Rates of hip fractures and sexually transmitted infections are worse than average.",
                sentence);
        }

        #endregion

        #region Sentence-7A Tests

        [TestMethod]
        public void TestSentence7A_1_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultStatutoryHomelessnessSig = Significance.Worse,
                    AdultViolentCrimeSig = Significance.Same,
                    AdultLongTermUnemploymentSig = Significance.Same,
                    AdultExcessWinterDeathsSig = Significance.Same,
                    AdultUnder75MortalityRateCvdSig = Significance.Better,
                    AdultUnder75MortalityRateCancerSig = Significance.Better,
                }
            };
            var sentence = builder.GetSentence7(Significance.Worse);
            Assert.AreEqual(
                "The rate of statutory homelessness is worse than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence7A_2_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultStatutoryHomelessnessSig = Significance.Worse,
                    AdultViolentCrimeSig = Significance.Worse,
                    AdultLongTermUnemploymentSig = Significance.Same,
                    AdultExcessWinterDeathsSig = Significance.Better,
                    AdultUnder75MortalityRateCvdSig = Significance.Better,
                    AdultUnder75MortalityRateCancerSig = Significance.Better,
                }
            };
            var sentence = builder.GetSentence7(Significance.Worse);
            Assert.AreEqual(
                "Rates of statutory homelessness and violent crime are worse than average.",
                sentence);
        }


        [TestMethod]
        public void TestSentence7A_3_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultStatutoryHomelessnessSig = Significance.Worse,
                    AdultViolentCrimeSig = Significance.Worse,
                    AdultLongTermUnemploymentSig = Significance.Worse,
                    AdultExcessWinterDeathsSig = Significance.Better,
                    AdultUnder75MortalityRateCvdSig = Significance.Better,
                    AdultUnder75MortalityRateCancerSig = Significance.Better,
                }
            };
            var sentence = builder.GetSentence7(Significance.Worse);
            Assert.AreEqual(
                "Rates of statutory homelessness, violent crime and long term unemployment are worse than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence7A_4_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultStatutoryHomelessnessSig = Significance.Worse,
                    AdultViolentCrimeSig = Significance.Better,
                    AdultLongTermUnemploymentSig = Significance.Worse,
                    AdultExcessWinterDeathsSig = Significance.Better,
                    AdultUnder75MortalityRateCvdSig = Significance.Better,
                    AdultUnder75MortalityRateCancerSig = Significance.Better,
                }
            };
            var sentence = builder.GetSentence7(Significance.Worse);
            Assert.AreEqual(
                "Rates of statutory homelessness and long term unemployment are worse than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence7A_0_Worse()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultStatutoryHomelessnessSig = Significance.Better,
                    AdultViolentCrimeSig = Significance.Better,
                    AdultLongTermUnemploymentSig = Significance.Better,
                    AdultExcessWinterDeathsSig = Significance.Better,
                    AdultUnder75MortalityRateCvdSig = Significance.Better,
                    AdultUnder75MortalityRateCancerSig = Significance.Better,
                }
            };
            var sentence = builder.GetSentence7(Significance.Worse);
            Assert.AreEqual(
                "",
                sentence);
        }

        #endregion

        #region Sentence-7B Tests

        [TestMethod]
        public void TestSentence7B_0_Beter()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultStatutoryHomelessnessSig = Significance.Worse,
                    AdultViolentCrimeSig = Significance.Worse,
                    AdultLongTermUnemploymentSig = Significance.Worse,
                    AdultExcessWinterDeathsSig = Significance.Worse,
                    AdultUnder75MortalityRateCvdSig = Significance.Worse,
                    AdultUnder75MortalityRateCancerSig = Significance.Worse,
                }
            };
            var sentence = builder.GetSentence7(Significance.Better);
            Assert.AreEqual(
                "",
                sentence);
        }

        [TestMethod]
        public void TestSentence7B_1_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultStatutoryHomelessnessSig = Significance.Better,
                    AdultViolentCrimeSig = Significance.Same,
                    AdultLongTermUnemploymentSig = Significance.Same,
                    AdultExcessWinterDeathsSig = Significance.Same,
                    AdultUnder75MortalityRateCvdSig = Significance.Worse,
                    AdultUnder75MortalityRateCancerSig = Significance.Worse,
                }
            };
            var sentence = builder.GetSentence7(Significance.Better);
            Assert.AreEqual(
                "The rate of statutory homelessness is better than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence7B_2_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultStatutoryHomelessnessSig = Significance.Worse,
                    AdultViolentCrimeSig = Significance.Same,
                    AdultLongTermUnemploymentSig = Significance.Same,
                    AdultExcessWinterDeathsSig = Significance.Same,
                    AdultUnder75MortalityRateCvdSig = Significance.Better,
                    AdultUnder75MortalityRateCancerSig = Significance.Better,
                }
            };
            var sentence = builder.GetSentence7(Significance.Better);
            Assert.AreEqual(
                "Rates of early deaths from cardiovascular diseases and early deaths from cancer are better than average.",
                sentence);
        }


        [TestMethod]
        public void TestSentence7B_3_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultStatutoryHomelessnessSig = Significance.Same,
                    AdultViolentCrimeSig = Significance.Same,
                    AdultLongTermUnemploymentSig = Significance.Same,
                    AdultExcessWinterDeathsSig = Significance.Better,
                    AdultUnder75MortalityRateCvdSig = Significance.Better,
                    AdultUnder75MortalityRateCancerSig = Significance.Better,
                }
            };
            var sentence = builder.GetSentence7(Significance.Better);
            Assert.AreEqual(
                "Rates of excess winter deaths, early deaths from cardiovascular diseases and early deaths from cancer are better than average.",
                sentence);
        }

        [TestMethod]
        public void TestSentence7B_4_Better()
        {
            var builder = new HealthProfilesKeyMessage4Builder
            {
                data = new KeyMessageData
                {
                    AdultStatutoryHomelessnessSig = Significance.Same,
                    AdultViolentCrimeSig = Significance.Same,
                    AdultLongTermUnemploymentSig = Significance.Same,
                    AdultExcessWinterDeathsSig = Significance.Better,
                    AdultUnder75MortalityRateCvdSig = Significance.Better,
                    AdultUnder75MortalityRateCancerSig = Significance.Better,
                }
            };
            var sentence = builder.GetSentence7(Significance.Better);
            Assert.AreEqual(
                "Rates of excess winter deaths, early deaths from cardiovascular diseases and early deaths from cancer are better than average.",
                sentence);
        }

        #endregion

    }
}
