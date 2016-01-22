using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.KeyMessages
{
    public class SentenceJoiner
    {
        private List<string> sentences = new List<string>();

        public string Join()
        {
            return sentences.Any()
                ? sentences.Aggregate((i, j) => i + " " + j)
                : string.Empty;
        }

        public void Add(string sentence)
        {
            if (string.IsNullOrEmpty(sentence) == false)
            {
                sentences.Add(sentence);
            }
        }
    }
}