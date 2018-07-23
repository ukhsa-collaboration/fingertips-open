using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PholioVisualisation.SearchIndexing
{
    /// <summary>
    /// Reads lists of synonyms from file.
    /// </summary>
    public static class SynonymReader
    {
        public static IList<IList<string>> GetSynonymLists()
        {
            var synonymLists = new List<IList<string>>();

            var lines = ReadFile();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) == false && line[0].Equals('#') == false)
                {
                    // Split line into trimmed words
                    var words = line.Split(',').ToList()
                        .Select(x => x.Trim()).ToList();

                    synonymLists.Add(words);
                }
            }

            return synonymLists;
        }

        private static IList<string> ReadFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "PholioVisualisation.SearchIndexing.synonyms.txt";

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