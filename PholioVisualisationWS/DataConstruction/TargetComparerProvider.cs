using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class TargetComparerProvider
    {
        private IGroupDataReader _groupDataReader;
        private IAreasReader _areasReader;

        private IArea _england;

        public TargetComparerProvider(IGroupDataReader groupDataReader, IAreasReader areasReader)
        {
            _groupDataReader = groupDataReader;
            _areasReader = areasReader;
        }

        /// <summary>
        /// Gets a target comparer with any required data already assigned.
        /// </summary>
        /// <param name="indicatorMetadata">Indicator metadata</param>
        /// <param name="grouping">Grouping</param>
        /// <param name="timePeriod">Optional: grouping datapoint will be used if parameter is not passed</param>
        public TargetComparer GetTargetComparer(IndicatorMetadata indicatorMetadata, Grouping grouping, TimePeriod timePeriod = null)
        {  
            var targetComparer = TargetComparerFactory.New(indicatorMetadata.TargetConfig);

            if (targetComparer != null)
            {
                // Ensure time period is set
                if (timePeriod == null)
                {
                    timePeriod = TimePeriod.GetDataPoint(grouping);
                }

                new TargetComparerDataAssigner(_groupDataReader)
                    .AssignExtraDataIfRequired(England, targetComparer, grouping, indicatorMetadata, timePeriod);
            }

            return targetComparer;
        }

        private IArea England
        {
            get
            {
                if (_england == null)
                {
                    _england = _areasReader.GetAreaFromCode(AreaCodes.England);
                }
                return _england;
            }
        }
    }
}