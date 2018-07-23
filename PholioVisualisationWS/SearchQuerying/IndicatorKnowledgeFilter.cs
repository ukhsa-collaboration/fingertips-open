using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.SearchQuerying
{
    public class IndicatorKnowledgeFilter
    {
        private IIndicatorMetadataRepository _indicatorMetadataRepository;

        public IndicatorKnowledgeFilter(IIndicatorMetadataRepository indicatorMetadataRepository)
        {
            _indicatorMetadataRepository = indicatorMetadataRepository;
        }

        public IList<int> FilterIndicatorIdsForSearchTermExpectations(string searchTerm,
            IList<int> indicatorIds)
        {
            var expectations = _indicatorMetadataRepository.GetIndicatorMetadataSearchExpectations(searchTerm);

            // Remove false positives (the ids the search included but shouldn't have)
            var negatives = GetIdsThatShouldNotBeIncluded(expectations);
            var filteredIds = indicatorIds.Where(x => negatives.Contains(x) == false).ToList();

            // Add false negatives (the ids the search missed)
            var expectedIndicatorIds = GetExpectedIndicatorIds(expectations);
            AddMissingIndicatorIds(expectedIndicatorIds, filteredIds);

            return filteredIds;
        }

        private static void AddMissingIndicatorIds(IList<int> expectedIndicatorIds, IList<int> filteredIds)
        {
            foreach (var indicatorId in expectedIndicatorIds)
            {
                if (filteredIds.Contains(indicatorId) == false)
                {
                    filteredIds.Add(indicatorId);
                }
            }
        }

        private static List<int> GetIdsThatShouldNotBeIncluded(IList<IndicatorMetadataSearchExpectation> expectations)
        {
            var negatives = expectations
                .Where(x => x.Expectation == false)
                .Select(x => x.IndicatorId).ToList();
            return negatives;
        }

        private static List<int> GetExpectedIndicatorIds(IList<IndicatorMetadataSearchExpectation> expectations)
        {
            var positives = expectations
                .Where(x => x.Expectation)
                .Select(x => x.IndicatorId).ToList();
            return positives;
        }
    }
}