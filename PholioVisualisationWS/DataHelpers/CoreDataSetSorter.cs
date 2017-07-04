using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataSorting
{
    public class CoreDataSetSorter
    {
        private IList<CoreDataSet> dataList;

        public CoreDataSetSorter(IList<CoreDataSet> dataList)
        {
            this.dataList = dataList;
        }

        public IList<CoreDataSet> SortByAgeId(IList<Age> ages)
        {
            var sortedDataList = new List<CoreDataSet>();

            foreach (var age in ages)
            {
                sortedDataList.AddRange(
                    dataList.Where(x => x.AgeId == age.Id));
            }

            return sortedDataList;
        }

        public IList<CoreDataSet> SortBySexId(IList<Sex> sexes)
        {
            var sortedDataList = new List<CoreDataSet>();

            foreach (var sex in sexes)
            {
                sortedDataList.AddRange(
                    dataList.Where(x => x.SexId == sex.Id));
            }

            return sortedDataList;
        }

        public IEnumerable<CoreDataSet> SortByDescendingYear()
        {
            return dataList.OrderByDescending(x => x.Year);
        }
    }
}