using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PholioVisualisation.DataAccess
{
    public class CsvWriter
    {
        private const string Quote = "\"";
        private const string EscapedQuote = "\"\"";
        private static readonly char[] CharactersThatMustBeQuoted = { ',', '"', '\n' };

        private object[] headerItems = {};
        private readonly List<object[]> rowItemsList = new List<object[]>();

        public void AddHeader(params object[] items)
        {
            headerItems = items;
        }

        public void AddLine(params object[] items)
        {
            rowItemsList.Add(items);
        }

        public byte[] WriteAsBytes()
        {
            byte[] bytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(memoryStream))
                {
                    // Header
                    if (headerItems.Any())
                    {
                        writer.WriteLine(GetLine(headerItems));
                    }

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

        private static string GetLine(object[] rowItemsList)
        {
            var items = rowItemsList.Select(Escape);
            var line = string.Join(",", items);
            return line;
        }

        private static object Escape(object o)
        {
            if (o is string == false)
            {
                return o;
            }

            string s = (string)o;

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