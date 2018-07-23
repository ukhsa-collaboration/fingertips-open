using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataSorting;
using PholioVisualisation.Formatting;

namespace PholioVisualisation.DataConstruction
{
    public class LongerLivesAreaDetailsBuilder
    {
        private readonly IAreasReader _areasReader = ReaderFactory.GetAreasReader();
        private readonly IGroupDataReader _groupDataReader = ReaderFactory.GetGroupDataReader();
        private IList<string> _ignoredAreaCodes;
        private IndicatorComparerFactory _indicatorComparerFactory;
        private IArea _area;
        private bool _isParentArea;

        public LongerLivesAreaDetails GetAreaDetails(int profileId, int groupId,
            int childAreaTypeId, string areaCode)
        {
            _area = AreaFactory.NewArea(_areasReader, areaCode);
            _isParentArea = IsParentArea(_area);

            _indicatorComparerFactory = new IndicatorComparerFactory
            {
                PholioReader = ReaderFactory.GetPholioReader()
            };

            var parentCodeToRanks = new Dictionary<string, List<AreaRankGrouping>>();
            var parentCodeToSignificances = new Dictionary<string, List<int?>>();
            var parentCodeToBenchmarks = new Dictionary<string, object>();

            InitIgnoredAreaCodes(profileId);

            // Get parent areas
            Decile decile;
            var parentAreas = GetParentAreas(childAreaTypeId, out decile);

            foreach (IArea parentArea in parentAreas)
            {
                string parentAreaCode = parentArea.Code;

                GroupData groupData = new GroupDataAtDataPointRepository().GetGroupDataProcessed(
                    parentAreaCode, childAreaTypeId, profileId, groupId);

                var rankBuilder = new AreaRankBuilder(_groupDataReader,_areasReader, 
                    new PholioLabelReader(), new NumericFormatterFactory(_groupDataReader));

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
                    var areaRankGrouping = rankBuilder
                        .BuildRank(_area, grouping, metadata, timePeriod, childAreaDataList);
                    ranks.Add(areaRankGrouping);

                    // Area significance
                    if (areaRankGrouping != null && _isParentArea == false)
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
            if (_isParentArea)
            {
                // Significances not relevant for parent areas
                parentCodeToSignificances = null;
                parentCodeToBenchmarks = null;
            }
            else
            {
                // Only need area web site link for child areas
                url = _areasReader.GetAreaUrl(_area.Code);
            }

            // Bespoke response object
            return new LongerLivesAreaDetails
            {
                Area = _area,
                Decile = decile,
                Url = url,
                Ranks = parentCodeToRanks,
                Significances = parentCodeToSignificances,
                Benchmarks = parentCodeToBenchmarks
            };
        }

        private List<IArea> GetParentAreas(int childAreaTypeId, out Decile decile)
        {
            Area country = _areasReader.GetAreaFromCode(AreaCodes.England);
            var parentAreas = new List<IArea> { country };

            // Add subnational parent areas
            AddOnsClusterToParentAreas(childAreaTypeId, _area, parentAreas);
            decile = GetDecile(parentAreas, childAreaTypeId, _area.Code);

            if (ShouldHaveNearestNeighbours(childAreaTypeId) && _isParentArea == false)
            {
                parentAreas.Add(NearestNeighbourArea.New(NearestNeighbourTypeIds.Cipfa,
                    _area.Code));
            }

            return parentAreas;
        }

        /// <summary>
        /// Synonymous with question is this a parent area
        /// </summary>
        private static bool IsParentArea(IArea area)
        {
            return area.IsCountry ||
                area.IsCountyAndUADeprivationDecile || area.IsOnsClusterGroup ||
                Area.IsNearestNeighbour(area.Code) || area is CategoryArea;
        }

        private static bool ShouldHaveNearestNeighbours(int childAreaTypeId)
        {
            return childAreaTypeId == AreaTypeIds.CountyAndUnitaryAuthority ||
                   childAreaTypeId == AreaTypeIds.DistrictAndUnitaryAuthority;
        }

        private int GetSignificance(string areaCode, GroupRoot groupRoot, Grouping grouping,
            IList<CoreDataSet> childAreaDataList)
        {
            CoreDataSet areaData = new CoreDataSetFilter(groupRoot.Data).SelectWithAreaCode(areaCode);
            if (areaData == null)
            {
                return (int)Significance.None;
            }

            var comparer = _indicatorComparerFactory.New(grouping);

            if (comparer is ICategoryComparer)
            {
                ICategoryComparer categoryComparer = comparer as ICategoryComparer;
                var values = new CoreDataSetFilter(childAreaDataList).SelectValidValues().ToList();
                categoryComparer.SetDataForCategories(values);

                return categoryComparer.GetCategory(areaData);
            }

            if (areaData.Significance.ContainsKey(grouping.ComparatorId))
            {
                return areaData.Significance[grouping.ComparatorId];
            }

            return (int)Significance.None;
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
                if (_area.IsGpPractice)
                {
                    // No need to calculate rank of practice in whole country
                    throw new FingertipsException("Do not use for GP Practices");
                }
                else
                {
                    // All data in country
                    dataList = new CoreDataSetListProvider(_groupDataReader).GetChildAreaData(grouping,
                        parentArea, timePeriod);
                    dataList = new CoreDataSetFilter(dataList).RemoveWithAreaCode(_ignoredAreaCodes).ToList();
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
            var isParentAreaTypeOnsCluster = childAreaTypeId == AreaTypeIds.DistrictAndUnitaryAuthority;

            if (isParentAreaTypeOnsCluster &&
                _isParentArea == false)
            {
                var allParentAreas = _areasReader.GetParentAreas(area.Code);

                var onsCluster = allParentAreas.First(x => x.IsOnsClusterGroup);

                parentAreas.Add(onsCluster);
            }
        }

        private void InitIgnoredAreaCodes(int profileId)
        {
            IProfileReader profilesReader = ReaderFactory.GetProfileReader();
            _ignoredAreaCodes = profilesReader.GetAreaCodesToIgnore(profileId).AreaCodesIgnoredEverywhere;
        }

        private static CategorisedArea GetDecileArea(int childAreaTypeId, string areaCode, IAreasReader areasReader)
        {
            CategorisedArea decileArea = null;
            int? categoryTypeId = null;

            if (childAreaTypeId == AreaTypeIds.DistrictAndUnitaryAuthority)
            {
                categoryTypeId = CategoryTypeIds.DeprivationDecileDistrictAndUA2015;
            }
            else if (childAreaTypeId == AreaTypeIds.CountyAndUnitaryAuthority)
            {
                categoryTypeId = CategoryTypeIds.DeprivationDecileCountyAndUA2015;
            }
            else if (childAreaTypeId == AreaTypeIds.CcgsPreApr2017)
            {
                categoryTypeId = CategoryTypeIds.DeprivationDecileCcg2010;
            }

            if (categoryTypeId.HasValue)
            {
                decileArea = areasReader.GetCategorisedArea(areaCode, AreaTypeIds.Country,
                    childAreaTypeId, categoryTypeId.Value);
            }

            return decileArea;
        }

        private Decile GetDecile(List<IArea> parentAreas, int childAreaTypeId, string areaCode)
        {
            CategorisedArea decileArea = GetDecileArea(childAreaTypeId, areaCode, _areasReader);

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