using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class IndicatorMetadataCollection
    {
        /// <summary>
        /// Public list used in compiling JSON
        /// </summary>
        public List<IndicatorMetadata> IndicatorMetadata { get; private set; }

        private Dictionary<int, IndicatorMetadata> metadataDictionary =
            new Dictionary<int, IndicatorMetadata>();

        /// <summary>
        /// Constructor
        /// </summary>
        public IndicatorMetadataCollection()
        {
            IndicatorMetadata = new List<IndicatorMetadata>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public IndicatorMetadataCollection(IList<IndicatorMetadata> indicatorMetadataList)
        {
            IndicatorMetadata = new List<IndicatorMetadata>();
            AddIndicatorMetadata(indicatorMetadataList);
        }

        public void AddIndicatorMetadata(IList<IndicatorMetadata> indicatorMetadataList)
        {
            foreach (var indicatorMetadata in indicatorMetadataList)
            {
                var id = indicatorMetadata.IndicatorId;
                if (metadataDictionary.ContainsKey(id) == false)
                {
                    metadataDictionary.Add(id, indicatorMetadata);
                    IndicatorMetadata.Add(indicatorMetadata);
                }
            }
        }

        public IndicatorMetadata GetIndicatorMetadataById(int indicatorId)
        {
            if (metadataDictionary != null && metadataDictionary.ContainsKey(indicatorId))
            {
                return metadataDictionary[indicatorId];
            }
            return null;
        }
    }
}
