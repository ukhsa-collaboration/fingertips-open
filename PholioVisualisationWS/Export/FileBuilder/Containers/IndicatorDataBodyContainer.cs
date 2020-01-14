using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export.FileBuilder.Writers;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export.FileBuilder.Containers
{
    public class IndicatorDataBodyContainer
    {
        private readonly IndicatorMetadataProvider _indicatorMetadataProvider;
        private readonly ExportAreaHelper _areaHelper;
        private readonly IAreasReader _areasReader;
        private readonly PholioReader _pholioReader = ReaderFactory.GetPholioReader();
        private readonly CsvBuilderAttributesForBodyContainer _attributesForBodyContainer;

        private CsvBuilderIndicatorDataBodyPeriodWriter _bodyPeriodWriter;
        private LookUpManager _lookUpManager;
        private SingleIndicatorFileWriter fileWriter;

        public IndicatorDataBodyContainer(IndicatorMetadataProvider indicatorMetadataProvider, ExportAreaHelper areaHelper,
            IAreasReader areasReader, CsvBuilderAttributesForBodyContainer attributesForBodyContainer)
        {
            _indicatorMetadataProvider = indicatorMetadataProvider;
            _areaHelper = areaHelper;
            _areasReader = areasReader;
            _attributesForBodyContainer = attributesForBodyContainer;
        }

        public IndicatorComparer AssistanceComparer { get; private set; }

        public byte[] GetIndicatorDataFile(int indicatorId)
        {
            byte[] content;
            fileWriter = new SingleIndicatorFileWriter(indicatorId, _attributesForBodyContainer);
            var shouldUseCache = ShouldUseFileCache(_attributesForBodyContainer.GeneralParameters.ParentAreaCode);

            // Read file from cache
            if (shouldUseCache)
            {
                content = TryReadFileFromCache();
                if (content != null)
                {
                    return content;
                }
            }

            // Build data file
            Grouping grouping;
            IndicatorMetadata indicatorMetadata;
            if (HasBeenIgnoreFileWriterSettings(indicatorId, out grouping, out indicatorMetadata))
            {
                // Empty file
                return new byte[] { };
            }

            WriteIndicatorDataBodyInFile(indicatorId, indicatorMetadata, grouping);

            content = fileWriter.GetFileContent();

            // Write file to cache
            if (shouldUseCache)
            {
                fileWriter.SaveFile(content);
            }

            return content;
        }

        private void WriteIndicatorDataBodyInFile(int indicatorId, IndicatorMetadata indicatorMetadata, Grouping grouping)
        {
            _bodyPeriodWriter = new CsvBuilderIndicatorDataBodyPeriodWriter(indicatorMetadata, grouping, _areaHelper,
                _attributesForBodyContainer.GroupDataReader, _areasReader, _attributesForBodyContainer.GeneralParameters,
                _attributesForBodyContainer.OnDemandParameters, AssistanceComparer);

            _bodyPeriodWriter.WritePeriodsForIndicatorBodyInFile(indicatorId, fileWriter, grouping);
        }

        private bool ShouldUseFileCache(string parentAreaCode)
        {
            // File download related to area list must never be from cache
            if (Area.IsAreaListAreaCode(parentAreaCode))
            {
                return false;
            }

            return fileWriter.UseFileCache;
        }

        private byte[] TryReadFileFromCache()
        {
            return fileWriter.TryLoadFile();
        }

        private bool HasBeenIgnoreFileWriterSettings(int indicatorId,
            out Grouping grouping, out IndicatorMetadata indicatorMetadata)
        {
            // Ignore the indicator if no grouping found
            if (!HasGroupingAndIndicatorMetadata(indicatorId, out grouping, out indicatorMetadata)) return true;

            // Lazy initialisation (would not be necessary if cached files are available)
            _areaHelper.Init();

            InitFileWriter(grouping, indicatorMetadata);
            InitComparer(grouping);

            return false;
        }

        private bool HasGroupingAndIndicatorMetadata(int indicatorId, out Grouping grouping, out IndicatorMetadata indicatorMetadata)
        {
            indicatorMetadata = null;
            if (!_attributesForBodyContainer.HasGrouping(indicatorId, _attributesForBodyContainer.OnDemandParameters.GroupIds, out grouping))
            {
                return false;
            }

            indicatorMetadata = FindIndicatorMetadata(grouping);
            return true;
        }

        private void InitFileWriter(Grouping grouping, IndicatorMetadata indicatorMetadata)
        {
            var trendMarkerLabelProvider = new TrendMarkerLabelProvider(grouping.PolarityId);
            var significanceFormatter = new SignificanceFormatter(grouping.PolarityId, grouping.ComparatorMethodId);
            fileWriter.Init(LookUpManager, trendMarkerLabelProvider, significanceFormatter, indicatorMetadata);
        }

        private void InitComparer(Grouping grouping)
        {
            AssistanceComparer = null;
            if (grouping.IsComparatorDefined())
            {
                AssistanceComparer = new IndicatorComparerFactory { PholioReader = _pholioReader }.New(grouping);
            }
        }

        private IndicatorMetadata FindIndicatorMetadata(Grouping grouping)
        {
            var indicatorMetadataTextOption =
                _attributesForBodyContainer.AttributesForIndicators.IndicatorMetadataTextOption;

            var profileId = _attributesForBodyContainer.OnDemandParameters.ProfileId;

            return _indicatorMetadataProvider.GetIndicatorMetadata(new List<Grouping> { grouping }, indicatorMetadataTextOption, profileId).First();
        }

        /// <summary>
        /// Lazily initialise look up manager. May not need these if all files are already cached.
        /// </summary>
        private LookUpManager LookUpManager
        {
            get
            {
                if (_lookUpManager == null)
                {
                    // Init look up manager
                    var allAreaTypes = new List<int>
                    {
                        AreaTypeIds.Country,
                        _attributesForBodyContainer.GeneralParameters.ParentAreaTypeId,
                        _attributesForBodyContainer.GeneralParameters.ChildAreaTypeId
                    };
                    string publicId = null;
                    if (Area.IsAreaListAreaCode(_attributesForBodyContainer.GeneralParameters.ParentAreaCode))
                    {
                        publicId = _attributesForBodyContainer.GeneralParameters.ParentAreaCode;
                    }
                    var categoryTypeIds = _areasReader.GetAllCategoryTypes().Select(x => x.Id).ToList();
                    _lookUpManager = new LookUpManager(_pholioReader, _areasReader, allAreaTypes, categoryTypeIds, publicId);
                }
                return _lookUpManager;
            }
        }
    }

}