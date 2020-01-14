using Fpm.ProfileData;
using System;
using System.Collections.Generic;

namespace Fpm.MainUI.Helpers
{
    public class IndicatorMetadataTextParser : IIndicatorMetadataTextParser
    {
        public const char Separator = '¬';

        public IList<IndicatorMetadataTextItem> Parse(string concatenatedMetadataProperties)
        {
            var items = new List<IndicatorMetadataTextItem>();

            var bits = concatenatedMetadataProperties.Split(Separator);

            for (int i = 0; i < bits.Length; i += 2)
            {
                var item = new IndicatorMetadataTextItem();
                items.Add(item);

                string idString = bits[i];

                if (idString.EndsWith("o"))
                {
                    idString = idString.TrimEnd('o');
                    item.IsBeingOverriddenForFirstTime = true;
                }

                item.PropertyId = Int32.Parse(idString);

                var text = bits[i + 1];
                try
                {
                    item.Text = Uri.UnescapeDataString(text);
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(
                        new FpmException("Failed to parse indicator metadata: '" + text + "'", ex), null);
                    throw;
                }
            }

            return items;
        }
    }
}