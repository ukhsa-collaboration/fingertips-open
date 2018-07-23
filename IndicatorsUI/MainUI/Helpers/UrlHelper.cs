using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IndicatorsUI.MainUI.Helpers
{
    public class UrlHelper
    {
        public static string CombineUrl(params string [] urlParts)
        {
            StringBuilder sb = new StringBuilder(urlParts[0].TrimEnd('/'));

            for (int i = 1; i < urlParts.Length; i++)
            {
                var urlPart = urlParts[i];

                if (string.IsNullOrWhiteSpace(urlPart) == false)
                {
                    // Trim leading '/' if present
                    var trimmedPart = urlPart.Trim('/');
                    sb.Append("/");
                    sb.Append(trimmedPart);
                }
            }

            return sb.ToString();
        }
    }
}