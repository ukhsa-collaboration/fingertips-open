using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PholioVisualisation.KeyMessages
{
    /// <summary>
    ///     Required because messages may be created by people who do not
    ///     understand now to create valid HTML.
    /// </summary>
    public class KeyMessageOverrideCleaner
    {
        public KeyMessageOverrideCleaner(string message)
        {
            CleanMessage = message;
            EnsureHrefInAllLinksStartsWithHttp();
            EnsureSingleQuotesAreReplacedWithDoubleQuotesInLinks();
        }

        public string CleanMessage { get; private set; }

        private void EnsureSingleQuotesAreReplacedWithDoubleQuotesInLinks()
        {
            CleanMessage = Regex.Replace(CleanMessage, " href='([^']+)'", 
                " href=\"$1\"",RegexOptions.IgnoreCase);
        }

        private void EnsureHrefInAllLinksStartsWithHttp()
        {
            List<int> hrefIndexes = GetIndexesOfMatches(" href=");
            List<int> hrefsWithHttpIndexes = GetIndexesOfMatches(" href=.http");

            List<int> indexesToFix = hrefIndexes.Where(x => hrefsWithHttpIndexes.Contains(x) == false).ToList();
            for (int i = indexesToFix.Count - 1; i >= 0; i--)
            {
                CleanMessage = CleanMessage.Insert(indexesToFix[i] + 7, "http://");
            }
        }

        private List<int> GetIndexesOfMatches(string regexString)
        {
            var regex = new Regex(regexString, RegexOptions.IgnoreCase);

            MatchCollection matches = regex.Matches(CleanMessage);
            var indexes = new List<int>();
            foreach (Match match in matches)
            {
                indexes.AddRange(from Capture capture in match.Captures select capture.Index);
            }

            return indexes;
        }
    }
}