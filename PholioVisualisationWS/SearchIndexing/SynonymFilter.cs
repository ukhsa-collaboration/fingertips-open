using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using System.Collections.Generic;

namespace PholioVisualisation.SearchIndexing
{
    /// <summary>
    /// Add synonyms to the Lucene token stream
    /// </summary>
    public class SynonymFilter : TokenFilter
    {
        private readonly Queue<string> _splittedQueue = new Queue<string>();
        private readonly TermAttribute _termAttr;
        private readonly PositionIncrementAttribute _posAttr;
        private State CurrentState { get; set; }
        private readonly IList<IList<string>> _synonymLists;

        public SynonymFilter(TokenStream input)
            : base(input)
        {
            _termAttr = (TermAttribute)AddAttribute(typeof(TermAttribute));
            _posAttr = (PositionIncrementAttribute)AddAttribute(typeof(PositionIncrementAttribute));
            _synonymLists = SynonymReader.GetSynonymLists();
        }

        public override bool IncrementToken()
        {
            if (_splittedQueue.Count > 0)
            {
                string splitted = _splittedQueue.Dequeue();
                RestoreState(CurrentState);
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
                    _splittedQueue.Enqueue(syn);
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