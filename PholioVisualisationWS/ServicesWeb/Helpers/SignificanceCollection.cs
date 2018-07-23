using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesWeb.Helpers
{
    public interface ISignificanceCollection
    {
        List<ComparatorSignificance> Significances { get; set; }

        void Add(Significance significance, string name);
    }

    public class SignificanceCollection : ISignificanceCollection
    {
        private List<ComparatorSignificance> significances;

        public List<ComparatorSignificance> Significances
        {
            get { return significances ?? (significances = new List<ComparatorSignificance>()); }
            set { significances = value; }
        }

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