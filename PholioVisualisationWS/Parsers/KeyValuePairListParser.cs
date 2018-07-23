using System.Collections.Generic;

namespace PholioVisualisation.Parsers
{
    public static class KeyValuePairListParser
    {
        public static Dictionary<string, string> Parse(string keyValuePairString)
        {
            var map = new Dictionary<string,string>();

            var pairs = new StringListParser(keyValuePairString).StringList;
            foreach (var pair in pairs)
            {
                if (pair.Contains(":"))
                {
                    var bits = pair.Split(':');
                    map.Add(bits[0].Trim(), bits[1].Trim());
                }
            }

            return map;
        }
    }
}