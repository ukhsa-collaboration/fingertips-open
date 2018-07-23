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

        public static void AddTooltipLeft(IDictionary<string, object> properties, string message)
        {
            AddTooltip(properties, message, "left");
        }

        public static void AddTooltipRight(IDictionary<string, object> properties, string message)
        {
            AddTooltip(properties, message, "right");
        }

        public static void AddTooltipBottom(IDictionary<string, object> properties, string message)
        {
            AddTooltip(properties, message, "bottom");
        }

        private static void AddTooltip(IDictionary<string, object> properties, string message, string placement)
        {
            properties.Add("data-toggle", "tooltip");
            properties.Add("data-placement", placement);
            properties.Add("title", message);
        }
    }
}