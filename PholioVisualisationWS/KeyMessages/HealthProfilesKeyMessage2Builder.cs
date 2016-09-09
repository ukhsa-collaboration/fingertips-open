using System.Collections.Generic;
using Nustache.Core;

namespace PholioVisualisation.KeyMessages
{
    public class HealthProfilesKeyMessage2Builder
    {
        private const string SentenceTemplate =
            @"Life expectancy is {{ComparisonText}} in the most deprived areas of {{Area}} than in the least deprived areas.";

        internal KeyMessageData data; /* set as internal so can be mocked in unit tests */

        public string ProcessKeyMessage(KeyMessageData data)
        {
            this.data = data;
            return GetSentence();
        }

        internal string GetSentence()
        {
            string comparisonText;

            if (string.IsNullOrEmpty(data.MaleSlopeIndexOfInequalityForLifeExpectancy) ||
                string.IsNullOrEmpty(data.FemaleSlopeIndexOfInequalityForLifeExpectancy))
            {
                return string.Empty;
            }

            if (data.IsMaleSlopeIndexOfInequalitySignificant && data.IsFemaleSlopeIndexOfInequalitySignificant)
            {
                comparisonText = string.Format("{0} years lower for men and {1} years lower for women",
                    data.MaleSlopeIndexOfInequalityForLifeExpectancy,
                    data.FemaleSlopeIndexOfInequalityForLifeExpectancy);
            }
            else if (!data.IsMaleSlopeIndexOfInequalitySignificant && data.IsFemaleSlopeIndexOfInequalitySignificant)
            {
                comparisonText = string.Format("{0} years lower for women",
                    data.FemaleSlopeIndexOfInequalityForLifeExpectancy);
            }
            else if (data.IsMaleSlopeIndexOfInequalitySignificant && !data.IsFemaleSlopeIndexOfInequalitySignificant)
            {
                comparisonText = string.Format("{0} years lower for men",
                    data.MaleSlopeIndexOfInequalityForLifeExpectancy);
            }
            else
            {
                comparisonText = "not significantly different for people";
            }

            var sentenceData = new Dictionary<string, string>
            {
                {"ComparisonText", comparisonText},
                {"Area", data.Area.Name}
            };
            return Render.StringToString(SentenceTemplate, sentenceData);
        }
    }
}