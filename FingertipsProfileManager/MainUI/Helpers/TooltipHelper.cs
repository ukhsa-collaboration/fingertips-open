using System.Collections.Generic;

namespace Fpm.MainUI.Helpers
{
    /// <summary>
    /// Bootstrap tooltip helper
    /// </summary>
    public class TooltipHelper
    {
        public static IDictionary<string, object> GetTooltipRight(string message)
        {
            var properties = new Dictionary<string, object>();
            AddTooltipRight(properties, message);
            return properties;
        }

        public static void AddTooltipRight(IDictionary<string, object> properties, string message)
        {
            properties.Add("data-toggle", "tooltip");
            properties.Add("data-placement", "right");
            properties.Add("title", message);
        }
    }
}