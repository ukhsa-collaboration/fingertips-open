using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.DataConstruction;
using SpreadsheetGear;


namespace PholioVisualisation.Export
{
    public class ProfileDataBuilder : ExcelFileBuilder
    {
        public const string NationalLabel = "England";

        private IGroupDataReader _groupDataReader = ReaderFactory.GetGroupDataReader();
        private PholioLabelReader _labelReader = new PholioLabelReader();
        private PholioReader _pholioReader = ReaderFactory.GetPholioReader();
        private IAreasReader _areasReader = ReaderFactory.GetAreasReader();
        private SheetNamer _sheetNamer = new SheetNamer();
        private IArea _nationalArea;
        private IArea _parentArea;
        private List<IndicatorMetadata> _indicatorMetadatas = new List<IndicatorMetadata>();
        private ComparatorMap _comparatorMap;
        private Profile _profile;
        private readonly List<int> _restrictSearchProfileIds = new List<int>();
        private IList<int> _indicatorIds;
        private ProfileDataWriter _profileDataWriter;
        private IList<ParentArea> _parentAreas;
        private IAreaType _subnationalAreaType;
        private IList<ValueNote> _valueNotes;
        private IList<string> _groupRootKeys = new List<string>();
        private Dictionary<string, Area> _areaCodeToParentMap;
        private string _childAreaTypeSheetName, _subnationalAreaTypeSheetName;

        public ProfileDataBuilder(ComparatorMap comparatorMap, Profile profile, List<int> restrictSearchProfileIds,
            IList<ParentArea> parentAreas, IAreaType subnationalAreaType)
        {
            this._comparatorMap = comparatorMap;
            this._profile = profile;
            this._restrictSearchProfileIds = restrictSearchProfileIds;
            this._parentAreas = parentAreas;

            _profileDataWriter = new ProfileDataWriter(profile);

            _profileDataWriter.AddOrganisationDetails(profile.Id);

            _nationalArea = comparatorMap.GetNationalComparator().Area;

            this._subnationalAreaType = subnationalAreaType;
        }

        public ProfileDataBuilder(ComparatorMap comparatorMap, Profile profile, List<int> restrictSearchProfileIds,
            IList<int> indicatorIds, IList<ParentArea> parentAreas, IAreaType subnationalAreaType)
            : this(comparatorMap, profile, restrictSearchProfileIds, parentAreas, subnationalAreaType)
        {
            this._indicatorIds = indicatorIds;
        }

        protected ProfileDataBuilder()
        {
        }

        public override IWorkbook BuildWorkbook()
        {
            if (_indicatorIds != null)
            {
                CreateFileByIndicatorIds();
            }
            else
            {
                CreateFileByProfile();
            }

            _profileDataWriter.FinaliseBeforeWrite();
            return _profileDataWriter.Workbook;
        }

        private void CreateFileByIndicatorIds()
        {
            foreach (var parentArea in _parentAreas)
            {
                InitParentArea(parentArea);

                GroupData data = new GroupDataBuilderByIndicatorIds
                {
                    IndicatorIds = _indicatorIds,
                    ProfileId = _profile.Id,
                    RestrictSearchProfileIds = _restrictSearchProfileIds,
                    ComparatorMap = _comparatorMap,
                    ParentAreaCode = parentArea.AreaCode,
                    AreaTypeId = parentArea.ChildAreaTypeId
                }.Build();

                if (data.IsDataOk)
                {
                    data.GroupRoots = new GroupRootFilter(_groupDataReader).RemoveRootsWithoutChildAreaData(data.GroupRoots);
                }

                WriteCoreData(data, parentArea);
            }

            WriteIndicatorMetadata();
        }

        private void CreateFileByProfile()
        {
            foreach (ParentArea parentArea in _parentAreas)
            {
                InitParentArea(parentArea);

                //Get deciles for all relevant areas
                var categoryAreaType = _subnationalAreaType as CategoryAreaType;
                if (categoryAreaType != null)
                {
                    BuildCategoryAreaMap(categoryAreaType, parentArea);
                }

                foreach (int groupId in _profile.GroupIds)
                {
                    GroupData data = new GroupDataBuilderByGroupings
                    {
                        ProfileId = _profile.Id,
                        GroupId = groupId,
                        ChildAreaTypeId = parentArea.ChildAreaTypeId,
                        ComparatorMap = _comparatorMap,
                        AssignData = false
                    }.Build();
                    WriteCoreData(data, parentArea);
                }
            }

            WriteIndicatorMetadata();
        }

        private void BuildCategoryAreaMap(CategoryAreaType categoryAreaType, ParentArea parentArea)
        {
            var categories = _areasReader.GetCategories(categoryAreaType.CategoryTypeId);
            var subnationalCategoryIdToCategoryAreaMap = categories
                .ToDictionary<Category, int, IArea>(
                    category => category.Id,
                    CategoryArea.New
                );

            foreach (
                var categorisedArea in
                    _areasReader.GetCategorisedAreasForAllCategories(AreaTypeIds.Country, parentArea.ChildAreaTypeId,
                        categoryAreaType.CategoryTypeId))
            {
                var area = new Area
                {
                    Code = categorisedArea.CategoryId.ToString(),
                    Name = subnationalCategoryIdToCategoryAreaMap[categorisedArea.CategoryId].Name
                };

                _areaCodeToParentMap.Add(categorisedArea.AreaCode, area);
            }
        }

        private void InitParentArea(ParentArea parentArea)
        {
            Comparator comparator = _comparatorMap.GetRegionalComparatorByRegion(parentArea);

            // Add child area worksheet
            var areaType = _areasReader.GetAreaType(parentArea.ChildAreaTypeId);
            _childAreaTypeSheetName = _sheetNamer.GetSheetName(areaType.ShortName);
            _profileDataWriter.AddSheet(_childAreaTypeSheetName);
            _parentArea = comparator.Area;

            // Add parent area(s) worksheet
            if (_subnationalAreaType.Id != AreaTypeIds.Country)
            {
                _subnationalAreaTypeSheetName = _sheetNamer.GetSheetName(_subnationalAreaType.ShortName);
                _profileDataWriter.AddSheet(_subnationalAreaTypeSheetName);
            }

            // Add England Worksheet
            _profileDataWriter.AddSheet(NationalLabel);

            //Get all possible parent areas for the child area type
            _areaCodeToParentMap = _areasReader.GetParentAreasFromChildAreaId(_subnationalAreaType.Id);
        }

        /// <summary>
        /// Write indicator metadata in order it appears in grouping table (not
        /// in order of indicator ID).
        /// </summary>
        private void WriteIndicatorMetadata()
        {
            _profileDataWriter.AddIndicatorMetadata(
                _indicatorMetadatas
                    .Distinct()
                    .ToList()
                );
        }

        private IList<ValueNote> ValueNotes
        {
            get { return _valueNotes ?? (_valueNotes = _pholioReader.GetAllValueNotes()); }
        }

        private void WriteCoreData(GroupData data, ParentArea parentArea)
        {
            if (data.IsDataOk)
            {
                var valueNoteLookUp = ValueNotes.ToDictionary(x => x.Id, x => x.Text);

                // Child areas
                var childAreaCodes = data.Areas.Select(x => x.Code).ToArray();
                var childAreaCodeToAreaMap = data.Areas.ToDictionary(area => area.Code);

                // Worksheets
                WorksheetInfo childAreaWorksheet = _profileDataWriter.GetWorksheetInfo(_childAreaTypeSheetName);
                WorksheetInfo subnationalWorksheet = _profileDataWriter.GetWorksheetInfo(_subnationalAreaTypeSheetName);
                WorksheetInfo nationalWorksheet = _profileDataWriter.GetWorksheetInfo(NationalLabel);

                // Subnational areas
                ParentDataWriter parentDataWriter = ParentDataWriterFactory.New(_areasReader, _groupDataReader,
                    subnationalWorksheet, _profileDataWriter, _subnationalAreaType);
                var parentAreaDataProvider = new CoreDataSetProviderFactory().New(_parentArea);

                // No subnational sheet if direct parent area is country
                var isSubnationalSheet = subnationalWorksheet != null;

                var areMultipleSubnationalAreasRequired = AreMultipleSubnationalAreasRequired();

                foreach (var groupRoot in data.GroupRoots)
                {
                    // Check this data has not already been writen
                    var key = new GroupRootUniqueKey(groupRoot).Key + parentArea.AreaCode;
                    if (_groupRootKeys.Contains(key))
                    {
                        // This added has already been written
                        continue;
                    }
                    _groupRootKeys.Add(key);

                    var indicatorMetadata = data.GetIndicatorMetadataById(groupRoot.IndicatorId);

                    // Adding here means order in metadata sheet is same as in data sheet
                    AddMetadata(indicatorMetadata);

                    var timePeriodFormatter = new TimePeriodTextFormatter(indicatorMetadata);
                    var coreDataCollector = new CoreDataCollector();

                    var grouping = groupRoot.Grouping.FirstOrDefault();
                    if (grouping != null)
                    {
                        var sex = _labelReader.LookUpSexLabel(grouping.SexId);
                        var age = _labelReader.LookUpAgeLabel(grouping.AgeId);

                        var timePeriods = grouping.GetTimePeriodIterator(indicatorMetadata.YearType).TimePeriods;

                        // Write core data
                        foreach (TimePeriod timePeriod in timePeriods)
                        {
                            string timeString = timePeriodFormatter.Format(timePeriod);

                            var rowLabels = new RowLabels
                            {
                                Age = age,
                                Sex = sex,
                                TimePeriod = timeString,
                                IndicatorName = indicatorMetadata.Descriptive[IndicatorMetadataTextColumnNames.Name],
                                ValueNoteLookUp = valueNoteLookUp
                            };

                            // Write child area data
                            var coreDataList = _groupDataReader.GetCoreData(grouping, timePeriod, childAreaCodes);
                            _profileDataWriter.AddData(childAreaWorksheet, rowLabels, coreDataList, childAreaCodeToAreaMap, _areaCodeToParentMap);

                            // Subnational data
                            if (isSubnationalSheet)
                            {
                                if (areMultipleSubnationalAreasRequired)
                                {
                                    // Multiple parent areas
                                    var dataList = parentDataWriter.AddMultipleAreaData(rowLabels, grouping, timePeriod,
                                        indicatorMetadata, _areaCodeToParentMap);
                                    coreDataCollector.AddDataList(dataList);
                                }
                                else
                                {
                                    // One parent area
                                    var coreData = parentAreaDataProvider.GetData(grouping, timePeriod, indicatorMetadata);
                                    _profileDataWriter.AddData(subnationalWorksheet, rowLabels, coreData, _parentArea);
                                    coreDataCollector.AddData(coreData);
                                }
                            }

                            // Write national data
                            var nationalData = new CoreDataSetProviderFactory().New(_nationalArea).GetData(grouping, timePeriod, indicatorMetadata);
                            _profileDataWriter.AddData(nationalWorksheet, rowLabels, nationalData, _nationalArea);
                        }

                        // Write trend data
                        var trendMarkerWriter = TrendMarkerWriterFactory.New(_profileDataWriter, groupRoot.PolarityId, timePeriods, _profile.HasTrendMarkers);

                        // Child area trend markers
                        trendMarkerWriter.WriteChildTrendMarkers(childAreaWorksheet, groupRoot.RecentTrends, childAreaCodes);

                        // Subnational trend markers
                        if (isSubnationalSheet)
                        {
                            if (areMultipleSubnationalAreasRequired)
                            {
                                trendMarkerWriter.WriteMultipleSubnationalTrendMarkers(subnationalWorksheet, grouping,
                                    indicatorMetadata, coreDataCollector, parentDataWriter.CategoryIdAndAreaCodes);
                            }
                            else
                            {
                                trendMarkerWriter.WriteSingleSubnationalTrendMarker(subnationalWorksheet, grouping,
                                    indicatorMetadata, coreDataCollector);
                            }
                        }

                        // National trend markers
                        trendMarkerWriter.WriteNationalTrendMarkers(nationalWorksheet, groupRoot.RecentTrends,
                            _nationalArea.Code);
                    }
                }
            }
        }

        private void AddMetadata(IndicatorMetadata metadata)
        {
            if (_indicatorMetadatas.Select(x => x.IndicatorId).Contains(metadata.IndicatorId) == false)
            {
                _indicatorMetadatas.Add(metadata);
            }
        }

        private bool AreMultipleSubnationalAreasRequired()
        {
            return _parentArea.Code == _nationalArea.Code;
        }

    }
}