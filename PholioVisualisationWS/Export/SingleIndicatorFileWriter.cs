using PholioVisualisation.Analysis;
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
        private readonly IndicatorExportParameters _parameters;
        private IDateChangeHelper _dateChangeHelper = new DateChangeHelper(new MonthlyReleaseHelper(), new CoreDataAuditRepository(), new CoreDataSetRepository());
        private ProfileConfig _profileConfig;

        // Indicator specific varibles
        private IndicatorMetadata _indicatorMetadata;
        private readonly Dictionary<string, IndicatorDateChange> _timePeriodToDataChanges = new Dictionary<string, IndicatorDateChange>();
        private TargetComparer _targetComparer;

        private const string UnavailableInfo = "Unavailable info";

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
            SignificanceFormatter significanceFormatter, IndicatorMetadata indicatorMetadata)
        {
            _lookUpManager = lookUpManager;
            _trendMarkerLabelProvider = trendMarkerLabelProvider;
            _significanceFormatter = significanceFormatter;
            _indicatorMetadata = indicatorMetadata;
            InitTargetComparer();
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

        public void WriteData(CoreDataSet data, string timePeriod,
            IArea parentArea, TrendMarkerResult trendMarkerResult, Significance comparedToEnglandSignificance,
            Significance comparedToSubnationalParentSignificance, int? sortableTimePeriod)
        {
            var formatter = new CoreDataSetExportFormatter(_lookUpManager, data);
            var areaCode = data.AreaCode;

            var trendMarkerLabel = GetTrendMarkerLabel(trendMarkerResult);
            var includeParentDetails = parentArea != null & Area.IsAreaListArea(parentArea) == false;

            var items = new List<object>
            {
                _indicatorMetadata.IndicatorId,
                _indicatorMetadata.Name,
                includeParentDetails ? parentArea.Code : "",
                includeParentDetails ? parentArea.Name: "",
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
            var changeText = _profileConfig != null ? GetDateChange(data, timePeriod).HasDataChangedRecently ? "New data" : string.Empty : UnavailableInfo;
            
            items.Add(changeText);

            // Add target data 
            if (_targetComparer != null)
            {
                var significance = _targetComparer.CompareAgainstTarget(data);
                items.Add(_significanceFormatter.GetTargetLabel(significance));
            }

            if (changeText == UnavailableInfo)
            {
                items.Add("Notice: If new data field info is required, please add &profile_id=NUMBER at the url end, replacing NUMBER word for the corresponding number of profile id");
            }

            _csvWriter.AddLine(items.ToArray());
        }

        private string GetTrendMarkerLabel(TrendMarkerResult trendMarkerResult)
        {
            var trendMarkerLabel = trendMarkerResult != null
                ? _trendMarkerLabelProvider.GetLabel(trendMarkerResult.Marker).Text
                : "";
            return trendMarkerLabel;
        }

        private void InitTargetComparer()
        {
            if (_indicatorMetadata.TargetId != null)
            {
                _targetComparer = TargetComparerFactory.New(_indicatorMetadata.TargetConfig);
            }
        }

        private bool UseFileCache
        {
            get
            {
                return ApplicationConfiguration.Instance.UseFileCache;
            }
        }

        private IndicatorDateChange GetDateChange(CoreDataSet data, string timePeriod)
        {
            if (!_timePeriodToDataChanges.ContainsKey(timePeriod))
            {
                var dataChange = _dateChangeHelper.GetIndicatorDateChange(data.GetTimePeriod(),
                    _indicatorMetadata, _profileConfig.NewDataDeploymentCount);

                _timePeriodToDataChanges.Add(timePeriod, dataChange);

                return dataChange;
            }

            return _timePeriodToDataChanges[timePeriod];
        }
    }
}