using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.SearchQuerying
{
    public class GeographicalSearch : SearchEngine
    {
        public bool AreEastingAndNorthingRetrieved = true;
        public bool ExcludeCcGs = false;

        public List<GeographicalSearchResult> SearchPlacePostcodes(string searchText, int childAreaTypeId)
        {
            var searchResults = new List<GeographicalSearchResult>();

            var userSearch = new SearchUserInput(searchText);

            if (userSearch.IsQueryValid)
            {
                var textInfo = CultureInfo.CurrentCulture.TextInfo;
                var isPostcode = userSearch.ContainsAnyNumbers;

                // Setup the fields to search through
                BooleanQuery finalQuery = isPostcode ?
                    GetPostcodeQuery(userSearch.SearchText, FieldNames.Postcode) :
                    GetPlaceNameQuery(userSearch, childAreaTypeId);

                // Perform the search
                var directory = FSDirectory.Open(new DirectoryInfo(
                    Path.Combine(ApplicationConfiguration.SearchIndexDirectory, "placePostcodes")));
                var searcher = new IndexSearcher(directory, true);

                //Add the sorting parameters
                var sortBy = new Sort(
                    new SortField(FieldNames.PlaceTypeWeighting, SortField.INT, true)
                   );
                var docs = searcher.Search(finalQuery, null, ShortResultCount, sortBy);

                int resultCount = docs.ScoreDocs.Length;
                for (int i = 0; i < resultCount; i++)
                {
                    ScoreDoc scoreDoc = docs.ScoreDocs[i];

                    Document doc = searcher.Doc(scoreDoc.Doc);

                    var name = isPostcode ?
                        doc.Get(FieldNames.Postcode).ToUpper() :
                        doc.Get(FieldNames.NameFormatted);

                    var geographicalSearchResult = new GeographicalSearchResult
                    {
                        PlaceName = name,
                        County = textInfo.ToTitleCase(doc.Get(FieldNames.County)),
                        PolygonAreaCode = doc.Get("Parent_Area_Code_" + childAreaTypeId),
                        PolygonAreaName = doc.Get("Parent_Area_Name_" + childAreaTypeId)
                    };

                    // Check parent area code was found
                    if (geographicalSearchResult.PolygonAreaCode == null)
                    {
                        throw new FingertipsException(string.Format(
                            "Area type id not supported for search: {0}", childAreaTypeId));
                    }

                    AssignEastingAndNorthing(doc, geographicalSearchResult);

                    if (!ExcludeCcGs)
                    {
                        searchResults.Add(geographicalSearchResult);
                    }
                    else
                    {
                        if (geographicalSearchResult.Easting != 0 && geographicalSearchResult.Northing != 0)
                        {
                            searchResults.Add(geographicalSearchResult);
                        }
                    }
                }
            }

            return searchResults;
        }

        private void AssignEastingAndNorthing(Document doc, GeographicalSearchResult pp)
        {
            if (AreEastingAndNorthingRetrieved)
            {
                var easting = doc.Get(FieldNames.Easting);
                if (easting != null)
                {
                    pp.Easting = int.Parse(easting);
                    pp.Northing = int.Parse(doc.Get(FieldNames.Northing));
                }
            }
        }

        protected static BooleanQuery GetPlaceNameQuery(SearchUserInput searchUserInput, int parentAreaTypeId)
        {
            var masterQuery = new BooleanQuery();
            foreach (string queryTerm in searchUserInput.Terms)
            {
                var query = new WildcardQuery(new Term(FieldNames.PlaceName, queryTerm));
                masterQuery.Add(query, searchUserInput.BooleanCombinationLogic);
            }

            var parentAreaTypeIdQuery = new WildcardQuery(new Term("Parent_Area_Code_" + parentAreaTypeId, "x"));
            masterQuery.Add(parentAreaTypeIdQuery, BooleanClause.Occur.MUST_NOT);

            return masterQuery;
        }

        protected static BooleanQuery GetPostcodeQuery(string searchText, params string[] fieldNames)
        {
            searchText = InsertSpaceIfNonePresent(searchText);

            var terms = GetCodeTerms(searchText);

            var masterQuery = new BooleanQuery();
            foreach (string queryTerm in terms)
            {
                foreach (string fieldName in fieldNames)
                {
                    var query = new WildcardQuery(new Term(fieldName, queryTerm));
                    masterQuery.Add(query, BooleanClause.Occur.MUST);
                }
            }
            return masterQuery;
        }

        private static string InsertSpaceIfNonePresent(string searchText)
        {
            if (searchText.IndexOf(" ") == -1 &&
                searchText.Length > 4 &&
                Regex.IsMatch(searchText, @"\w\d+\d\w*"))
            {
                // Insert space if none present e.g. "cb123"
                searchText = Regex.Replace(searchText, @"(\w+)(\d+)(\d)(\w*)",
                    m => string.Format("{0}{1} {2}{3}",
                        m.Groups[1].Value,
                        m.Groups[2].Value,
                        m.Groups[3].Value,
                        m.Groups[4].Value
                        ));
            }
            return searchText;
        }

        private static List<string> GetCodeTerms(string rawSearchText)
        {
            string text = rawSearchText.ToLower().Trim();
            List<string> terms = new List<string>();
            if (text.Contains(" "))
            {
                var bits = text.Split(' ');
                terms.Add(bits.First());
                terms.Add(bits.Last() + "*");
            }
            else
            {
                terms.Add(text);
            }
            return terms;
        }
    }
}