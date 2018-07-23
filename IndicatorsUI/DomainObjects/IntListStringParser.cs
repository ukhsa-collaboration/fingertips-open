using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndicatorsUI.DomainObjects
{
    /// <summary>
    /// NOTE: this class also exists in PholioVisualisation solution
    /// </summary>
    public class IntListStringParser
    {
        public List<int> IntList { get; set; }

        public IntListStringParser(string listString)
        {
            IntList = new List<int>();
            if (string.IsNullOrEmpty(listString) == false)
            {
                string[] bits = listString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string bit in bits)
                {
                    int id;
                    if (int.TryParse(bit, out id))
                    {
                        IntList.Add(id);
                    }
                }
            }
        }
    }
}
