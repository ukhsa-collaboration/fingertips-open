using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class MultipleCoreDataCollector
    {
        private CoreDataCollector _englandCoreDataCollector = new CoreDataCollector();
        private CoreDataCollector _parentCoreDataCollector = new CoreDataCollector();
        private CoreDataCollector _childCoreDataCollector = new CoreDataCollector();


        public IList<CoreDataSet> GetDataListForEngland(CoreDataSet coreData)
        {
            return _englandCoreDataCollector.GetDataListForArea(coreData);
        }

        public IList<CoreDataSet> GetDataListForParentArea(CoreDataSet coreData)
        {
            return _parentCoreDataCollector.GetDataListForArea(coreData);
        }

        public IList<CoreDataSet> GetDataListForChildArea(CoreDataSet coreData)
        {
            return _childCoreDataCollector.GetDataListForArea(coreData);
        }

        public void AddEnglandDataList(IList<CoreDataSet> coreDataList)
        {
            _englandCoreDataCollector.AddDataList(coreDataList);
        }

        public void AddParentDataList(IList<CoreDataSet> coreDataList)
        {
            _parentCoreDataCollector.AddDataList(coreDataList);
        }

        public void AddChildDataList(IList<CoreDataSet> coreDataList)
        {
            _childCoreDataCollector.AddDataList(coreDataList);
        }
    }
}