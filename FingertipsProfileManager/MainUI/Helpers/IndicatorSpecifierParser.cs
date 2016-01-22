using System.Collections.Generic;

namespace Fpm.MainUI.Helpers
{
    public static class IndicatorSpecifierParser
    {
        /// <summary>
        ///     Parses a list of indicator details, e.g. "1~2~3"
        /// </summary>
        public static List<IndicatorSpecifier> Parse(IEnumerable<string> itemStrings)
        {
            var details = new List<IndicatorSpecifier>();

            if (itemStrings != null)
            {
                foreach (string itemString in itemStrings)
                {
                    if (string.IsNullOrWhiteSpace(itemString) == false)
                    {
                        details.Add(Parse(itemString));
                    }
                }
            }
            return details;
        }

        private static IndicatorSpecifier Parse(string itemString)
        {
            string[] bits = itemString.Split('~');

            int ageId = bits.Length > 2
                ? int.Parse(bits[2])
                : 0;

            return new IndicatorSpecifier
            {
                IndicatorId = int.Parse(bits[0]),
                SexId = int.Parse(bits[1]),
                AgeId = ageId
            };
        }
    }
}