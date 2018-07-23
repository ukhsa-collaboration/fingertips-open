using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Formatting
{
    public interface INumericFormatterFactory
    {
        /// <summary>
        /// New numeric formatter instance.
        /// </summary>
        NumericFormatter New(IndicatorMetadata metadata);

        /// <summary>
        /// New numeric formatter instance.
        /// </summary>
        /// <param name="metadata">Indicator metadata of data to be formatted.</param>
        /// <param name="limits">May be null if fixed decimal place is specified in metadata.</param>
        NumericFormatter NewWithLimits(IndicatorMetadata metadata, Limits limits);
    }

    public class NumericFormatterFactory : INumericFormatterFactory
    {
        private IGroupDataReader _groupDataReader;

        public NumericFormatterFactory(IGroupDataReader groupDataReader)
        {
            _groupDataReader = groupDataReader;
        }

        /// <summary>
        /// New numeric formatter instance.
        /// </summary>
        public NumericFormatter New(IndicatorMetadata metadata)
        {
            var limits = _groupDataReader.GetCoreDataLimitsByIndicatorId(metadata.IndicatorId);
            return NewWithLimits(metadata, limits);
        }

        /// <summary>
        /// New numeric formatter instance.
        /// </summary>
        /// <param name="metadata">Indicator metadata of data to be formatted.</param>
        /// <param name="limits">May be null if fixed decimal place is specified in metadata.</param>
       public NumericFormatter NewWithLimits(IndicatorMetadata metadata, Limits limits)
        {
            if (metadata.IndicatorId == IndicatorIds.SuicidePreventionPlan)
            {
                return new SuicidePreventionPlanFormatter();
            }

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
