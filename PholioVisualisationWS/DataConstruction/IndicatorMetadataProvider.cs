using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PholioVisualisation.DataConstruction
{
    public enum IndicatorMetadataTextOptions
    {
        OverrideGenericWithProfileSpecific,
        UseGeneric
    }

    public class IndicatorMetadataProvider
    {
        private IGroupDataReader reader = ReaderFactory.GetGroupDataReader();

        private List<IndicatorMetadataTextProperty> properties;

        public string[] TruncatedPropertyNames = new[] {
                    IndicatorMetadataTextColumnNames.Name,
                    IndicatorMetadataTextColumnNames.NameLong,
                    IndicatorMetadataTextColumnNames.DataSource
                };

        /// <summary>
        /// Private constructor
        /// </summary>
        private IndicatorMetadataProvider() { }

        public static IndicatorMetadataProvider Instance
        {
            get
            {
                //No cache option
                return new IndicatorMetadataProvider();
            }
        }

        public IList<IndicatorMetadataTextProperty> IndicatorMetadataTextProperties
        {
            get
            {
                if (properties == null)
                {
                    properties = reader.GetIndicatorMetadataTextProperties().ToList();
                }

                // Should be a deep copy if expect any changes
                return properties.AsReadOnly();
            }
        }

        public IList<IndicatorMetadata> GetIndicatorMetadata(IList<Grouping> groupings, IndicatorMetadataTextOptions option)
        {
            IList<IndicatorMetadata> indicatorMetadataList = reader.GetIndicatorMetadata(groupings, IndicatorMetadataTextProperties);

            if (option == IndicatorMetadataTextOptions.OverrideGenericWithProfileSpecific)
            {
                OverrideGenericTextMetadata(groupings, indicatorMetadataList);
            }

            return indicatorMetadataList;
        }

        public IndicatorMetadata GetIndicatorMetadata(Grouping grouping)
        {
            IndicatorMetadata indicatorMetadata = reader.GetIndicatorMetadata(grouping, IndicatorMetadataTextProperties);
            OverrideGenericTextMetadata(
                new List<Grouping> { grouping },
                new List<IndicatorMetadata> { indicatorMetadata });
            return indicatorMetadata;
        }

        public IList<IndicatorMetadata> GetIndicatorMetadata(IList<int> indicatorIds)
        {
            return reader.GetIndicatorMetadata(indicatorIds, IndicatorMetadataTextProperties);
        }

        public IndicatorMetadata GetIndicatorMetadata(int indicatorId)
        {
            return reader.GetIndicatorMetadata(indicatorId, IndicatorMetadataTextProperties);
        }

        public IndicatorMetadataCollection GetIndicatorMetadataCollection(IList<Grouping> groupings)
        {
            return new IndicatorMetadataCollection(
                GetIndicatorMetadata(groupings, IndicatorMetadataTextOptions.OverrideGenericWithProfileSpecific));
        }

        public IndicatorMetadataCollection GetIndicatorMetadataCollection(Grouping grouping)
        {
            return GetIndicatorMetadataCollection(new List<Grouping> { grouping });
        }

        private void OverrideGenericTextMetadata(IList<Grouping> groupings, IList<IndicatorMetadata> genericIndicatorMetadata)
        {
            IList<IndicatorMetadata> specificIndicatorMetadataList = reader.GetGroupSpecificIndicatorMetadataTextValues(
                groupings, IndicatorMetadataTextProperties);

            if (specificIndicatorMetadataList.Count > 0)
            {
                Dictionary<int, IndicatorMetadata> metadataMap =
                    genericIndicatorMetadata.ToDictionary(indicatorMetadata => indicatorMetadata.IndicatorId);

                foreach (var specificIndicatorMetadata in specificIndicatorMetadataList)
                {
                    int indicatorId = specificIndicatorMetadata.IndicatorId;

                    if (metadataMap.ContainsKey(indicatorId))
                    {
                        IDictionary<string, string> genericText = metadataMap[indicatorId].Descriptive;
                        IDictionary<string, string> specificText = specificIndicatorMetadata.Descriptive;

                        foreach (var key in specificText.Keys)
                        {
                            if (genericText.ContainsKey(key))
                            {
                                genericText[key] = specificText[key];
                            }
                            else
                            {
                                genericText.Add(key, specificText[key]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Replaces the complete descriptive metadata with one where
        /// only selected properties are retained, e.g. Name, NameLong, Source
        /// </summary>
        public void ReduceDescriptiveMetadata(IList<IndicatorMetadata> indicatorMetadataList)
        {
            foreach (var indicatorMetadata in indicatorMetadataList)
            {
                IDictionary<string, string> fullDescriptive = indicatorMetadata.Descriptive;

                IDictionary<string, string> truncatedDescriptive = new Dictionary<string, string>();

                foreach (var propertyName in TruncatedPropertyNames)
                {
                    if (fullDescriptive.ContainsKey(propertyName))
                    {
                        truncatedDescriptive[propertyName] = fullDescriptive[propertyName];
                    }
                }

                indicatorMetadata.Descriptive = truncatedDescriptive;
            }
        }

        /// <summary>
        ///Excludes all system content fields
        /// </summary>
        public void RemoveSystemContentFields(IList<IndicatorMetadata> indicatorMetadataList)
        {
            foreach (var indicatorMetadata in indicatorMetadataList)
            {
                IDictionary<string, string> removedSystemContentDescriptive = indicatorMetadata.Descriptive;

                var systemContentTextProperties = properties.Where(x => x.IsSystemContent);

                foreach (var indicatorMetadataTextProperty in systemContentTextProperties)
                {
                    var propertyName = indicatorMetadataTextProperty.ColumnName;
                    if (removedSystemContentDescriptive.ContainsKey(propertyName))
                    {
                        removedSystemContentDescriptive.Remove(propertyName);
                    }
                }

                indicatorMetadata.Descriptive = removedSystemContentDescriptive;
            }
        }

        public void CorrectLocalDocumentLink(IList<IndicatorMetadata> indicatorMetadataList)
        {
            foreach (var indicatorMetadata in indicatorMetadataList)
            {
                var baseUrl = "href=\"" + ApplicationConfiguration.UrlUI + @"/documents";
                var description = indicatorMetadata.Descriptive;
                var searchString = @"href=""documents";
                foreach (var item in description.Keys.ToList())
                {

                    description[item] = Regex.Replace(description[item], searchString, baseUrl);
                }
            }
        }
    }
}