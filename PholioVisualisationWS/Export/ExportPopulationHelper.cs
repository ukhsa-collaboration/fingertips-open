using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class ExportPopulationHelper
    {
        public static List<CoreDataSet> FilterQuinaryPopulations(IList<CoreDataSet> data)
        {
            var newList = new List<CoreDataSet>();

            var sexIds = new[] { SexIds.Male, SexIds.Female };

            foreach (var sexId in sexIds)
            {
                var sexData = FilterBySex(data, sexId);

                if (sexData != null)
                {
                    newList.AddRange(sexData);
                }
            }

            return newList;
        }

        private static IEnumerable<CoreDataSet> FilterBySex(IList<CoreDataSet> data, int sexId)
        {
            var filteredData = data.Where(x => x.SexId != sexId);
            filteredData = new QuinaryPopulationSorter(filteredData.ToList()).SortedData;
            return filteredData;
        }
    }
}