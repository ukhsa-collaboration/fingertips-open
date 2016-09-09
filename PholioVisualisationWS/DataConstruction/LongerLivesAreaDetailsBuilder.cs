using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class LongerLivesAreaDetailsBuilder
    {
        private readonly IAreasReader areasReader = ReaderFactory.GetAreasReader();
        private readonly IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        private IList<string> ignoredAreaCodes;
        private IndicatorComparerFactory indicatorComparerFactory;
        private IArea area;

        public LongerLivesAreaDetails GetAreaDetails(int profileId, int groupId, 
            int childAreaTypeId, string areaCode)
        {
            area = AreaFactory.NewArea(areasReader, areaCode);
            indicatorComparerFactory = new IndicatorComparerFactory
            {
                PholioReader = ReaderFactory.GetPholioReader()
            };

            var parentCodeToRanks = new Dictionary<string, List<AreaRankGrouping>>();
            var parentCodeToSignificances = new Dictionary<string, List<int?>>();
            var parentCodeToBenchmarks = new Dictionary<string, object>();

            InitIgnoredAreaCodes(profileId);

            // Set up parent areas
            Area country = areasReader.GetAreaFromCode(AreaCodes.England);
            var parentAreas = new List<IArea> { country };
            AddOnsClusterToParentAreas(childAreaTypeId, area, parentAreas);
            Decile decile = GetDecile(parentAreas, childAreaTypeId, areaCode);

            var isParentArea = IsParentArea(area);

            foreach (IArea parentArea in parentAreas)
            {
                string parentAreaCode = parentArea.Code;

                GroupData groupData = new GroupDataAtDataPointRepository().GetGroupDataProcessed(
                    parentAreaCode, childAreaTypeId, profileId, groupId);

                var rankBuilder = new AreaRankBuilder
                {
                    GroupDataReader = groupDataReader,
                    AreasReader = areasReader,
                    Area = area
                };

                var ranks = new List<AreaRankGrouping>();
                var significances = new List<int?>();
                var benchmarks = new List<CoreDataSet>();

                parentCodeToRanks.Add(parentAreaCode, ranks);
                foreach (GroupRoot groupRoot in groupData.GroupRoots)
                {
                    Grouping grouping = GetGrouping(parentArea, groupRoot);

                    // Get data list
                    IndicatorMetadata metadata = groupData.GetIndicatorMetadataById(grouping.IndicatorId);
                    TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);
                    IList<CoreDataSet> childAreaDataList =
                        GetChildAreaDataList(parentArea, groupRoot, grouping, timePeriod);
                    var areaRankGrouping = rankBuilder.BuildRank(grouping, metadata, timePeriod, childAreaDataList);
                    ranks.Add(areaRankGrouping);

                    // Area significance
                    if (areaRankGrouping != null && isParentArea == false)
                    {
                        var significance = GetSignificance(areaCode, groupRoot, grouping, childAreaDataList);
                        significances.Add(significance);
                    }
                    else
                    {
                        significances.Add(null);
                    }

                    benchmarks.Add(grouping.ComparatorData);
                }
                parentCodeToSignificances.Add(parentAreaCode, significances);
                parentCodeToBenchmarks.Add(parentAreaCode, benchmarks);
            }

            string url = null;
            if (isParentArea)
            {
                // Significances not relevant for parent areas
                parentCodeToSignificances = null;
                parentCodeToBenchmarks = null;
            }
            else
            {
                // Only need area web site link for child areas
                url = areasReader.GetAreaUrl(area.Code);
            }

            // Bespoke response object
            return new LongerLivesAreaDetails
            {
                Area = area,
                Decile = decile,
                Url = url,
                Ranks = parentCodeToRanks,
                Significances = parentCodeToSignificances,
                Benchmarks = parentCodeToBenchmarks
            };
        }

        /// <summary>
        /// Synonymous with question is this a parent area
        /// </summary>
        private static bool IsParentArea(IArea area)
        {
            return area.IsCountry ||
                area.IsCountyAndUADeprivationDecile ||
                area.AreaTypeId == AreaTypeIds.OnsClusterGroup2001;
        }

        private int GetSignificance(string areaCode, GroupRoot groupRoot, Grouping grouping,
            IList<CoreDataSet> childAreaDataList)
        {
            CoreDataSet areaData = AreaHelper.GetDataForAreaFromDataList(areaCode, groupRoot.Data);
            if (areaData == null)
            {
                return (int)Significance.None;
            }

            var comparer = indicatorComparerFactory.New(grouping);

            if (comparer is ICategoryComparer)
            {
                ICategoryComparer categoryComparer = comparer as ICategoryComparer;
                var values = new CoreDataSetFilter(childAreaDataList).SelectValidValues().ToList();
                categoryComparer.SetDataForCategories(values);

                return categoryComparer.GetCategory(areaData);
            }

            return areaData.Significance[grouping.ComparatorId];
        }

        private static Grouping GetGrouping(IArea parentArea, GroupRoot groupRoot)
        {
            Grouping grouping = parentArea.IsCountry
                ? groupRoot.GetNationalGrouping()
                : groupRoot.GetSubnationalGrouping();

            if (grouping == null)
            {
                throw new FingertipsException(
                    "Cannot find both national and subnational groupings for indicator ID " +
                    groupRoot.IndicatorId + ". You may need to change the comparator for this indicator in FPM.");
            }
            return grouping;
        }

        private IList<CoreDataSet> GetChildAreaDataList(IArea parentArea,
            GroupRoot groupRoot, Grouping grouping, TimePeriod timePeriod)
        {
            IList<CoreDataSet> dataList;
            if (parentArea.IsCountry)
            {
                if (area.IsGpPractice)
                {
                    // No need to calculate rank of practice in whole country
                    throw new FingertipsException("Do not use for GP Practices");
                }
                else
                {
                    // All data in country
                    dataList = new CoreDataSetListProvider(groupDataReader).GetChildAreaData(grouping,
                        parentArea, timePeriod);
                    dataList = new CoreDataSetFilter(dataList).RemoveWithAreaCode(ignoredAreaCodes).ToList();
                }
            }
            else
            {
                // Use subnational data
                dataList = groupRoot.Data;
            }
            return dataList;
        }

        private void AddOnsClusterToParentAreas(int childAreaTypeId, IArea area, 
            List<IArea> parentAreas)
        {
            var isParentAreaTypeOnsCluster =
                (childAreaTypeId == AreaTypeIds.DistrictAndUnitaryAuthority ||
                 childAreaTypeId == AreaTypeIds.Ccg);

            if (isParentAreaTypeOnsCluster &&
                area.IsCountry == false &&
                area.AreaTypeId != AreaTypeIds.OnsClusterGroup2001)
            {
                var allParentAreas = areasReader.GetParentAreas(area.Code);

                var onsCluster = allParentAreas.First(x => x.AreaTypeId == AreaTypeIds.OnsClusterGroup2001);

                parentAreas.Add(onsCluster);
            }
        }

        private void InitIgnoredAreaCodes(int profileId)
        {
            IProfileReader profilesReader = ReaderFactory.GetProfileReader();
            ignoredAreaCodes = profilesReader.GetAreaCodesToIgnore(profileId).AreaCodesIgnoredEverywhere;
        }

        private static CategorisedArea GetDecileArea(int childAreaTypeId, string areaCode, IAreasReader areasReader)
        {
            CategorisedArea decileArea = null;

            if (childAreaTypeId == AreaTypeIds.DistrictAndUnitaryAuthority)
            {
                decileArea = areasReader.GetCategorisedArea(areaCode, AreaTypeIds.Country,
                    AreaTypeIds.DistrictAndUnitaryAuthority,
                    CategoryTypeIds.DeprivationDecileDistrictAndUA2010);
            }
            else if (childAreaTypeId == AreaTypeIds.CountyAndUnitaryAuthority)
            {
                decileArea = areasReader.GetCategorisedArea(areaCode, AreaTypeIds.Country,
                    AreaTypeIds.CountyAndUnitaryAuthority,
                    CategoryTypeIds.DeprivationDecileCountyAndUA2010);
            }

            return decileArea;
        }

        private Decile GetDecile(List<IArea> parentAreas, int childAreaTypeId, string areaCode)
        {
            CategorisedArea decileArea = GetDecileArea(childAreaTypeId, areaCode, areasReader);

            if (decileArea != null)
            {
                int decileNumber = decileArea.CategoryId;

                CategoryArea parentArea = CategoryArea.New(decileArea.CategoryTypeId, decileNumber);
                parentAreas.Add(parentArea);

                return new Decile
                {
                    Number = decileArea.CategoryId,
                    Code = parentArea.Code
                };
            }

            return null;
        }
    }
}