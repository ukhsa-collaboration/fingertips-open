
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupData
    {
        public IList<IArea> Areas { get; set; }

        /// <summary>
        /// Public list used in compiling JSON
        /// </summary>
        public IList<IndicatorMetadata> IndicatorMetadata
        {
            get
            {
                return metadataCollection.IndicatorMetadata;
            }
        }

        public IList<GroupRoot> GroupRoots { get; set; }

        private IndicatorMetadataCollection metadataCollection;

        public bool IsDataOk
        {
            get { return Areas != null && GroupRoots != null && GroupRoots.Count > 0; }
        }

        public void InitIndicatorMetadata(IndicatorMetadataCollection metadataCollection)
        {
            this.metadataCollection = metadataCollection;
        }

        public IndicatorMetadata GetIndicatorMetadataById(int indicatorId)
        {
            return metadataCollection.GetIndicatorMetadataById(indicatorId);
        }

        public void Clear()
        {
            GroupRoots = new List<GroupRoot>();

            IndicatorMetadataCollection indicatorMetadataCollection =
                IndicatorMetadataProvider.Instance.GetIndicatorMetadataCollection(new List<Grouping>(), ProfileIds.Undefined);

            InitIndicatorMetadata(indicatorMetadataCollection);

            Areas = new List<IArea>();
        }

        public bool CanBeCached
        {
            get { return IsDataOk; }
        }
    }
}