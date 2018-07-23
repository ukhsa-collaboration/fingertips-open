using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export.File;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace PholioVisualisation.Export
{
    public class SingleIndicatorFileWriter
    {
        private CsvWriter _csvWriter = new CsvWriter();
        private string _fileName;
        private ExportFileManager _exportFileManager;
        private LookUpManager _lookUpManager;
        private TrendMarkerLabelProvider _trendMarkerLabelProvider;
        private SignificanceFormatter _significanceFormatter;
        private IndicatorExportParameters _parameters;
        private DateChangeHelper _dateChangeHelper = new DateChangeHelper(new MonthlyReleaseHelper(), new CoreDataAuditRepository());
        private ProfileConfig _profileConfig;
        private readonly Dictionary<int, IndicatorDateChange> _indicatorDataChanges = new Dictionary<int, IndicatorDateChange>();

        public SingleIndicatorFileWriter(int indicatorId, IndicatorExportParameters parameters)
        {
            _fileName = CacheFileNamer.GetIndicatorFileName(
                indicatorId, parameters.ParentAreaCode,
                parameters.ParentAreaTypeId,
                parameters.ChildAreaTypeId,
                parameters.ProfileId);

            _parameters = parameters;
            _profileConfig = ReaderFactory.GetProfileReader().GetProfileConfig(_parameters.ProfileId);
        }

        public void Init(LookUpManager lookUpManager, TrendMarkerLabelProvider trendMarkerLabelProvider,
            SignificanceFormatter significanceFormatter)
        {
            _lookUpManager = lookUpManager;
            _trendMarkerLabelProvider = trendMarkerLabelProvider;
            _significanceFormatter = significanceFormatter;
        }

        public byte[] TryLoadFile()
        {
            if (UseFileCache)
            {
                // try load indicator file if caching
                _exportFileManager = new ExportFileManager(_fileName);
                return _exportFileManager.TryGetFile();
            }
            return null;
        }

        public byte[] GetFileContent()
        {
            var contents = _csvWriter.WriteAsBytes();
            if (UseFileCache)
            {
                _exportFileManager.SaveFile(contents);
            }

            return contents;
        }

        public void WriteData(IndicatorMetadata indicatorMetadata, CoreDataSet data, string timePeriod,
            IArea parentArea, TrendMarkerResult trendMarkerResult, Significance comparedToEnglandSignificance,
            Significance comparedToSubnationalParentSignificance, int? sortableTimePeriod)
        {
            var formatter = new CoreDataSetExportFormatter(_lookUpManager, data);
            var areaCode = data.AreaCode;

            var trendMarkerLabel = trendMarkerResult != null
                ? _trendMarkerLabelProvider.GetLabel(trendMarkerResult.Marker).Text
                : "";

            var items = new List<object>
            {
                indicatorMetadata.IndicatorId,
                indicatorMetadata.Name,
                parentArea == null ? "" : parentArea.Code,
                parentArea == null ? "" : parentArea.Name,
                areaCode,
                _lookUpManager.GetAreaName(areaCode),
                _lookUpManager.GetAreaTypeName(areaCode),
                _lookUpManager.GetSexName(data.SexId),
                _lookUpManager.GetAgeName(data.AgeId),
                formatter.CategoryType,
                formatter.Category,
                timePeriod,
                formatter.Value,
                formatter.LowerCI95,
                formatter.UpperCI95,
                formatter.LowerCI99_8,
                formatter.UpperCI99_8,
                formatter.Count,
                formatter.Denominator,
                formatter.ValueNote,
                trendMarkerLabel,
                _significanceFormatter.GetLabel(comparedToEnglandSignificance),
                _significanceFormatter.GetLabel(comparedToSubnationalParentSignificance)
            };

            // Add sortable time period only if defined
            if (sortableTimePeriod.HasValue)
            {
                items.Add(sortableTimePeriod);
            }


            // Data Changes   
            if (_profileConfig != null)
            {
                var datachange = GetDataChange(indicatorMetadata);
                var dataChangeText = datachange.HasDataChangedRecently ? "New data" : "";
                items.Add(dataChangeText);
            }
            _csvWriter.AddLine(items.ToArray());
        }

        private bool UseFileCache
        {
            get
            {
                return ApplicationConfiguration.Instance.UseFileCache;
            }
        }

        private IndicatorDateChange GetDataChange(IndicatorMetadata metadata)
        {
            if (!_indicatorDataChanges.ContainsKey(metadata.IndicatorId))
            {
                var dataChange = _dateChangeHelper.AssignDateChange(metadata, _profileConfig.NewDataTimeSpanInDays);
                _indicatorDataChanges.Add(metadata.IndicatorId, dataChange);
                return dataChange;
            }
            return _indicatorDataChanges[metadata.IndicatorId];
        }
    }
}