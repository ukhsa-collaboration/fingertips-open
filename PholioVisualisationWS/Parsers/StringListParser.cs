using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.Parsers
{
    public class StringListParser
    {
        private string concaternatedStrings;

        public StringListParser(string concaternatedStrings)
        {
            this.concaternatedStrings = concaternatedStrings;
        }

        public List<string> StringList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(concaternatedStrings))
                {
                    return new List<string>();
                }

                return concaternatedStrings
                    .Split(',')
                    .Select(bit => bit.Trim())
                    .Where(trimmed => trimmed.Length > 0)
                    .ToList();
            }
        }

    }
}
