using System;
using System.Collections.Generic;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export.FileBuilder.Containers;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export.FileBuilder.Writers
{

    public class CsvBuilderIndicatorDataBodyPeriodWriter
    {
        private readonly ExportAreaHelper _areaHelper;
        private readonly TimePeriodTextFormatter _timePeriodFormatter;
        private readonly IList<TimePeriod> _timePeriods;
        private readonly MultipleCoreDataCollector _coreDataCollector;

        private readonly BodyPeriodWriterContainer _bodyPeriodWriterContainer;
        
        public CsvBuilderIndicatorDataBodyPeriodWriter(IndicatorMetadata indicatorMetadata, Grouping grouping, ExportAreaHelper areaHelper, IGroupDataReader groupDataReader,
                                                        IndicatorExportParameters generalParameters, OnDemandQueryParametersWrapper onDemandQueryParameters, IndicatorComparer comparer = null)
        {
            _areaHelper = areaHelper;

            _timePeriodFormatter = new TimePeriodTextFormatter(indicatorMetadata);
            _timePeriods = grouping.GetTimePeriodIterator(indicatorMetadata.YearType).TimePeriods;

            // Data collectors for calculating recent trends
            _coreDataCollector = new MultipleCoreDataCollector();

            _bodyPeriodWriterContainer = new BodyPeriodWriterContainer(generalParameters, onDemandQueryParameters, indicatorMetadata, areaHelper, groupDataReader, _timePeriods, _coreDataCollector, grouping, comparer);
        }

        public void WritePeriodsForIndicatorBodyInFile(int indicatorId, ref SingleIndicatorFileWriter fileWriter)
        {
            foreach (var timePeriod in _timePeriods)
            {
                WriteSinglePeriodInFile(indicatorId, timePeriod, ref fileWriter);
            }
        }

        private void WriteSinglePeriodInFile(int indicatorId, TimePeriod timePeriod, ref SingleIndicatorFileWriter fileWriter)
        {
            var timeString = _timePeriodFormatter.Format(timePeriod);
            var sortableTimePeriod = timePeriod.ToSortableNumber();

            try
            {
                IList<CoreDataSet> englandCoreDataForComparison = null;
                var englandCoreData = _bodyPeriodWriterContainer.WriteProcessedData(indicatorId, timePeriod, timeString, sortableTimePeriod, ref fileWriter,
                                                                                    ref englandCoreDataForComparison, null, _areaHelper.England.Code);

                // This collection into england data list is necessary for trends calculations
                _coreDataCollector.AddEnglandDataList(englandCoreData);

                var parentCoreDataForComparison = BodyPeriodComparisonContainer.CloneCoreDataForComparison(englandCoreDataForComparison);
                var parentCoreData = _bodyPeriodWriterContainer.WriteProcessedData(indicatorId, timePeriod, timeString, sortableTimePeriod, ref fileWriter,
                                                                                    ref parentCoreDataForComparison, _areaHelper.England, _areaHelper.ParentAreaCodes);

                // This collection into parent data list is necessary for trends calculations
                _coreDataCollector.AddParentDataList(parentCoreData);

                // Child data
                var childCoreData = _bodyPeriodWriterContainer.WriteChildProcessedData(indicatorId, timePeriod, timeString, sortableTimePeriod, ref fileWriter, englandCoreDataForComparison, parentCoreDataForComparison);

                _coreDataCollector.AddChildDataList(childCoreData);
            }
            catch (Exception ex)
            {
                throw new FingertipsException(string.Format(
                    "It could not be built data for indicator {0} and time period {1}", indicatorId, timePeriod), ex);
            }
        }
    }
}
