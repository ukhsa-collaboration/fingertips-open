using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.UserData;
using PholioVisualisation.UserData.Repositories;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    ///     Provides a list of CoreDataSet objects.
    /// </summary>
    public class CoreDataSetListProvider
    {
        private IGroupDataReader _groupDataReader;

        /// <summary>
        ///     Parameterless constructor required for mocking.
        /// </summary>
        protected CoreDataSetListProvider()
        {
        }

        public CoreDataSetListProvider(IGroupDataReader groupDataReader)
        {
            _groupDataReader = groupDataReader;
        }

        public virtual IList<CoreDataSet> GetChildAreaData(Grouping grouping, IArea parentArea, TimePeriod period)
        {
            // Get child data of category area
            var categoryArea = parentArea as CategoryArea;
            if (categoryArea != null)
            {
                return _groupDataReader.GetCoreDataListForChildrenOfCategoryArea(
                    grouping, period, categoryArea);
            }

            if (Area.IsAreaListAreaCode(parentArea.Code))
            {
                return GetAreaListData(grouping, period, parentArea.Code);
            }

            // Get child data of nearest neighbour area
            var nearestNeighbourArea = parentArea as NearestNeighbourArea;
            if (nearestNeighbourArea != null)
            {
                return GetNearestNeighbourData(grouping, period, nearestNeighbourArea);
            }

            return parentArea.IsCountry
                ? _groupDataReader.GetCoreDataForAllAreasOfType(grouping, period)
                : _groupDataReader.GetCoreDataListForChildrenOfArea(grouping, period, parentArea.Code);
        }

        private IList<CoreDataSet> GetAreaListData(Grouping grouping, TimePeriod period, string parentAreaCode)
        {
            IAreaListRepository areaListRepository = new AreaListRepository(new fingertips_usersEntities());
            var areaList = areaListRepository.GetAreaListByPublicId(parentAreaCode);
            var areaListAreaCodes = areaList.AreaListAreaCodes.Select(x => x.AreaCode);

            IList<CoreDataSet> dataList = new List<CoreDataSet>();
            foreach (var areaCode in areaListAreaCodes)
            {
                var dataListForAreaCode = _groupDataReader.GetCoreDataListForChildrenOfArea(grouping, period, areaCode);

                foreach (var data in dataListForAreaCode)
                {
                    dataList.Add(data);
                }
            }

            return dataList;
        }

        private IList<CoreDataSet> GetNearestNeighbourData(Grouping grouping, TimePeriod period, NearestNeighbourArea nearestNeighbourArea)
        {
            // Get data of neighbours
            var dataList = _groupDataReader.GetCoreDataListForChildrenOfNearestNeighbourArea(
                grouping, period, nearestNeighbourArea).ToList();

            // Include the area that has neighbours
            var areaData = _groupDataReader.GetCoreData(grouping, period,
                nearestNeighbourArea.AreaCodeOfAreaWithNeighbours);
            dataList.AddRange(areaData);

            return dataList;
        }
    }
}