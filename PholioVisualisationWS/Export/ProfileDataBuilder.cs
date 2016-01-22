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
        protected IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        protected PholioLabelReader labelReader = new PholioLabelReader();
        protected PholioReader pholioReader = ReaderFactory.GetPholioReader();
        protected IAreasReader areasReader = ReaderFactory.GetAreasReader();
        protected SheetNamer sheetNamer = new SheetNamer();
        protected IArea nationalArea;
        protected IArea _parentArea;
        protected List<IndicatorMetadata> indicatorMetadatas = new List<IndicatorMetadata>();
        protected ComparatorMap comparatorMap;
        protected Profile profile;
        protected readonly List<int> restrictSearchProfileIds = new List<int>();
        protected IList<int> indicatorIds;
        protected ParentDisplay parentDisplay;

        private ProfileDataWriter profileDataWriter;
        private IList<ParentArea> parentAreas;
        private IAreaType subnationalAreaType;
        private IList<ValueNote> _valueNotes;

        private IList<string> groupRootKeys = new List<string>();

        private Dictionary<string, Area> areaCodeToParentMap;

        private string childAreaLabel, subnationalAreaLabel;
        public const string NationalLabel = "England";

        public ProfileDataBuilder(ComparatorMap comparatorMap, Profile profile, List<int> restrictSearchProfileIds,
            ParentDisplay parentDisplay, IList<ParentArea> parentAreas, IAreaType subnationalAreaType)
        {
            this.comparatorMap = comparatorMap;
            this.profile = profile;
            this.restrictSearchProfileIds = restrictSearchProfileIds;
            this.parentDisplay = parentDisplay;
            this.parentAreas = parentAreas;

            profileDataWriter = new ProfileDataWriter(profile);

            profileDataWriter.AddOrganisationDetails(profile.Id);

            nationalArea = comparatorMap.GetNationalComparator().Area;

            this.subnationalAreaType = subnationalAreaType;
        }

        public ProfileDataBuilder(ComparatorMap comparatorMap, Profile profile, List<int> restrictSearchProfileIds,
            ParentDisplay parentDisplay,
            IList<int> indicatorIds, IList<ParentArea> parentAreas, IAreaType subnationalAreaType)
            : this(comparatorMap, profile, restrictSearchProfileIds, parentDisplay, parentAreas, subnationalAreaType)
        {
            this.indicatorIds = indicatorIds;
        }

        protected ProfileDataBuilder()
        {
        }

        public override IWorkbook BuildWorkbook()
        {
            if (indicatorIds != null)
            {
                CreateFileByIndicatorIds();
            }
            else
            {
                CreateFileByProfile();
            }

            profileDataWriter.FinaliseBeforeWrite();
            return profileDataWriter.Workbook;
        }

        private void CreateFileByIndicatorIds()
        {
            foreach (var parentArea in parentAreas)
            {
                InitParentArea(parentArea);

                GroupData data = new GroupDataBuilderByIndicatorIds
                {
                    IndicatorIds = indicatorIds,
                    ProfileId = profile.Id,
                    RestrictSearchProfileIds = restrictSearchProfileIds,
                    ComparatorMap = comparatorMap,
                    ParentAreaCode = parentArea.AreaCode,
                    AreaTypeId = parentArea.ChildAreaTypeId
                }.Build();

                if (data.IsDataOk)
                {
                    data.GroupRoots = new GroupRootFilter(data.GroupRoots).RemoveRootsWithoutChildAreaData();
                }

                WriteCoreData(data, parentArea);
            }

            WriteIndicatorMetadata();
        }

        private void CreateFileByProfile()
        {
            foreach (ParentArea parentArea in parentAreas)
            {
                InitParentArea(parentArea);

                ComparatorMap limitedMap = comparatorMap.LimitByParentArea(parentArea);

                //Get deciles for all relevant areas
                var categoryAreaType = subnationalAreaType as CategoryAreaType;
                if (categoryAreaType!=null)
                {
                    BuildCategoryAreaMap(categoryAreaType, parentArea);
                }

                foreach (int groupId in profile.GroupIds)
                {
                    GroupData data = new GroupDataBuilderByGroupings
                    {
                        ProfileId = profile.Id,
                        GroupId = groupId,
                        ChildAreaTypeId = parentArea.ChildAreaTypeId,
                        ComparatorMap = limitedMap,
                        AssignData = false
                    }.Build();
                    WriteCoreData(data, parentArea);
                }
            }

            WriteIndicatorMetadata();
        }

        private void BuildCategoryAreaMap(CategoryAreaType categoryAreaType, ParentArea parentArea)
        {
            var categories = areasReader.GetCategories(categoryAreaType.CategoryTypeId);
            var subnationalCategoryIdToCategoryAreaMap = categories
                .ToDictionary<Category, int, IArea>(
                    category => category.CategoryId,
                    CategoryArea.New
                );

            foreach (
                var categorisedArea in
                    areasReader.GetCategorisedAreasForAllCategories(AreaTypeIds.Country, parentArea.ChildAreaTypeId,
                        categoryAreaType.CategoryTypeId))
            {
                var area = new Area
                {
                    Code = categorisedArea.CategoryId.ToString(),
                    Name = subnationalCategoryIdToCategoryAreaMap[categorisedArea.CategoryId].Name
                };

                areaCodeToParentMap.Add(categorisedArea.AreaCode, area);
            }
        }

        private void InitParentArea(ParentArea parentArea)
        {
            Comparator comparator = comparatorMap.GetRegionalComparatorByRegion(parentArea);

            // Add child area worksheet
            var areaType = areasReader.GetAreaType(parentArea.ChildAreaTypeId);
            childAreaLabel = sheetNamer.GetSheetName(areaType.ShortName);
            profileDataWriter.AddSheet(childAreaLabel);
            _parentArea = comparator.Area;

            // Add parent area(s) worksheet
            if (subnationalAreaType.Id != AreaTypeIds.Country)
            {
                subnationalAreaLabel = sheetNamer.GetSheetName(subnationalAreaType.ShortName);
                profileDataWriter.AddSheet(subnationalAreaLabel);
            }

            if (profile.IsNational)
            {
                // Add England Worksheet
                profileDataWriter.AddSheet(NationalLabel);
            }

            //Get all possible parent areas for the child area type
            areaCodeToParentMap = areasReader.GetParentAreasFromChildAreaId(subnationalAreaType.Id);
        }

        /// <summary>
        /// Write indicator metadata in order it appears in grouping table (not
        /// in order of indicator ID).
        /// </summary>
        private void WriteIndicatorMetadata()
        {
            profileDataWriter.AddIndicatorMetadata(
                indicatorMetadatas
                    .Distinct()
                    .ToList()
                );
        }

        public IList<ValueNote> ValueNotes
        {
            get { return _valueNotes ?? (_valueNotes = pholioReader.GetValueNotes()); }
        }

        private void WriteCoreData(GroupData data, ParentArea parentArea)
        {
            if (data.IsDataOk)
            {
                // Child areas
                var childAreaCodes = data.Areas.Select(x => x.Code).ToArray();
                var childAreaCodeToAreaMap = data.Areas.ToDictionary(area => area.Code);

                // Subnational areas
                var categoryAreaType = subnationalAreaType as CategoryAreaType;
                Dictionary<string, IArea> subnationalAreaCodeToAreaMap = null;
                Dictionary<int, IArea> subnationalCategoryIdToCategoryAreaMap = null;
                IList<string> subnationalAreaCodes = null;
                if (categoryAreaType != null)
                {
                    var categories = areasReader.GetCategories(categoryAreaType.CategoryTypeId);
                    subnationalCategoryIdToCategoryAreaMap = categories
                        .ToDictionary<Category, int, IArea>(
                        category => category.CategoryId,
                        category => CategoryArea.New(category)
                        );
                }
                else
                {
                    // set subnationalAreaCodeToAreaMap
                    var areas = areasReader.GetAreasByAreaTypeId(subnationalAreaType.Id);
                    subnationalAreaCodes = areas.Select(x => x.Code).ToList();
                    subnationalAreaCodeToAreaMap = areas
                        .ToDictionary<Area, string, IArea>(
                        area => area.Code,
                        area => area
                        );
                }

                var valueNotes = ValueNotes;

                foreach (var groupRoot in data.GroupRoots)
                {
                    // Check this data has not already been writen
                    var key = new GroupRootUniqueKey(groupRoot).Key + parentArea.AreaCode;
                    if (groupRootKeys.Contains(key))
                    {
                        // This added has already been written
                        continue;
                    }
                    groupRootKeys.Add(key);

                    var metadata = data.GetIndicatorMetadataById(groupRoot.IndicatorId);

                    // Adding here means order in metadata sheet is same as in data sheet
                    AddMetadata(metadata);

                    var timePeriodFormatter = new TimePeriodTextFormatter(metadata);

                    var grouping = groupRoot.Grouping.FirstOrDefault();
                    if (grouping != null)
                    {
                        var sex = labelReader.LookUpSexLabel(grouping.SexId);
                        var age = labelReader.LookUpAgeLabel(grouping.AgeId);

                        var iterator = grouping.GetTimePeriodIterator(metadata.YearType);
                        foreach (TimePeriod timePeriod in iterator.TimePeriods)
                        {
                            string timeString = timePeriodFormatter.Format(timePeriod);

                            // Write child area data
                            var coreData = groupDataReader.GetCoreData(grouping, timePeriod, childAreaCodes);

                            profileDataWriter.AddData(childAreaLabel, timeString, coreData, childAreaCodeToAreaMap, sex,
                                metadata.Descriptive, age, valueNotes, areaCodeToParentMap);

                            // Subnational data
                            if (subnationalAreaLabel != null)
                            {
                                if (AreMultipleSubnationalAreasRequired())
                                {
                                    // Write data for all areas of subnational type
                                    if (categoryAreaType != null)
                                    {
                                        var subnationalData = groupDataReader
                                            .GetCoreDataListForAllCategoryAreasOfCategoryAreaType(
                                                grouping, timePeriod, categoryAreaType.CategoryTypeId, AreaCodes.England);

                                        profileDataWriter.AddCategorisedData(subnationalAreaLabel, timeString,
                                            subnationalData,
                                            subnationalCategoryIdToCategoryAreaMap, sex, metadata.Descriptive, age,
                                            valueNotes);
                                    }
                                    else
                                    {
                                        var subnationalcoreData = groupDataReader.GetCoreData(grouping, timePeriod,
                                            subnationalAreaCodes.ToArray());
                                        if (subnationalcoreData.Count == 0)
                                        {
                                            AddCalculatedAverageValue(subnationalAreaCodeToAreaMap, grouping, timePeriod, metadata, timeString, sex, age, valueNotes);
                                        }
                                        else
                                        {
                                            profileDataWriter.AddData(subnationalAreaLabel, timeString, subnationalcoreData,
                                                subnationalAreaCodeToAreaMap, sex, metadata.Descriptive, age, valueNotes, areaCodeToParentMap);
                                        }
                                    }
                                }
                                else
                                {
                                    if (categoryAreaType != null && (categoryAreaType.CategoryTypeId != CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority && categoryAreaType.CategoryTypeId != CategoryTypeIds.DeprivationDecileDistrictAndUnitaryAuthority))
                                    {
                                        var subnationalcoreData = groupDataReader.GetCoreData(grouping, timePeriod, subnationalAreaCodes.ToArray());
                                        if (subnationalcoreData.Count == 0)
                                        {
                                            AddCalculatedAverageValue(subnationalAreaCodeToAreaMap, grouping, timePeriod, metadata, timeString, sex, age, valueNotes);
                                        }
                                        else
                                        {
                                            // Write data for one subnational area
                                            AddSingleAreaData(grouping, timePeriod, subnationalAreaLabel, metadata, timeString,
                                                sex, age, _parentArea, valueNotes);
                                        }
                                    }
                                    else
                                    {
                                        // Write data for one subnational area
                                        AddSingleAreaData(grouping, timePeriod, subnationalAreaLabel, metadata, timeString,
                                            sex, age, _parentArea, valueNotes);
                                    }
                                }
                            }

                            // Write national data
                            if (profile.IsNational && IsEnglandDataRequired())
                            {
                                AddSingleAreaData(grouping, timePeriod, NationalLabel, metadata, timeString,
                                    sex, age, nationalArea, valueNotes);
                            }
                        }
                    }
                }
            }
        }

        private void AddCalculatedAverageValue(Dictionary<string, IArea> subnationalAreaCodeToAreaMap, Grouping grouping, TimePeriod timePeriod,
            IndicatorMetadata metadata, string timeString, string sex, string age, IList<ValueNote> valueNotes)
        {
            foreach (var subnationalArea in subnationalAreaCodeToAreaMap)
            {
                var regionalAreas = ReadChildAreas(subnationalArea.Value.Code, grouping.AreaTypeId);
                var regionalValues = groupDataReader.GetCoreData(grouping, timePeriod,
                    regionalAreas.Select(x => x.Code).ToArray());

                var averageCalculator = AverageCalculatorFactory.New(regionalValues, metadata);
                if (averageCalculator.Average != null)
                {
                    profileDataWriter.AddData(subnationalAreaLabel, timeString, averageCalculator.Average, subnationalArea.Value,
                        sex, metadata.Descriptive, age, valueNotes);
                }
            }
        }

        private IList<IArea> ReadChildAreas(string parentAreaCode, int childAreaTypeId)
        {
            IList<IArea> childAreas = new ChildAreaListBuilder(areasReader, parentAreaCode, childAreaTypeId).ChildAreas;
            return childAreas;
        }

        private void AddMetadata(IndicatorMetadata metadata)
        {
            if (indicatorMetadatas.Select(x => x.IndicatorId).Contains(metadata.IndicatorId) == false)
            {
                indicatorMetadatas.Add(metadata);
            }
        }

        private bool AreMultipleSubnationalAreasRequired()
        {
            return _parentArea.Code == nationalArea.Code;
        }

        private bool IsEnglandDataRequired()
        {
            return parentDisplay != ParentDisplay.RegionalOnly;
        }

        private void AddSingleAreaData(Grouping grouping, TimePeriod timePeriod, string worksheetLabel,
            IndicatorMetadata metadata, string timeString, string sex, string age, IArea area,
            IList<ValueNote> valueNotes)
        {
            var coreData = new CoreDataSetProviderFactory().New(area).GetData(grouping, timePeriod, metadata);

            if (coreData != null)
            {
                profileDataWriter.AddData(worksheetLabel, timeString, coreData, area, sex,
                    metadata.Descriptive, age, valueNotes);
            }
        }
    }
}