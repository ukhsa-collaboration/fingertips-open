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

        public IndicatorDataBodyContainer(IndicatorMetadataProvider indicatorMetadataProvider, ExportAreaHelper areaHelper, IAreasReader areasReader, CsvBuilderAttributesForBodyContainer attributesForBodyContainer)
        {
            _indicatorMetadataProvider = indicatorMetadataProvider;
            _areaHelper = areaHelper;
            _areasReader = areasReader;
            _attributesForBodyContainer = attributesForBodyContainer;
        }

        public IndicatorComparer AssistanceComparer { get; private set; }

        public byte[] GetIndicatorDataFile(int indicatorId)
        {
            var fileWriter = new SingleIndicatorFileWriter(indicatorId, _attributesForBodyContainer.GeneralParameters);

            byte[] content;
            // Don't recalculate if exist
            if (HasBeenWrittenFileFromCache(fileWriter, out content))
                return content;

            Grouping grouping; IndicatorMetadata indicatorMetadata;
            if (HasBeenIgnoreFileWriterSettings(indicatorId, fileWriter, out grouping, out indicatorMetadata))
                return new byte[] { };

            WriteIndicatorDataBodyInFile(indicatorId, indicatorMetadata, grouping, ref fileWriter);

            return fileWriter.GetFileContent();
        }

        private void WriteIndicatorDataBodyInFile(int indicatorId, IndicatorMetadata indicatorMetadata, Grouping grouping, ref SingleIndicatorFileWriter fileWriter)
        {
            _bodyPeriodWriter = new CsvBuilderIndicatorDataBodyPeriodWriter(indicatorMetadata, grouping, _areaHelper, _attributesForBodyContainer.GroupDataReader,
                                                                            _attributesForBodyContainer.GeneralParameters, _attributesForBodyContainer.OnDemandParameters, AssistanceComparer);

            _bodyPeriodWriter.WritePeriodsForIndicatorBodyInFile(indicatorId, ref fileWriter);
        }

        private static bool HasBeenWrittenFileFromCache(SingleIndicatorFileWriter fileWriter, out byte[] bytes)
        {
            bytes = null;
            bytes = fileWriter.TryLoadFile();
            return bytes != null;
        }

        private bool HasBeenIgnoreFileWriterSettings(int indicatorId, SingleIndicatorFileWriter fileWriter, out Grouping grouping, out IndicatorMetadata indicatorMetadata)
        {
            // Ignore the indicator if no grouping found
            if (!HasGroupingAndIndicatorMetadata(indicatorId, out grouping, out indicatorMetadata)) return true;

            // Lazy initialisation (would not be necessary if cached files are available)
            _areaHelper.Init();

            InitFileWriter(grouping, indicatorMetadata, fileWriter);
            InitComparer(grouping);

            return false;
        }

        private bool HasGroupingAndIndicatorMetadata(int indicatorId, out Grouping grouping, out IndicatorMetadata indicatorMetadata)
        {
            indicatorMetadata = null;
            if (!_attributesForBodyContainer.HasGrouping(indicatorId, _attributesForBodyContainer.OnDemandParameters.GroupIds, out grouping)) return false;

            indicatorMetadata = FindIndicatorMetadata(grouping);
            return true;
        }

        private void InitFileWriter(Grouping grouping, IndicatorMetadata indicatorMetadata, SingleIndicatorFileWriter fileWriter)
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
            return _indicatorMetadataProvider.GetIndicatorMetadata(new List<Grouping> { grouping }, _attributesForBodyContainer.AttributesForIndicators.IndicatorMetadataTextOption).First();
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
                    var categoryTypeIds = _areasReader.GetAllCategoryTypes().Select(x => x.Id).ToList();
                    _lookUpManager = new LookUpManager(_pholioReader, _areasReader, allAreaTypes, categoryTypeIds);
                }
                return _lookUpManager;
            }
        }
    }

}
