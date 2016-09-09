using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class CoreDataCollector
    {
        private IList<IList<CoreDataSet>> _multipleAreaDataLists = new List<IList<CoreDataSet>>();
        private IList<CoreDataSet> _singleAreaDataList = new List<CoreDataSet>();

        public void AddDataList(IList<CoreDataSet> dataList)
        {
            _multipleAreaDataLists.Add(dataList);
        }

        public void AddData(CoreDataSet data)
        {
            _singleAreaDataList.Add(data);
        }

        public List<CoreDataSet> GetDataList()
        {
            List<CoreDataSet> dataList = new List<CoreDataSet>();
            foreach (var data in _singleAreaDataList)
            {
                dataList.Add(data ?? CoreDataSet.GetNullObject());
            }
            return dataList;
        }

        public List<CoreDataSet> GetDataListForArea(CategoryIdAndAreaCode area)
        {
            List<CoreDataSet> dataList = new List<CoreDataSet>();

            foreach (var subnationalDataList in _multipleAreaDataLists)
            {
                var data = subnationalDataList
                    .FirstOrDefault(x => x != null && (x.AreaCode == area.AreaCode || x.CategoryId == area.CategoryId));
                dataList.Add(data ?? CoreDataSet.GetNullObject());
            }
            return dataList;
        }
    }
}