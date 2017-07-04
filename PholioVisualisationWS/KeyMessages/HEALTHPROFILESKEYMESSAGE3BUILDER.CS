using System.Collections.Generic;
using Nustache.Core;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessages
{
    public class HealthProfilesKeyMessage3Builder
    {
        private const string Sentence1Template =
            @"In Year 6, {{ObesePercent}}% ({{ObeseCount}}) of children are classified as obese{{SignificanceText}}";

        private const string Sentence2Template =
            @"The rate of alcohol-specific hospital stays among those under 18 is {{NoOfStays}}{{SignificanceText}} This represents {{StaysPerYear}} stays per year.";

        private const string Sentence3Template =
            @"Levels of {{Options}} are {{BetterOrWorse}} than the England average.";

        internal KeyMessageData data;

        public string ProcessKeyMessage(KeyMessageData data)
        {
            this.data = data;
            var message = new SentenceJoiner();
            message.Add(GetSentence1());
            message.Add(GetSentence2());
            message.Add(GetSentence3());
            message.Add(GetSentence4());
            return message.Join();
        }

        internal string GetSentence1()
        {
            string significanceText = HealthProfilesKeyMessage4Builder.GetSignificanceText(data.ObesityYear6Significance);

            var sentenceData = new Dictionary<string, string>
            {
                {"ObesePercent", data.ObesityYear6Percentage},
                {"ObeseCount", data.ObesityYear6Count},
                {"SignificanceText", significanceText}
            };
            return Render.StringToString(Sentence1Template, sentenceData);
        }

        internal string GetSentence2()
        {
            if (data.Under18AlcoholSpecificHospitalStays.HasValue)
            {
                string significanceText = HealthProfilesKeyMessage4Builder.GetSignificanceTextWithAsterisks(
                    data.Under18AlcoholSpecificSignificance);

                var sentenceData = new Dictionary<string, string>
                {
                    {"NoOfStays", NumberCommariser.Commarise0DP(data.Under18AlcoholSpecificHospitalStays.Value)},
                    {"SignificanceText", significanceText},
                    {"StaysPerYear", data.Under18AlcoholSpecificHospitalStaysPerYear}
                };
                return Render.StringToString(Sentence2Template, sentenceData);
            }
            else
            {
                return string.Empty;
            }
        }

        internal string GetSentence3()
        {
            return ProcessSentance3And4(Significance.Worse);
        }

        internal string GetSentence4()
        {
            return ProcessSentance3And4(Significance.Better);
        }

        private string ProcessSentance3And4(Significance sig)
        {
            var qualifiedItems = new List<string>();
            if ((data.TeenagePregnancySig != sig) &&
                (data.GcseAchievementSig != sig) &&
                (data.BreastfeedingInitiationSig != sig) &&
                (data.SmokingAtTimeOfDeliverySig != sig))
            {
                return string.Empty;
            }

            if (data.TeenagePregnancySig == sig)
            {
                qualifiedItems.Add("teenage pregnancy");
            }

            if (data.GcseAchievementSig == sig)
            {
                qualifiedItems.Add("GCSE attainment");
            }

            if (data.BreastfeedingInitiationSig == sig)
            {
                qualifiedItems.Add("breastfeeding initiation");
            }

            if (data.SmokingAtTimeOfDeliverySig == sig)
            {
                qualifiedItems.Add("smoking at time of delivery");
            }

            string options = PhraseJoiner.Join(qualifiedItems);
            string betterOrWorse = HealthProfilesKeyMessage4Builder.GetBetterOrWorse(sig);

            var sentenceData = new Dictionary<string, string> {
                {"Options", options}, 
                {"BetterOrWorse", betterOrWorse}
            };
            return Render.StringToString(Sentence3Template, sentenceData);
        }
    }
}