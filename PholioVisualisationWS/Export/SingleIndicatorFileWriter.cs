using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export.File;
using PholioVisualisation.Export.FileBuilder.Containers;
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
        private readonly CsvBuilderAttributesForBodyContainer _parameters;
        private IDateChangeHelper _dateChangeHelper = new DateChangeHelper(new MonthlyReleaseHelper(),
            new CoreDataAuditRepository(), new CoreDataSetRepository());
        private ProfileConfig _profileConfig;

        // Indicator specific variables
        private IndicatorMetadata _indicatorMetadata;
        private readonly Dictionary<string, IndicatorDateChange> _timePeriodToDataChanges = new Dictionary<string, IndicatorDateChange>();
        private TargetComparer _targetComparer;

        public SingleIndicatorFileWriter(int indicatorId, CsvBuilderAttributesForBodyContainer parameters)
        {
            _fileName = CacheFileNamer.GetIndicatorFileName(indicatorId, parameters);

            _parameters = parameters;
            _profileConfig = ReaderFactory.GetProfileReader().GetProfileConfig(_parameters.GeneralParameters.ProfileId);
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

        public void SaveFile(byte[] contents)
        {
            if (UseFileCache)
            {
                _exportFileManager.SaveFile(contents);
            }
        }

        public byte[] GetFileContent()
        {
            var contents = _csvWriter.WriteAsBytes();
            return contents;
        }

        public void WriteData(CoreDataSet data, string timePeriod,
            IArea parentArea, TrendMarkerResult trendMarkerResult, Significance comparedToEnglandSignificance,
            Significance comparedToSubnationalParentSignificance, int? sortableTimePeriod, Grouping grouping)
        {
            var formatter = new CoreDataSetExportFormatter(_lookUpManager, data);
            var areaCode = data.AreaCode;

            var trendMarkerLabel = GetTrendMarkerLabel(trendMarkerResult);
            var includeParentDetails = parentArea != null & Area.IsAreaListArea(parentArea) == false;

            var areaName = string.Empty;
            var areaTypeName = string.Empty;

            if (NearestNeighbourArea.IsNearestNeighbourAreaCode(areaCode))
            {
                areaCode = NearestNeighbourArea.GetAreaCodeFromNeighbourAreaCode(areaCode);
                areaName = "Nearest neighbours of ";
            }

            if (ExportAreaHelper.IsCategoryAreaCode(new[] { areaCode }))
            {
                var categoryAreaCode = ExportAreaHelper.GetCategoryInequalityFromAreaCode(areaCode, ReaderFactory.GetAreasReader());
                areaName = _lookUpManager.GetCategoryName(categoryAreaCode.CategoryTypeId, categoryAreaCode.CategoryId);
            }
            else
            {
                areaName = areaName + _lookUpManager.GetAreaName(areaCode);
                areaTypeName = _lookUpManager.GetAreaTypeName(areaCode);
            }

            var items = new List<object>
            {
                _indicatorMetadata.IndicatorId,
                _indicatorMetadata.Name,
                includeParentDetails ? parentArea.Code : "",
                includeParentDetails ? parentArea.Name: "",
                areaCode,
                areaName,
                areaTypeName,
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

            // Add sortable time period 
            var sortableTimePeriodText = sortableTimePeriod.HasValue ? sortableTimePeriod.Value.ToString() : string.Empty;
            items.Add(sortableTimePeriodText);

            // New data
            var changeText = GetNewDataText(data, timePeriod, grouping.AreaTypeId);
            items.Add(changeText);

            // Add target data 
            var targetText = GetTargetText(data);
            items.Add(targetText);

            _csvWriter.AddLine(items.ToArray());
        }

        public bool UseFileCache
        {
            get
            {
                return ApplicationConfiguration.Instance.UseFileCache;
            }
        }

        private string GetTargetText(CoreDataSet data)
        {
            string targetText;
            if (_targetComparer != null)
            {
                var significance = _targetComparer.CompareAgainstTarget(data);
                targetText = _significanceFormatter.GetTargetLabel(significance);
            }
            else
            {
                targetText = string.Empty;
            }

            return targetText;
        }

        private string GetNewDataText(CoreDataSet data, string timePeriod, int areaTypeId)
        {
            string changeText;
            if (_profileConfig == null) //TODO use search config instead
            {
                changeText = string.Empty;
            }
            else
            {
                changeText = GetDateChange(data, timePeriod, areaTypeId).HasDataChangedRecently ? "New data" : string.Empty;
            }

            return changeText;
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

        private IndicatorDateChange GetDateChange(CoreDataSet data, string timePeriod, int areaTypeId)
        {
            if (!_timePeriodToDataChanges.ContainsKey(timePeriod))
            {
                var dataChange = _dateChangeHelper.GetIndicatorDateChange(_indicatorMetadata,
                    _profileConfig.NewDataDeploymentCount, areaTypeId);

                _timePeriodToDataChanges.Add(timePeriod, dataChange);

                return dataChange;
            }

            return _timePeriodToDataChanges[timePeriod];
        }
    }
}