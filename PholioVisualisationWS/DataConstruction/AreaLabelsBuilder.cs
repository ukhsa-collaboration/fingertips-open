
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class AreaLabelsBuilder
    {
        public const string ShapeAreaCodePrefix = "S_";

        public IDictionary<int, string> Labels { get; private set; }

        public AreaLabelsBuilder(IEnumerable<Area> areas, string prefix)
        {
            Labels = new Dictionary<int, string>();

            foreach (var d in areas)
            {
                int id = int.Parse(d.Code.Replace(prefix, string.Empty));
                Labels.Add(id, d.Name);
            }
        }

        public AreaLabelsBuilder(IEnumerable<Category> categories)
        {
            Labels = new Dictionary<int, string>();

            foreach (var category in categories)
            {
                int id = category.CategoryId;
                Labels.Add(id, category.Name);
            }
        } 

    }
}
