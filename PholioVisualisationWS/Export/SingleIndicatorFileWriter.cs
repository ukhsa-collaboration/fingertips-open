using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export.File;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

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

        public SingleIndicatorFileWriter(int indicatorId, IndicatorExportParameters parameters)
        {
            _fileName = CacheFileNamer.GetIndicatorFileName(
                indicatorId, parameters.ParentAreaCode,
                parameters.ParentAreaTypeId,
                parameters.ChildAreaTypeId,
                parameters.ProfileId);
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
            var useFileCache = ApplicationConfiguration.UseFileCache;
            if (useFileCache)
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
            var useFileCache = ApplicationConfiguration.UseFileCache;
            if (useFileCache)
            {
                _exportFileManager.SaveFile(contents);
            }

            return contents;
        }

        public void WriteData(IndicatorMetadata indicatorMetadata, CoreDataSet data, string timePeriod,
            IArea parentArea, TrendMarkerResult trendMarkerResult, Significance comparedToEnglandSignificance,
            Significance comparedToSubnationalParentSignificance, int? sortableTimePeriod = null)
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
                formatter.LowerCI,
                formatter.UpperCI,
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

            _csvWriter.AddLine(items.ToArray());
        }
    }
}