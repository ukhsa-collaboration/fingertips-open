using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Lucene.Net.Search;

namespace PholioVisualisation.SearchQuerying
{
    public class SearchUserInput
    {
        private List<string> terms = new List<string>();

        public SearchUserInput(string searchText)
        {
            SearchText = searchText;
            ProcessSearchText();
        }

        // Public input values
        public string SearchText { get; private set; }

        // Public output values
        public BooleanClause.Occur BooleanCombinationLogic { get; private set; }

        public IList<string> Terms
        {
            get { return terms; }
        }

        public bool IsQueryValid
        {
            get
            {
                if (terms.Any() == false)
                {
                    // No search terms
                    return false;
                }

                if (terms.Count == 1)
                {
                    var term = terms.First();

                    // e.g. "a*"
                    return term.Length > 2;
                }

                return true;
            }
        }

        public bool IsCommaSeparatedNumberList
        {
            get { return Regex.IsMatch(SearchText, @"^[\d, ]+$"); }
        }

        public bool ContainsAnyNumbers
        {
            get { return Regex.IsMatch(SearchText, @"\d"); }
        }

        private void ProcessSearchText()
        {
            BooleanCombinationLogic = BooleanClause.Occur.MUST;

            if (string.IsNullOrEmpty(SearchText) == false)
            {
                var searchString = CleanSearchText;
                if (string.IsNullOrEmpty(searchString) == false)
                {
                    string[] split = searchString.Split(new[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in split)
                    {
                        if (IsWordQueryPart(s) == false)
                        {
                            AddWord(s);
                        }
                        else if (IsOr(s))
                        {
                            // One occurence of OR switches logic
                            BooleanCombinationLogic = BooleanClause.Occur.SHOULD;
                        }
                    }
                }
            }
        }

        public string CleanSearchText
        {
            get
            {
                string searchString = SearchText.Trim();
                return new Regex("[^A-Za-z0-9 ]").Replace(searchString, " ");
            }
        }

        private void AddWord(string word)
        {
            word = word.ToLower() + "*";
            terms.Add(word);
        }

        private static bool IsWordQueryPart(string word)
        {
            return IsAnd(word) || IsOr(word);
        }

        private static bool IsOr(string word)
        {
            return word.Equals("or", StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool IsAnd(string word)
        {
            return word.Equals("and", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}