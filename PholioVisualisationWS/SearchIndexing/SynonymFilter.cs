using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace PholioVisualisation.SearchIndexing
{
    /// <summary>
    /// Add synonyms to the Lucene token stream
    /// </summary>
    public class SynonymFilter : TokenFilter
    {
        private Queue<string> splittedQueue = new Queue<string>();
        private readonly TermAttribute _termAttr;
        private readonly PositionIncrementAttribute _posAttr;
        private State _currentState;
        private IList<IList<string>> _synonymLists;

        public SynonymFilter(TokenStream input)
            : base(input)
        {
            _termAttr = (TermAttribute)AddAttribute(typeof(TermAttribute));
            _posAttr = (PositionIncrementAttribute)AddAttribute(typeof(PositionIncrementAttribute));
            _synonymLists = SynonymReader.GetSynonymLists();
        }

        public override bool IncrementToken()
        {
            if (splittedQueue.Count > 0)
            {
                string splitted = splittedQueue.Dequeue();
                RestoreState(_currentState);
                _termAttr.SetTermBuffer(splitted);
                _posAttr.SetPositionIncrement(0);
                return true;
            }

            if (!input.IncrementToken())
                return false;

            var currentTerm = new string(_termAttr.TermBuffer(), 0, _termAttr.TermLength());

            IEnumerable<string> synonyms = GetSynonyms(currentTerm);

            if (synonyms == null)
            {
                return false;
            }
            foreach (string syn in synonyms)
            {
                if (!currentTerm.Equals(syn))
                {
                    splittedQueue.Enqueue(syn);
                }
            }

            return true;
        }

        private IEnumerable<string> GetSynonyms(string currentTerm)
        {
            foreach (var synonyms in _synonymLists)
            {
                if (synonyms.Contains(currentTerm))
                {
                    return synonyms;
                }
            }

            return new List<string> { currentTerm };
        }
    }
}