using System.Collections.Generic;
using System.Linq;
using Nustache.Core;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessages
{
    public class HealthProfilesKeyMessage4Builder
    {
        private const string Sentence2Template =
            "The rate of alcohol-related harm hospital stays is {{AlcoholAdmissionsToHospital}}{{SignificanceText}} This represents {{AdultAlcoholAdmissionsPerYear}} stays per year.";

        private const string Sentence3Template =
            "The rate of self-harm hospital stays is {{AdultSelfHarmAdmissions}}{{SignificanceText}} This represents {{AdultSelfHarmAdmissionsPerYear}} stays per year.";

        private const string Sentence5Template =
            "Estimated levels of adult {{Options}} are {{BetterOrWorse}} than the England average.";

        private const string Sentence6And7Template =
            "{{Rate}} of {{Options}} {{IsOrAre}} {{BetterOrWorse}} than average.";


        internal KeyMessageData data; /* set as internal so can be mocked in unit tests*/

        public string ProcessKeyMessage(KeyMessageData data)
        {
            this.data = data;
            var message = new SentenceJoiner();
            message.Add(GetSentence2());
            message.Add(GetSentence3());
            message.Add(GetSentence5(Significance.Worse));
            message.Add(GetSentence5(Significance.Better));
            message.Add(GetSentence6(Significance.Worse));
            message.Add(GetSentence6(Significance.Better));
            message.Add(GetSentence7(Significance.Worse));
            message.Add(GetSentence7(Significance.Better));
            return message.Join();
        }

        public string GetSentence2()
        {
            var coreDataSet = data.AdultAlcoholAdmissions;
            if (coreDataSet == null)
            {
                return string.Empty;
            }

            var sentenceData = new Dictionary<string, string>
            {
                {"AlcoholAdmissionsToHospital", NumberCommariser.Commarise0DP(coreDataSet.Value)},
                {
                    "AdultAlcoholAdmissionsPerYear",
                    NumberCommariser.Commarise0DP(data.AdultAlcoholAdmissionsPerYear.Value)
                },
                {"SignificanceText", GetSignificanceTextWithAsterisks(data.AdultAlcoholAdmissionsSignificance)}
            };
            return Render.StringToString(Sentence2Template, sentenceData);
        }

        public string GetSentence3()
        {
            if (string.IsNullOrWhiteSpace(data.AdultSelfHarmAdmissions))
            {
                return string.Empty;
            }

            var sentenceData = new Dictionary<string, string>
            {
                {"AdultSelfHarmAdmissions", data.AdultSelfHarmAdmissions},
                {"AdultSelfHarmAdmissionsPerYear", data.AdultSelfHarmAdmissionsPerYear},
                {"SignificanceText", GetSignificanceTextWithAsterisks(data.AdultSelfHarmAdmissionsSignificance)}
            };
            return Render.StringToString(Sentence3Template, sentenceData);
        }

        public string GetSentence5(Significance sig)
        {
            var labels = new List<SignificanceLabel>
            {
                SignificanceLabel.New(data.AdultExcessWeightSignificance,"excess weight"),
                SignificanceLabel.New(data.AdultSmokingPrevalenceSignificance,"smoking"),
                SignificanceLabel.New(data.AdultSmokingInRoutineAndManualOccupationsSignificance,
                    "smoking in routine and manual occupations"),
                SignificanceLabel.New(data.AdultPhysicalActivitySignificance,"physical activity"),
            };

            if (labels.Any(x => x.Significance == sig) == false)
            {
                return string.Empty;
            }

            var qualifiedItems = GetQualifiedItems(sig, labels);

            string options = PhraseJoiner.Join(qualifiedItems);

            var sentenceData = new Dictionary<string, string>
            {
                {"Options", options},
                {"BetterOrWorse", GetBetterOrWorse(sig)}
            };
            return Render.StringToString(Sentence5Template, sentenceData);
        }

        public string GetSentence6(Significance sig)
        {
            var labels = new List<SignificanceLabel>
            {
                SignificanceLabel.New(data.AdultHipFracturesSignificance,"hip fractures"),
                SignificanceLabel.New(data.AdultSTISignificance,"sexually transmitted infections"),
                SignificanceLabel.New(data.AdultKilledAndSeriouslyInjuredOnRoadsSignificance,
                    "people killed and seriously injured on roads"),
                SignificanceLabel.New(data.AdultIncidenceOfTBSignificance, "TB")
            };

            if (labels.Any(x => x.Significance == sig) == false)
            {
                return string.Empty;
            }

            var qualifiedItems = GetQualifiedItems(sig, labels);

            string rate, isOrAre;
            if (qualifiedItems.Count > 1)
            {
                rate = "Rates";
                isOrAre = "are";
            }
            else
            {
                rate = "The rate";
                isOrAre = "is";
            }

            string options = PhraseJoiner.Join(qualifiedItems);

            var sentenceData = new Dictionary<string, string>
            {
                {"Rate", rate},
                {"Options", options},
                {"IsOrAre", isOrAre},
                {"BetterOrWorse", GetBetterOrWorse(sig)}
            };
            return Render.StringToString(Sentence6And7Template, sentenceData);
        }

        public string GetSentence7(Significance sig)
        {
            var labels = new List<SignificanceLabel>
            {
                SignificanceLabel.New(data.AdultStatutoryHomelessnessSig,"statutory homelessness"),
                SignificanceLabel.New(data.AdultViolentCrimeSig,"violent crime"),
                SignificanceLabel.New(data.AdultExcessWinterDeathsSig,"excess winter deaths"),
                SignificanceLabel.New(data.AdultUnder75MortalityRateCvdSig,"early deaths from cardiovascular diseases"),
                SignificanceLabel.New(data.AdultUnder75MortalityRateCancerSig,"early deaths from cancer"),
                SignificanceLabel.New(data.PeopleInEmploymentSig,"the percentage of people in employment")
            };

            var significanceCounter = new SignificanceCounter(labels.Select(x => x.Significance));

            if (significanceCounter.GetProportionGreen() < 0.5 &&
                significanceCounter.GetProportionAmber() < 0.5)
            {
                return string.Empty;
            }

            var qualifiedItems  = GetQualifiedItems(sig, labels);

            if (!qualifiedItems.Any())
                // check at least one of the indicator is red or green ( depending upon passed sig )
                return string.Empty;

            string rate, isOrAre;
            if (qualifiedItems.Count > 1)
            {
                rate = "Rates";
                isOrAre = "are";
            }
            else
            {
                rate = "The rate";
                isOrAre = "is";
            }

            string options = PhraseJoiner.Join(qualifiedItems);

            var sentenceData = new Dictionary<string, string>
            {
                {"Rate", rate},
                {"Options", options},
                {"IsOrAre", isOrAre},
                {"BetterOrWorse", GetBetterOrWorse(sig)}
            };

            return Render.StringToString(Sentence6And7Template, sentenceData);
        }

        public static string GetSignificanceTextWithAsterisks(Significance significance)
        {
            switch (significance)
            {
                case Significance.Better:
                    return "*, better than the average for England.";
                case Significance.Worse:
                    return "*, worse than the average for England.";
                default:
                    return "*.";
            }
        }

        public static string GetSignificanceText(Significance significance)
        {
            switch (significance)
            {
                case Significance.Better:
                    return ", better than the average for England.";
                case Significance.Worse:
                    return ", worse than the average for England.";
                default:
                    return ".";
            }
        }

        public static string GetBetterOrWorse(Significance sig)
        {
            return sig == Significance.Better ? "better" : "worse";
        }

        private static List<string> GetQualifiedItems(Significance sig, List<SignificanceLabel> labels)
        {
            var qualifiedItems = new List<string>();
            foreach (var label in labels)
            {
                if (label.Significance == sig)
                    qualifiedItems.Add(label.Label);
            }
            return qualifiedItems;
        }
    }
}