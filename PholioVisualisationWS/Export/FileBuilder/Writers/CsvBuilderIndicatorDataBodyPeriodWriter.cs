using System;
using System.Collections.Generic;
using NHibernate.Util;
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

        public CsvBuilderIndicatorDataBodyPeriodWriter(IndicatorMetadata indicatorMetadata, Grouping grouping, ExportAreaHelper areaHelper, IGroupDataReader groupDataReader, IAreasReader areasReader,
            IndicatorExportParameters generalParameters, OnDemandQueryParametersWrapper onDemandQueryParameters, IndicatorComparer comparer = null)
        {
            _areaHelper = areaHelper;

            _timePeriodFormatter = new TimePeriodTextFormatter(indicatorMetadata);
            _timePeriods = grouping.GetTimePeriodIterator(indicatorMetadata.YearType).TimePeriods;

            // Data collectors for calculating recent trends
            _coreDataCollector = new MultipleCoreDataCollector();

            _bodyPeriodWriterContainer = new BodyPeriodWriterContainer(generalParameters, onDemandQueryParameters, indicatorMetadata,
                areaHelper, groupDataReader, areasReader, _timePeriods, _coreDataCollector, grouping, comparer);
        }

        public void WritePeriodsForIndicatorBodyInFile(int indicatorId, SingleIndicatorFileWriter fileWriter, Grouping grouping)
        {
            foreach (var timePeriod in _timePeriods)
            {
                WriteSinglePeriodInFile(indicatorId, timePeriod, fileWriter, grouping);
            }
        }

        private void WriteSinglePeriodInFile(int indicatorId, TimePeriod timePeriod, SingleIndicatorFileWriter fileWriter, Grouping grouping)
        {
            var timeString = _timePeriodFormatter.Format(timePeriod);
            var sortableTimePeriod = timePeriod.ToSortableNumber();

            try
            {
                // Write England data
                IList<CoreDataSet> englandCoreDataForComparison = null;
                var englandCoreData = _bodyPeriodWriterContainer.WriteProcessedNationalData(indicatorId, timePeriod, timeString,
                    sortableTimePeriod, fileWriter, ref englandCoreDataForComparison, grouping, _areaHelper.England.Code);

                // Write parent data
                var parentCoreDataForComparison = BodyPeriodComparisonContainer.CloneCoreDataForComparison(englandCoreDataForComparison);
                var parentCoreData = _bodyPeriodWriterContainer.WriteProcessedSubNationalData(indicatorId, timePeriod,
                    timeString, sortableTimePeriod, fileWriter, ref parentCoreDataForComparison, grouping);

                // Write child data
                var childCoreData = _bodyPeriodWriterContainer.WriteChildProcessedData(indicatorId, timePeriod,
                    timeString, sortableTimePeriod, fileWriter, englandCoreDataForComparison,
                    parentCoreDataForComparison, grouping);

                // Collect data for trends calculations
                _coreDataCollector.AddEnglandDataList(englandCoreData);
                _coreDataCollector.AddParentDataList(parentCoreData);
                _coreDataCollector.AddChildDataList(childCoreData);
            }
            catch (Exception ex)
            {
                throw new FingertipsException(
                    string.Format(
                        "CSV data could not be built for indicator {0} and time period {1}.{2}Additional information: {3}",
                        indicatorId, timePeriod, Environment.NewLine, ex.Message), ex);
            }
        }
    }
}