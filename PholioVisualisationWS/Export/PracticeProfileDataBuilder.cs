using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;

namespace PholioVisualisation.Export
{
    /// <summary>
    /// Builds a Excel file of Practice Profile data.
    /// </summary>
    public class PracticeProfileDataBuilder : ExcelFileBuilder
    {
        private PracticeProfileDataWriter writer;

        private PracticeDataAccess practiceReader = new PracticeDataAccess();
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();
        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        private PholioReader pholioReader = ReaderFactory.GetPholioReader();
        private Dictionary<string, Area> practiceCodeToParentMap;
        private CoreDataSetProviderFactory coreDataSetProviderFactory;

        // Must be set before BuildWorkbook called
        public List<int> GroupIds { get; set; }
        public string AreaCode { get; set; }
        public int ParentAreaTypeId { get; set; }

        /// <summary>
        /// Creates instance to export population data.
        /// </summary>
        public PracticeProfileDataBuilder()
        {
        }

        public override IWorkbook BuildWorkbook()
        {
            writer = new PracticeProfileDataWriter(ParentAreaTypeId);

            CheckParameters();

            Area area = areasReader.GetAreaFromCode(AreaCode);

            IList<string> practiceCodes = GetPracticeCodes(area);
            writer.AddPracticeCodes(practiceCodes);

            InitPracticeToParentAreaMaps(area, practiceCodes);


            IList<IndicatorMetadata> metadataList;
            writer.AddParentsToPracticeSheet(practiceCodeToParentMap, false);
            IEnumerable<Area> parentAreas = GetUniqueAreaCodesOfValues(practiceCodeToParentMap);
            parentAreas = from a in parentAreas orderby a.Name select a;
            writer.AddAreaNamesCodestoParentAreaSheet(parentAreas, ParentAreaTypeId);

            coreDataSetProviderFactory = new CoreDataSetProviderFactory
            {
                CcgPopulationProvider = new CcgPopulationProvider(pholioReader)
            };
         
            metadataList = AddPopulationData(area, parentAreas);               

            writer.AddIndicatorMetadata(metadataList);
            AddPracticeAddresses(area, practiceCodes);

            writer.FinaliseBeforeWrite();

            return writer.Workbook;
        }

        private IList<IndicatorMetadata> AddPopulationData(Area area, IEnumerable<Area> ccgs)
        {
            Grouping grouping = groupDataReader.GetGroupingsByGroupIdAndIndicatorId(
                GroupIds.First(), // only ever 1 group Id for population data
                IndicatorIds.QuinaryPopulations);
            var metadataList = IndicatorMetadataProvider.Instance
                .GetIndicatorMetadataCollection(grouping)
                .IndicatorMetadata;

            IList<int> ageIds = QuinaryPopulationSorter.GetAgeIdsToOver95();
            IEnumerable<int> sexIds = new[] { SexIds.Male, SexIds.Female };

            var metadata = metadataList.First(); // Why First??
            IList<TimePeriod> timePeriods = grouping.GetTimePeriodIterator(metadata.YearType).TimePeriods;
            var periodLabels = GetPeriodLabels(metadata, timePeriods);
            AddPopulationTitles(sexIds, ageIds, periodLabels);

            foreach (TimePeriod timePeriod in timePeriods)
            {
                foreach (int sexId in sexIds)
                {
                    grouping.SexId = sexId;

                    foreach (int ageId in ageIds)
                    {
                        grouping.AgeId = ageId;

                        // Add data
                        writer.AddPracticeIndicatorValues(GetPracticeDataMap(area, grouping, timePeriod));
                    }
                }
            }

            foreach (var ccg in ccgs)
            {
                List<QuinaryPopulation> populations = new List<QuinaryPopulation>();

                foreach (TimePeriod timePeriod in timePeriods)
                {
                    foreach (int sexId in sexIds)
                    {
                        // Get all populations
                        populations.Add(practiceReader.GetCcgQuinaryPopulation(grouping.IndicatorId, timePeriod, ccg.Code, sexId));
                    }
                }

                writer.AddCcgPopulationValues(populations);
            }


            writer.AddIndicatorMetadata(metadataList);
            return metadataList;
        }

        private static IList<string> GetPeriodLabels(IndicatorMetadata metadata, IList<TimePeriod> timePeriods)
        {
            var builder = new TimePeriodTextListBuilder(metadata);
            builder.AddRange(timePeriods);
            IList<string> periodLabels = builder.GetTimePeriodStrings();
            return periodLabels;
        }

        /// <summary>
        /// Check required parameters have been set.
        /// </summary>
        private void CheckParameters()
        {
            if (GroupIds.Count == 0)
            {
                throw new ArgumentException("GroupId is not set");
            }

            if (AreaCode == null)
            {
                throw new ArgumentException("AreaCode is not set");
            }

            if (ParentAreaTypeId == -1)
            {
                throw new ArgumentException("AreaTypeId is not set");
            }
        }

        private void AddPopulationTitles(IEnumerable<int> sexIds, IList<int> ageIds, IList<string> periodLabels)
        {
            IList<string> populationLabels = ReaderFactory.GetPholioReader().GetQuinaryPopulationLabels(ageIds);

            PholioLabelReader labelReader = new PholioLabelReader();
            IList<string> sexLabels = sexIds.Select(labelReader.LookUpSexLabel).ToList();

            writer.AddPopulationTitles(sexLabels, periodLabels, populationLabels);
        }

        private IList<IndicatorMetadata> AddTopicData(Area area, IEnumerable<Area> parentAreas)
        {
             IndicatorMetadataCollection allMetadata = new IndicatorMetadataCollection();

            foreach (var groupId in GroupIds)
            {
                IList<Grouping> groupings = groupDataReader.GetGroupingsByGroupId(groupId);

                IList<GroupRoot> roots = new GroupRootBuilder(groupDataReader).BuildGroupRoots(groupings);

                IndicatorMetadataCollection metadataCollection =
                    IndicatorMetadataProvider.Instance.GetIndicatorMetadataCollection(groupings);

                allMetadata.AddIndicatorMetadata(metadataCollection.IndicatorMetadata);

                foreach (GroupRoot root in roots)
                {
                    Grouping grouping = root.FirstGrouping;

                    // Add indicator information
                    IndicatorMetadata metadata = metadataCollection.GetIndicatorMetadataById(grouping.IndicatorId);
                    IList<TimePeriod> periods = grouping.GetTimePeriodIterator(metadata.YearType).TimePeriods;
                    writer.AddPracticeIndicatorTitles(metadata, periods);
                    writer.AddCcgIndicatorTitles(metadata, periods);

                    foreach (TimePeriod timePeriod in periods)
                    {
                        // Add data
                        writer.AddPracticeIndicatorData(GetPracticeDataMap(area, grouping, timePeriod));
                        writer.AddAreaIndicatorData(GetParentAreaDataMap(grouping, timePeriod, parentAreas, metadata));
                    }
                }
            }
            return allMetadata.IndicatorMetadata;
        }

        private Dictionary<string, CoreDataSet> GetPracticeDataMap(Area area, Grouping grouping, TimePeriod timePeriod)
        {
            Dictionary<string, CoreDataSet> dataMap;
            if (area.IsCountry)
            {
                dataMap = practiceReader.GetPracticeCodeToBaseDataMap(grouping, timePeriod);
            }
            else if (area.IsGpPractice)
            {
                dataMap = new Dictionary<string, CoreDataSet>();
                CoreDataSet data = groupDataReader.GetCoreData(grouping, timePeriod, area.Code).FirstOrDefault();
                if (data != null && data.IsValueValid)
                {
                    dataMap.Add(area.Code, data);
                }
            }
            else
            {
                dataMap = practiceReader.GetPracticeCodeToBaseDataMap(grouping, timePeriod, area.Code);
            }
            return dataMap;
        }

        private Dictionary<string, CoreDataSet> GetParentAreaDataMap(Grouping grouping, TimePeriod timePeriod, IEnumerable<Area> parentAreas, IndicatorMetadata indicatorMetadata)
        {
            var areaDataMap = new Dictionary<string, CoreDataSet>();
            foreach (var parentArea in parentAreas)
            {
                var coreDataSet = coreDataSetProviderFactory.New(parentArea)
                    .GetData(grouping, timePeriod, indicatorMetadata);
            
                if (coreDataSet != null && coreDataSet.IsValueValid)
                {
                    areaDataMap.Add(parentArea.Code, coreDataSet);
                }
            }
            
            return areaDataMap;
        }

        private void InitPracticeToParentAreaMaps(Area area, IList<string> practiceCodes)
        {
            if (area.IsCountry)
            {
                // Add all Areas
                practiceCodeToParentMap =
                    areasReader.GetParentsFromChildAreaIdAndParentAreaTypeId(ParentAreaTypeId, AreaTypeIds.GpPractice);
            }
            else if (area.IsGpPractice)
            {
                practiceCodeToParentMap = new Dictionary<string, Area>();
                var code = area.Code;
                IList<Area> parentAreas = areasReader.GetParentAreas(code);
                AddCcgParent(practiceCodeToParentMap, code, parentAreas);
            }
            else if (area.IsCounty || area.IsUa || area.IsCcg)
            {
                practiceCodeToParentMap = new Dictionary<string, Area>();
                foreach (var code in practiceCodes)
                {
                    IList<Area> parentAreas = areasReader.GetParentAreas(code);
                    if (area.IsCounty)
                    {
                        AddCountyParent(practiceCodeToParentMap, code, parentAreas);
                    }
                    else if (area.IsUa)
                    {
                        AddUaParent(practiceCodeToParentMap, code, parentAreas);
                    }
                    else
                    {
                        AddCcgParent(practiceCodeToParentMap, code, parentAreas);
                    }
                }
            }
        }

        private void AddPracticeAddresses(Area area, IList<string> practiceCodes)
        {
            IList<AreaAddress> practiceAddresses;
            if (area.IsGpPractice)
            {
                practiceAddresses = new List<AreaAddress>();
                AddAddressToList(area.Code, practiceAddresses);
            }
            else if (area.IsCountry)
            {
                practiceAddresses = areasReader.GetAreaWithAddressByAreaTypeId(AreaTypeIds.GpPractice);
            }
            else
            {
                practiceAddresses = new List<AreaAddress>();
                foreach (var practiceCode in practiceCodes)
                {
                    AddAddressToList(practiceCode, practiceAddresses);
                }
            }

            Dictionary<string, AreaAddress> practiceCodeToAddressMap =
                practiceAddresses.ToDictionary(a => a.Code);
            writer.AddPracticeAddresses(practiceCodeToAddressMap);
        }

        private void AddAddressToList(string areaCode, IList<AreaAddress> practiceAddresses)
        {
            AreaAddress address = areasReader.GetAreaWithAddressFromCode(areaCode);
            if (address != null)
            {
                practiceAddresses.Add(address);
            }
        }

        private static void AddCcgParent(Dictionary<string, Area> map, string code, IList<Area> parentAreas)
        {
            var area = parentAreas.FirstOrDefault(a => AreaType.IsCcgAreaTypeId(a.AreaTypeId));
            AddArea(area, map, code);
        }

        private static void AddUaParent(Dictionary<string, Area> map, string code, IList<Area> parentAreas)
        {
            var area = parentAreas.FirstOrDefault(a => a.AreaTypeId == AreaTypeIds.UnitaryAuthority);
            AddArea(area, map, code);
        }

        private static void AddCountyParent(Dictionary<string, Area> map, string code, IList<Area> parentAreas)
        {
            var area = parentAreas.FirstOrDefault(a => a.AreaTypeId == AreaTypeIds.County);
            AddArea(area, map, code);
        }

        private static void AddArea(Area area, Dictionary<string, Area> map, string code)
        {
            if (area != null && map.ContainsKey(code) == false)
            {
                map.Add(code, area);
            }
        }

        private static IEnumerable<Area> GetUniqueAreaCodesOfValues(Dictionary<string, Area> practiceCodeToParentMap)
        {
            return practiceCodeToParentMap == null ? new List<Area>() :
                        practiceCodeToParentMap.Values
                            .GroupBy(a => a.Code)
                            .Select(a => a.First());
        }

        private IList<string> GetPracticeCodes(Area area)
        {
            if (area.IsCountry)
            {
                return areasReader.GetAreaCodesForAreaType(AreaTypeIds.GpPractice);
            }

            if (area.IsGpPractice)
            {
                return new List<string> { AreaCode };
            }

            // CCG
            return areasReader.GetChildAreaCodes(AreaCode, AreaTypeIds.GpPractice);
        }

    }
}
