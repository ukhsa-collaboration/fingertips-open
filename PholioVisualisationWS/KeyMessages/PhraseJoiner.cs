using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.KeyMessages
{
    public static class PhraseJoiner
    {
        public static string Join(List<string> items)
        {
            string outputString;
            if (items.Count > 0)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < items.Count; i++)
                {
                    if (i == 0)
                    {
                        sb.Append(items[i]);
                    }
                    else if ((i >= 1) && (i < items.Count - 1))
                    {
                        sb.Append(", ");
                        sb.Append(items[i]);
                    }
                    else if (i == items.Count - 1)
                    {
                        sb.Append(" and ");
                        sb.Append(items[i]);
                    }
                }
                outputString =  sb.ToString();
            }
            else
            {
                outputString = string.Empty;
            }
            return outputString;
        }
    }
}
