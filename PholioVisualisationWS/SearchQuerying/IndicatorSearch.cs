using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Parsers;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.SearchQuerying
{
    public class IndicatorSearch : SearchEngine
    {
        public const double ScoreThreshhold = 0.05;

        public IList<int> SearchIndicators(string searchText, IEnumerable<IndicatorMetadataTextProperty> properties)
        {
            List<int> indicatorIds = new List<int>();

            SearchUserInput searchUserInput = new SearchUserInput(searchText);

            if (searchUserInput.IsQueryValid)
            {
                if (searchUserInput.IsCommaSeparatedNumberList)
                {
                    // Allow user to search for list of indicator IDs
                    return new IntListStringParser(searchUserInput.SearchText).IntList;
                }

                var searchUserInputs = GetRelatedSearchTerms(searchUserInput);

                foreach (var userInput in searchUserInputs)
                {
                    var ids = SearchForIndicators(properties.ToList(), userInput);
                    indicatorIds.AddRange(ids);
                }

            }
            return indicatorIds;
        }

        private static List<int> SearchForIndicators(IEnumerable<IndicatorMetadataTextProperty> properties,
            SearchUserInput searchUserInput)
        {
            var masterQuery = new BooleanQuery();
            foreach (var property in properties)
            {
                var query = GetTextQuery(searchUserInput, property.ColumnName);
                query.SetBoost(property.SearchBoost);
                masterQuery.Add(query, BooleanClause.Occur.SHOULD);
            }

            IndexSearcher searcher = new IndexSearcher(GetIndexDirectory("indicators"), true);
            TopDocs docs = searcher.Search(masterQuery, null, MaxResultCount);

            List<int> indicatorIds = new List<int>();
            int resultCount = docs.ScoreDocs.Length;
            for (int i = 0; i < resultCount; i++)
            {
                ScoreDoc scoreDoc = docs.ScoreDocs[i];
                if (scoreDoc.Score > ScoreThreshhold)
                {
                    Document doc = searcher.Doc(scoreDoc.Doc);
                    indicatorIds.Add(Int32.Parse(doc.Get("id")));
                }
            }
            return indicatorIds.Distinct().ToList();
        }

        private IList<SearchUserInput> GetRelatedSearchTerms(SearchUserInput searchUserInput)
        {
            var text = searchUserInput.CleanSearchText;
            return RelatedTermsProvider
                .GetRelatedTerms(text)
                .Select(x => new SearchUserInput(x))
                .ToList();
        }
    }
}