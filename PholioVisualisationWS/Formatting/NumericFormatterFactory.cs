using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Formatting
{
    public class NumericFormatterFactory
    {
        /// <summary>
        /// New numeric formatter instance.
        /// </summary>
        public static NumericFormatter New(IndicatorMetadata metadata, IGroupDataReader groupDataReader)
        {
            var limits = groupDataReader.GetCoreDataLimitsByIndicatorId(metadata.IndicatorId);
            return NewWithLimits(metadata, limits);
        }

        /// <summary>
        /// New numeric formatter instance.
        /// </summary>
        /// <param name="metadata">Indicator metadata of data to be formatted.</param>
        /// <param name="limits">May be null if fixed decimal place is specified in metadata.</param>
       public static NumericFormatter NewWithLimits(IndicatorMetadata metadata, Limits limits)
        {
            int? decimalPlacesDisplayed = metadata.DecimalPlacesDisplayed;
            if (decimalPlacesDisplayed.HasValue && decimalPlacesDisplayed.Value >= 0)
            {
                return new FixedDecimalPlaceFormatter(decimalPlacesDisplayed.Value);
            }

            switch (metadata.ValueTypeId)
            {
                case 5:
                    return new ProportionFormatter(metadata, limits);
                default:
                    return new DefaultFormatter(metadata, limits);
            }
        }
    }
}
