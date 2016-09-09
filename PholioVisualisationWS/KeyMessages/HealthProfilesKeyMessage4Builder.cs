using System.Collections.Generic;
using System.Linq;
using Nustache.Core;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessages
{
    public class HealthProfilesKeyMessage4Builder
    {
        private const string Sentence1Template =
            "In {{Year}}, {{AdultObesityPercent}}%{{AdultObesityCount}} of adults are classified as obese{{SignificanceText}}";

        private const string Sentence2Template =
            "The rate of alcohol-related harm hospital stays is {{AlcoholAdmissionsToHospital}}{{SignificanceText}} This represents {{AdultAlcoholAdmissionsPerYear}} stays per year.";

        private const string Sentence3Template =
            "The rate of self-harm hospital stays is {{AdultSelfHarmAdmissions}}{{SignificanceText}} This represents {{AdultSelfHarmAdmissionsPerYear}} stays per year.";

        private const string Sentence4Template =
            "The rate of smoking related deaths is {{AdultSmokingRelatedDeaths}}{{SignificanceText}} This represents {{AdultSmokingRelatedDeathsPerYear}} deaths per year.";

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
            message.Add(GetSentence4());
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
            var sentenceData = new Dictionary<string, string>
            {
                {"AdultSelfHarmAdmissions", data.AdultSelfHarmAdmissions},
                {"AdultSelfHarmAdmissionsPerYear", data.AdultSelfHarmAdmissionsPerYear},
                {"SignificanceText", GetSignificanceTextWithAsterisks(data.AdultSelfHarmAdmissionsSignificance)}
            };
            return Render.StringToString(Sentence3Template, sentenceData);
        }

        public string GetSentence4()
        {
            if (data.AdultSmokingRelatedDeaths.HasValue &&
                data.AdultSmokingRelatedDeathsPerYear.HasValue)
            {
                var val = NumericFormatter.FormatZeroDP(data.AdultSmokingRelatedDeaths.Value);
                var count = NumberCommariser.Commarise0DP(data.AdultSmokingRelatedDeathsPerYear.Value);

                var sentenceData = new Dictionary<string, string>
                {
                    {"AdultSmokingRelatedDeaths", val},
                    {"AdultSmokingRelatedDeathsPerYear", count},
                    {"SignificanceText", GetSignificanceTextWithAsterisks(data.AdultSmokingRelatedDeathsSignificance)}
                };
                return Render.StringToString(Sentence4Template, sentenceData);
            }
            return string.Empty;
        }

        public string GetSentence5(Significance sig)
        {
            var qualifiedItems = new List<string>();
            if ((data.AdultExcessWeightSignificance != sig) &&
                (data.AdultSmokingPrevalenceSignificance != sig) &&
                (data.AdultPhysicalActivitySignificance != sig))
            {
                return string.Empty;
            }

            if (data.AdultExcessWeightSignificance == sig)
                qualifiedItems.Add("excess weight");
            if (data.AdultSmokingPrevalenceSignificance == sig)
                qualifiedItems.Add("smoking");
            if (data.AdultPhysicalActivitySignificance == sig)
                qualifiedItems.Add("physical activity");

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
            var qualifiedItems = new List<string>();
            if ((data.AdultHipFracturesSignificance != sig) &&
                (data.AdultSTISignificance != sig) &&
                (data.AdultKilledAndSeriouslyInjuredOnRoadsSignificance != sig) &&
                (data.AdultIncidenceOfTBSignificance != sig))
            {
                return string.Empty;
            }

            if (data.AdultHipFracturesSignificance == sig)
                qualifiedItems.Add("hip fractures");
            if (data.AdultSTISignificance == sig)
                qualifiedItems.Add("sexually transmitted infections");
            if (data.AdultKilledAndSeriouslyInjuredOnRoadsSignificance == sig)
                qualifiedItems.Add("people killed and seriously injured on roads");
            if (data.AdultIncidenceOfTBSignificance == sig)
                qualifiedItems.Add("TB");

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
            var sigList = new List<Significance>
            {
                data.AdultStatutoryHomelessnessSig,
                data.AdultViolentCrimeSig,
                data.AdultLongTermUnemploymentSig,
                data.AdultIncidenceOfMalignantMelanomaSig,
                data.AdultDrugMisuseSig,
                data.AdultExcessWinterDeathsSig,
                data.AdultUnder75MortalityRateCvdSig,
                data.AdultUnder75MortalityRateCancerSig
            };

            var significanceCounter = new SignificanceCounter(sigList);

            if ((!(significanceCounter.ProportionGreen >= 0.5)) && (!(significanceCounter.ProportionAmber >= 0.5)))
            {
                return string.Empty;
            }

            var qualifiedItems = new List<string>();

            if (data.AdultStatutoryHomelessnessSig == sig)
                qualifiedItems.Add("statutory homelessness");
            if (data.AdultViolentCrimeSig == sig)
                qualifiedItems.Add("violent crime");
            if (data.AdultLongTermUnemploymentSig == sig)
                qualifiedItems.Add("long term unemployment");
            if (data.AdultIncidenceOfMalignantMelanomaSig == sig)
                qualifiedItems.Add("new cases of malignant melanoma");
            if (data.AdultDrugMisuseSig == sig)
                qualifiedItems.Add("deaths from drug misuse");
            if (data.AdultExcessWinterDeathsSig == sig)
                qualifiedItems.Add("excess winter deaths");
            if (data.AdultUnder75MortalityRateCvdSig == sig)
                qualifiedItems.Add("early deaths from cardiovascular diseases");
            if (data.AdultUnder75MortalityRateCancerSig == sig)
                qualifiedItems.Add("early deaths from cancer");

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
    }
}