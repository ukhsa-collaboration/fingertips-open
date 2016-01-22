using System.Collections.Generic;
using Fpm.ProfileData;

namespace Fpm.Search
{
    public class DomainIndicatorsSearchResultBuilder
    {
//        public List<DomainIndicatorsSearchResult> NAME()
//        {
//
//            var allAreaTypes = CommonUtilities.GetAllAreaTypes();
//            var profiles = CommonUtilities.GetProfiles();
//
//            foreach (var indicatorMetadataTextValue in indicatorMatch)
//            {
//                var groupsWhereIndicatorIsUsed = CommonUtilities.GetGroupingsByIndicatorIds(new List<int> { indicatorMetadataTextValue.IndicatorId }).Distinct(new DistinctGroupComparer());
//
//                foreach (var grouping in groupsWhereIndicatorIsUsed)
//                {
//                    var groupingMetadata = CommonUtilities.GetGroupingMetadata(new List<int> { grouping.GroupId }).First();
//
//                    IEnumerable<GroupingPlusNames> groupingsPlusNames = _dataAccess.GetGroupingPlusNames(grouping.IndicatorId,
//                        grouping.GroupId, grouping.AreaTypeId).Distinct(new IndicatorNameComparer());
//
//                    var domainIndicators = new DomainIndicatorsSearchResult();
//
//                    foreach (var groupingPlusNames in groupingsPlusNames)
//                    {
//                        domainIndicators.GroupId = grouping.GroupId;
//                        domainIndicators.GroupName = groupingMetadata.GroupName;
//                        domainIndicators.SequenceId = groupingMetadata.Sequence;
//
//                        var profile = profiles.First(x => x.GroupIds.Contains(grouping.GroupId));
//                        domainIndicators.ProfileName = profile.Name;
//                        domainIndicators.UrlKey = profile.UrlKey;
//
//                        groupingPlusNames.AreaType = allAreaTypes.First(x => x.Id == grouping.AreaTypeId).ShortName;
//                        groupingPlusNames.ComparatorMethodId = grouping.ComparatorMethodId;
//                        groupingPlusNames.ComparatorId = grouping.ComparatorId;
//
//                        domainIndicators.Indicators.Add(groupingPlusNames);
//                    }
//
//                    model.Add(domainIndicators);
//                }
//            }
//        } 
    }
}