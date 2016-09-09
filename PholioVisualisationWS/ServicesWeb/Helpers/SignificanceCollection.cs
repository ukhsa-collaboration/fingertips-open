using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace ServicesWeb.Helpers
{
    public class SignificanceCollection
    {
        public List<ComparatorSignificance> Significances = new List<ComparatorSignificance>();

        public void Add(Significance significance, string name)
        {
            Significances.Add(new ComparatorSignificance
            {
                Id = significance,
                Name = name
            });
        }
    }
}