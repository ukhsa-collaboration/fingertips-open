using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public enum PartitionDataType
    {
        Sex,
        Age,
        Category
    }

    public class PartitionTrendDataDictionaryBuilder
    {
        private Dictionary<int, IList<CoreDataSet>> _trendDataDictionary;
        private readonly IList<INamedEntity> _namedEntities;
        private readonly List<CoreDataSet> _allData = new List<CoreDataSet>();
        private Func<IList<CoreDataSet>, INamedEntity, CoreDataSet> _getDataMethod;
        private int _timePeriodCount = 0;

        public Dictionary<int, IList<CoreDataSet>> Dictionary
        {
            get { return _trendDataDictionary; }
        }

        public IList<CoreDataSet> AllDataAsList
        {
            get { return _allData; }
        }

        public PartitionTrendDataDictionaryBuilder(IList<INamedEntity> namedEntities,
            PartitionDataType partitionDataType)
        {
            _namedEntities = namedEntities;
            InitDictionary();
            InitGetDataMethod(partitionDataType);
        }

        public void AddDataForNextTimePeriod(IList<CoreDataSet> dataList)
        {
            _timePeriodCount++;
            _allData.AddRange(dataList);

            foreach (var namedEntity in _namedEntities)
            {
                var data = _getDataMethod(dataList, namedEntity);
                _trendDataDictionary[namedEntity.Id].Add(data);
            }
        }

        public void RemoveEntity(int id)
        {
            _trendDataDictionary.Remove(id);
        }

        public int RemoveEarlyEmptyYears()
        {
            var index = GetEarliestIndexWithoutData();

            if (index > -1)
            {
                var copy = new Dictionary<int, IList<CoreDataSet>>(_trendDataDictionary);

                foreach (var key in copy.Keys)
                {
                    var dataList = copy[key];
                    _trendDataDictionary[key] = dataList.Skip(index + 1)
                        .ToList();
                }
            }

            return index;
        }

        private int GetEarliestIndexWithoutData()
        {
            var earliestIndexWithoutData = -1;

            for (int i = 0; i < _timePeriodCount; i++)
            {
                foreach (var key in _trendDataDictionary.Keys)
                {
                    var dataList = _trendDataDictionary[key];
                    if (dataList[i] != null && dataList[i].IsValueValid)
                    {
                        return earliestIndexWithoutData;
                    }
                }

                earliestIndexWithoutData++;
            }

            return earliestIndexWithoutData;
        }

        private void InitGetDataMethod(PartitionDataType partitionDataType)
        {
            switch (partitionDataType)
            {
                case PartitionDataType.Age:
                    _getDataMethod = GetDataByAge;
                    break;
                case PartitionDataType.Sex:
                    _getDataMethod = GetDataBySex;
                    break;
                case PartitionDataType.Category:
                    _getDataMethod = GetDataByCategory;
                    break;
            }
        }

        private void InitDictionary()
        {
            _trendDataDictionary = new Dictionary<int, IList<CoreDataSet>>();
            foreach (var namedEntity in _namedEntities)
            {
                _trendDataDictionary.Add(namedEntity.Id, new List<CoreDataSet>());
            }
        }

        protected static CoreDataSet GetDataBySex(IList<CoreDataSet> dataList, INamedEntity namedEntity)
        {
            return dataList.FirstOrDefault(x => x.SexId == namedEntity.Id);
        }

        protected static CoreDataSet GetDataByAge(IList<CoreDataSet> dataList, INamedEntity namedEntity)
        {
            return dataList.FirstOrDefault(x => x.AgeId == namedEntity.Id);
        }

        protected static CoreDataSet GetDataByCategory(IList<CoreDataSet> dataList, INamedEntity namedEntity)
        {
            return dataList.FirstOrDefault(x => x.CategoryId == namedEntity.Id);
        }
    }
}