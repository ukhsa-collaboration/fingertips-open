
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class TrendRootBuilder
    {
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();
        private bool _isParentAreaNearestNeighbour;

        public IList<TrendRoot> Build(IList<GroupRoot> groupRoots, ComparatorMap comparatorMap,
            int childAreaTypeId, int profileId, IList<IndicatorMetadata> indicatorMetadataList, bool isNearestNeighbour)
        {
            _isParentAreaNearestNeighbour = isNearestNeighbour;

            List<TrendRoot> trendRoots = new List<TrendRoot>();
            ITrendDataReader trendReader = ReaderFactory.GetTrendDataReader();

            var childAreaCodes = GetChildAreaCodes(comparatorMap, childAreaTypeId, profileId);

            if (groupRoots != null)
            {
                var metadataCollection = new IndicatorMetadataCollection(indicatorMetadataList);
                foreach (GroupRoot root in groupRoots)
                {
                    Grouping grouping = root.Grouping.FirstOrDefault();

                    if (grouping != null)
                    {
                        IndicatorMetadata indicatorMetadata = metadataCollection.GetIndicatorMetadataById(root.IndicatorId);
                        GroupRootTrendBuilderBase builder = GroupRootTrendBuilderBase.New(grouping, indicatorMetadata);
                        TrendRoot trendRoot = builder.BuildTrendRoot(comparatorMap, root, trendReader, childAreaCodes);
                        trendRoots.Add(trendRoot);
                    }
                }
            }

            return trendRoots;
        }

        private IList<string> GetChildAreaCodes(ComparatorMap comparatorMap, int childAreaTypeId, int profileId)
        {
            var parentAreaCode = comparatorMap.GetSubnationalComparator().Area.Code;
            IList<string> childAreaCodes = new ChildAreaListBuilder(areasReader, parentAreaCode,childAreaTypeId)
                .ChildAreas
                .Select(x => x.Code)
                .ToList();

            var filter = IgnoredAreasFilterFactory.New(profileId);
            childAreaCodes = filter.RemoveAreaCodesIgnoredEverywhere(childAreaCodes).ToList();

            // Add parent areacode to child areas for nearest neighbours only 
            if (_isParentAreaNearestNeighbour)
            {
                childAreaCodes.Insert(0, parentAreaCode.Substring(5));         
            }                              
            return childAreaCodes;
        }
    }
}
