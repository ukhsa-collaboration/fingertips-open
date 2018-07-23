using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    /// Base class for classes that provides collections of data objects.
    /// </summary>
    public abstract class AreaValuesBuilder
    {
        public int AreaTypeId { get; set; }
        public string ParentAreaCode { get; set; }
        public int DataPointOffset { get; set; }

        protected IGroupDataReader groupDataReader;
        protected Grouping Grouping;
        protected IndicatorMetadata IndicatorMetadata;
        protected NumericFormatter Formatter;
        protected TimePeriod Period;

        protected AreaValuesBuilder(IGroupDataReader groupDataReader)
        {
            this.groupDataReader = groupDataReader;
        }

        protected NumericFormatter GetFormatter()
        {
            return new NumericFormatterFactory(groupDataReader).New(IndicatorMetadata);
        }

        protected void InitBuild(Grouping grouping)
        {
            if (grouping != null)
            {
                Grouping = grouping;
                IndicatorMetadata = IndicatorMetadataProvider.Instance.GetIndicatorMetadata(grouping.IndicatorId);
                Period = new DataPointOffsetCalculator(grouping, DataPointOffset, IndicatorMetadata.YearType).TimePeriod;
                Formatter = GetFormatter();
            }
        }

    }
}
