using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.SearchQuerying
{
    public static class RelatedTermsProvider
    {
        private static Dictionary<string, IList<string>> _relatedTerms = null;

        public static IList<string> GetRelatedTerms(string text)
        {
            Init();

            if (_relatedTerms.ContainsKey(text))
            {
                return _relatedTerms[text];
            }
            return new List<string> { text };
        }

        public static void Init()
        {
            if (_relatedTerms != null) return;

            _relatedTerms = new Dictionary<string, IList<string>>();

            // Read file
            var lines = ReadFile();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) == false && line[0].Equals('#') == false)
                {
                    // Split line into trimmed words
                    var keyAndTerms = line.Split('>').ToList()
                        .Select(x => x.Trim()).ToList();

                    // Split related terms
                    var terms = keyAndTerms[1].Split(',').ToList()
                        .Select(x => x.Trim()).ToList();

                    // Add main term to list of related terms
                    var key = keyAndTerms[0];
                    terms.Add(key);

                    _relatedTerms.Add(key, terms);
                }
            }
        }

        private static IList<string> ReadFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "PholioVisualisation.SearchQuerying.related-terms.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.None
                ).ToList();
            }
        }
    }
}
