using System.IO;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using PholioVisualisation.DataAccess;
using Directory = Lucene.Net.Store.Directory;

namespace PholioVisualisation.SearchQuerying
{
    public abstract class SearchEngine
    {
        public const int MaxResultCount = 100;
        public const int ShortResultCount = 10;

        protected static BooleanQuery GetTextQuery(SearchUserInput searchUserInput, params string[] fieldNames)
        {
            var masterQuery = new BooleanQuery();
            foreach (string queryTerm in searchUserInput.Terms)
            {
                foreach (string fieldName in fieldNames)
                {
                    var query = new WildcardQuery(new Term(fieldName, queryTerm));
                    masterQuery.Add(query, BooleanClause.Occur.SHOULD);
                }
            }
            return masterQuery;
        }

        protected static Directory GetIndexDirectory(string type)
        {
            string path = Path.Combine(ApplicationConfiguration.Instance.SearchIndexDirectory, type);
            return FSDirectory.Open(new DirectoryInfo(path));
        }
    }
}