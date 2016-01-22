using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using Lucene.Net.Search;

namespace PholioVisualisation.SearchQuerying
{
    public class IndicatorSearch : SearchEngine
    {
        public List<int> SearchIndicators(string searchText)
        {
            List<int> indicatorIds = new List<int>();

            SearchUserInput searchUserInput = new SearchUserInput(searchText);

            if (searchUserInput.IsQueryValid)
            {
                BooleanQuery masterQuery = GetTextQuery(searchUserInput, "IndicatorText");

                IndexSearcher searcher = new IndexSearcher(GetIndexDirectory("indicators"), true);
                TopDocs docs = searcher.Search(masterQuery, null, MaxResultCount);

                int resultCount = docs.ScoreDocs.Length;
                for (int i = 0; i < resultCount; i++)
                {
                    ScoreDoc scoreDoc = docs.ScoreDocs[i];
                    Document doc = searcher.Doc(scoreDoc.Doc);
                    indicatorIds.Add(Int32.Parse(doc.Get("id")));
                }
            }
            return indicatorIds;
        }
    }
}