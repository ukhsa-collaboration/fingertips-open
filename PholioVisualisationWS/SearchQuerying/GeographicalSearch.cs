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
    public enum ResultsToInclude
    {
        PlaceNamesAndPostcodesOnly,
        ParentAreasOnly,
        PlaceNamesAndPostcodesAndParentAreas
    }

    public class GeographicalSearch : SearchEngine
    {
        public bool AreEastingAndNorthingRetrieved = true;

        private readonly TextInfo _textInfo= CultureInfo.CurrentCulture.TextInfo;

        public List<GeographicalSearchResult> SearchPlacePostcodes(string searchText, 
            int polygonAreaTypeId, IList<int> parentAreaTypesToInclude)
        {
            var userSearch = new SearchUserInput(searchText);

            if (userSearch.IsQueryValid)
            {
                // Search polygon area type
                bool includePolygonAreaType = parentAreaTypesToInclude.Any() && parentAreaTypesToInclude.Contains(polygonAreaTypeId);
                var resultsToInclude = includePolygonAreaType
                    ? ResultsToInclude.PlaceNamesAndPostcodesAndParentAreas
                    : ResultsToInclude.PlaceNamesAndPostcodesOnly;
                var searchResults = GetSearchResults(userSearch, polygonAreaTypeId, resultsToInclude);

                // Extra parent areas
                var extraParentAreaTypesToSearchFor = parentAreaTypesToInclude.Where(x => x != polygonAreaTypeId);
                foreach (var parentAreaTypeId in extraParentAreaTypesToSearchFor)
                {
                    var parentSearchResults = GetSearchResults(userSearch, parentAreaTypeId,
                        ResultsToInclude.ParentAreasOnly);

                    searchResults.InsertRange(0, parentSearchResults);
                }

                return searchResults;
            }

            return new List<GeographicalSearchResult>();
        }

        private List<GeographicalSearchResult> GetSearchResults(SearchUserInput userSearch, int polygonAreaTypeId, ResultsToInclude resultsToInclude)
        {
            var isPostcode = userSearch.ContainsAnyNumbers;

            // Setup the fields to search through
            BooleanQuery finalQuery = isPostcode
                ? GetPostcodeQuery(userSearch.SearchText, FieldNames.Postcode)
                : GetPlaceNameQuery(userSearch, polygonAreaTypeId);

            // Perform the search
            var directory = FSDirectory.Open(new DirectoryInfo(
                Path.Combine(ApplicationConfiguration.Instance.SearchIndexDirectory, "placePostcodes")));
            var searcher = new IndexSearcher(directory, true);

            //Add the sorting parameters
            var sortBy = new Sort(
                new SortField(FieldNames.PlaceTypeWeighting, SortField.INT, true)
                );
            var docs = searcher.Search(finalQuery, null, ShortResultCount, sortBy);

            List<GeographicalSearchResult> searchResults = new List<GeographicalSearchResult>();
            int resultCount = docs.ScoreDocs.Length;
            for (int i = 0; i < resultCount; i++)
            {
                ScoreDoc scoreDoc = docs.ScoreDocs[i];

                Document doc = searcher.Doc(scoreDoc.Doc);

                var geographicalSearchResult = NewGeographicalSearchResult(doc, polygonAreaTypeId, isPostcode);

                CheckPolygonAreaCodeIsDefined(polygonAreaTypeId, geographicalSearchResult);

                AssignEastingAndNorthing(doc, geographicalSearchResult);

                AddResultIfRequired(resultsToInclude, geographicalSearchResult, searchResults);
            }
            return searchResults;
        }

        private GeographicalSearchResult NewGeographicalSearchResult(Document doc, int polygonAreaTypeId, bool isPostcode)
        {
            var name = GetName(isPostcode, doc);

            var geographicalSearchResult = new GeographicalSearchResult
            {
                PlaceName = name,
                County = _textInfo.ToTitleCase(doc.Get(FieldNames.County)),
                PolygonAreaCode = doc.Get("Parent_Area_Code_" + polygonAreaTypeId),
                PolygonAreaName = doc.Get("Parent_Area_Name_" + polygonAreaTypeId)
            };
            return geographicalSearchResult;
        }

        private static string GetName(bool isPostcode, Document doc)
        {
            var name = isPostcode
                ? doc.Get(FieldNames.Postcode).ToUpper()
                : doc.Get(FieldNames.NameFormatted);
            return name;
        }

        private static void AddResultIfRequired(ResultsToInclude resultsToInclude,
            GeographicalSearchResult geographicalSearchResult, List<GeographicalSearchResult> searchResults)
        {
            bool addResult = true;
            switch (resultsToInclude)
            {
                case ResultsToInclude.ParentAreasOnly:
                    addResult = geographicalSearchResult.AreCoordinatesValid() == false;
                    break;
                case ResultsToInclude.PlaceNamesAndPostcodesOnly:
                    addResult = geographicalSearchResult.AreCoordinatesValid();
                    break;
            }
            if (addResult)
            {
                searchResults.Add(geographicalSearchResult);
            }
        }

        private static void CheckPolygonAreaCodeIsDefined(int childAreaTypeId, GeographicalSearchResult geographicalSearchResult)
        {
            if (geographicalSearchResult.PolygonAreaCode == null)
            {
                throw new FingertipsException(string.Format(
                    "Area type id not supported for search: {0}", childAreaTypeId));
            }
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