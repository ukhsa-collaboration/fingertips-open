using System.Collections.Generic;
using System.Text;
using Nustache.Core;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessages
{

    public class HealthProfilesKeyMessage1Builder
    {

        private const string Sentence1Template = @"The health of people in {{AreaName}} is {{WorseBetterSimilarVaried}} the England average.";
        private const string Sentence2Template = @"{{DeprivationisLowerOrHigher}} {{Value}}% ({{CountPerYear}}) children live in poverty.";
        private const string Sentence3Template = @"Life expectancy for {{MenWomen}} {{HigherLower}} the England average.";

        internal KeyMessageData data; /* set as internal so can be mocked in unit tests*/


        public string ProcessKeyMessage(KeyMessageData data)
        {
            this.data = data;
            var message = new SentenceJoiner();
            message.Add(GetSentence1());
            message.Add(GetSentence2());
            message.Add(GetSentence3());
            return message.Join();
        }

        /// <summary>
        /// Generate 1st sentence for KeyMessage 1 according to data provided.
        /// </summary>
        /// <returns>sentence1</returns>
        internal string GetSentence1()
        {
            var counter = data.SpineChartSignificances;
            var green = counter.ProportionGreen;
            var red = counter.ProportionRed;
            var amber = counter.ProportionAmber;
            string worseBetterSimilarVaried;

            if ((red >= 0.5 && green <= 0.14) || (red >= 0.4 && green == 0.0) || (red > 0.7))
            {
                worseBetterSimilarVaried = "generally worse than";
            }
            else if ((green >= 0.5 && red <= 0.14) || (green >= 0.4 && red == 0.0) || (green > 0.7))
            {
                worseBetterSimilarVaried = "generally better than";
            }
            else if (amber >= 0.69)
            {
                worseBetterSimilarVaried = "generally similar to";
            }
            else
            {
                worseBetterSimilarVaried = "varied compared with";
            }

            var sentenceData = new Dictionary<string, string> {
                { "AreaName", data.AreaName }, 
                { "WorseBetterSimilarVaried", worseBetterSimilarVaried }
            };
            return Render.StringToString(Sentence1Template, sentenceData);
        }

        /// <summary>
        /// Generate 2nd sentence for KeyMessage 1 according to data provided.
        /// </summary>
        /// <returns>sentence2</returns>
        internal string GetSentence2()
        {
            var childrenInPoverty = data.ChildrenInPoverty;
            if (childrenInPoverty == null || childrenInPoverty.IsValueValid == false)
            {
                return string.Empty;
            }

            string isLowerOrHigherText = string.Empty;

            switch (data.DeprivationSig)
            {
                case Significance.Better:
                    isLowerOrHigherText = "Deprivation is lower than average, however about";
                    break;
                case Significance.Worse:
                    isLowerOrHigherText = "Deprivation is higher than average and about";
                    break;
                case Significance.Same:
                    isLowerOrHigherText = "About";
                    break;
                case Significance.None:
                    /*In case we don't have sig data, to avoid breaking
                    * the process of creating pdfs, just return empty string.*/
                    return string.Empty;
            }

            var countperYear = childrenInPoverty.CountPerYear;
            var count = NumberCommariser.Commarise0DP(NumberRounder.ToNearest100(countperYear));
            var val = NumericFormatter.Format1DP(childrenInPoverty.Value);

            var sentenceData = new Dictionary<string, string> {
                { "DeprivationisLowerOrHigher", isLowerOrHigherText },
                { "CountPerYear", count },
                { "Value", val } 
            };

            return Render.StringToString(Sentence2Template, sentenceData);
        }

        /// <summary>
        /// Generate 3rd sentence for KeyMessage 1 according to data provided.
        /// </summary>
        /// <returns>sentence3</returns>
        internal string GetSentence3()
        {
            string menWomen, higherLower;

            if (data.MaleLifeExpectancyAtBirth == Significance.Better && data.FemaleLifeExpectancyAtBirth == Significance.Better)
            {
                //GG
                menWomen = "both men and women is";
                higherLower = "higher than";
            }
            else if (data.MaleLifeExpectancyAtBirth == Significance.Worse && data.FemaleLifeExpectancyAtBirth == Significance.Worse)
            {
                //RR
                menWomen = "both men and women is";
                higherLower = "lower than";
            }
            else if (data.MaleLifeExpectancyAtBirth == Significance.Same && data.FemaleLifeExpectancyAtBirth == Significance.Same)
            {
                //AA
                menWomen = "both men and women is";
                higherLower = "similar to";
            }
            else if (data.MaleLifeExpectancyAtBirth == Significance.Worse && data.FemaleLifeExpectancyAtBirth == Significance.Better)
            {
                //RG
                menWomen = "men is lower and for women";
                higherLower = "higher than";
            }
            else if (data.MaleLifeExpectancyAtBirth == Significance.Better && data.FemaleLifeExpectancyAtBirth == Significance.Worse)
            {
                //GR
                menWomen = "men is higher and for women";
                higherLower = "lower than";
            }
            else if (data.MaleLifeExpectancyAtBirth == Significance.Worse && data.FemaleLifeExpectancyAtBirth == Significance.Same)
            {
                //RA
                menWomen = "men is";
                higherLower = "lower than";
            }
            else if (data.MaleLifeExpectancyAtBirth == Significance.Same && data.FemaleLifeExpectancyAtBirth == Significance.Worse)
            {
                //AR
                menWomen = "women is";
                higherLower = "lower than";
            }
            else if (data.MaleLifeExpectancyAtBirth == Significance.Better && data.FemaleLifeExpectancyAtBirth == Significance.Same)
            {
                //GA
                menWomen = "men is";
                higherLower = "higher than";
            }
            else if (data.MaleLifeExpectancyAtBirth == Significance.Same && data.FemaleLifeExpectancyAtBirth == Significance.Better)
            {
                //AG
                menWomen = "women is";
                higherLower = "higher than";
            }
            else
            {
                return string.Empty; /* Return empty string of anything else. */
            }

            var sentenceData = new Dictionary<string, string> { { "MenWomen", menWomen }, { "HigherLower", higherLower } };
            return Render.StringToString(Sentence3Template, sentenceData);
        }
    }
}
