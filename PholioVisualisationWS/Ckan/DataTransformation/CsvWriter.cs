using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ckan.DataTransformation
{
    public class CsvWriter
    {
        private const string Quote = "\"";
        private const string EscapedQuote = "\"\"";
        private readonly static char[] CharactersThatMustBeQuoted = { ',', '"', '\n' };

        private List<string> headerItems = new List<string>();
        private List<List<string>> rowItemsList = new List<List<string>>();

        public void AddHeader(params object[] items)
        {
            headerItems = GetStringList(items);
        }

        public void AddLine(params object[] items)
        {
            rowItemsList.Add(GetStringList(items));
        }

        private static List<string> GetStringList(IEnumerable<object> items)
        {
            return items.Select(x => x.ToString()).ToList();
        }

        public byte[] WriteAsBytes()
        {
            byte[] bytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(memoryStream))
                {
                    // Header
                    writer.WriteLine(GetLine(headerItems));

                    foreach (var rowItems in rowItemsList)
                    {
                        var line = GetLine(rowItems);
                        writer.WriteLine(line);
                    }

                    writer.Close();
                    bytes = memoryStream.ToArray();
                }
            }

            return bytes;
        }

        private static string GetLine(List<string> rowItemsList)
        {
            var line = string.Join(",", rowItemsList.Select(Escape));
            return line;
        }

        private static string Escape(string s)
        {
            if (s.Contains(Quote))
            {
                s = s.Replace(Quote, EscapedQuote);
            }

            if (s.IndexOfAny(CharactersThatMustBeQuoted) > -1)
            {
                s = Quote + s + Quote;
            }

            return s;
        }
    }
}