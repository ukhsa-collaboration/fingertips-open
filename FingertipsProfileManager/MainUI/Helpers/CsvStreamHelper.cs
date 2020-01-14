using CsvHelper;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using System.Collections.Generic;
using System.IO;

namespace Fpm.MainUI.Helpers
{
    public static class CsvStreamHelper
    {
        private const string Quote = "\"";
        private const string EscapedQuote = "\"\"";
        private static readonly char[] CharactersThatMustBeQuoted = { ',', '"', '\n' };

        public static byte[] GetCsvDataForMetadataDownload(List<IndicatorMetadata> indicatorMetadataList)
        {
            var reader = ReaderFactory.GetProfilesReader();
            var indicatorMetadataTextProperties = reader.GetIndicatorMetadataTextProperties();

            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var csvWriter = new CsvSerializer(streamWriter))
                    {
                        var isHeaderRow = true;
                        foreach (var indicatorMetadata in indicatorMetadataList)
                        {
                            var indicatorMetadataTextValues = (List<IndicatorText>) reader.GetIndicatorTextValues(
                                indicatorMetadata.IndicatorId,
                                indicatorMetadataTextProperties, indicatorMetadata.OwnerProfileId);

                            if (isHeaderRow)
                            {
                                var displayNames = new List<string>();
                                foreach (var indicatorMetadataTextValue in indicatorMetadataTextValues)
                                {
                                    displayNames.Add(indicatorMetadataTextValue.IndicatorMetadataTextProperty.DisplayName);
                                }

                                csvWriter.Write(displayNames.ToArray());
                                csvWriter.WriteLine();

                                isHeaderRow = false;
                            }

                            var displayValues = new List<string>();
                            foreach (var indicatorMetadataTextValue in indicatorMetadataTextValues)
                            {
                                displayValues.Add(Escape(indicatorMetadataTextValue.ValueGeneric));
                            }

                            csvWriter.Write(displayValues.ToArray());
                            csvWriter.WriteLine();
                        }

                        streamWriter.Flush();

                        return memoryStream.ToArray();
                    }
                }
            }
        }

        private static string Escape(string item)
        {
            if (item == null)
            {
                return item;
            }

            if (item.Contains(Quote))
            {
                item = item.Replace(Quote, EscapedQuote);
            }

            if (item.IndexOfAny(CharactersThatMustBeQuoted) > -1)
            {
                item = Quote + item + Quote;
            }

            return item;
        }
    }
}
