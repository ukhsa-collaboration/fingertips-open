using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class MostRecentDataForAllCategoriesAction : MostRecentDataActionBase
    {
        public MostRecentDataForAllCategories GetResponse(int profileId = 0,
            string areaCode = null, int indicatorId = 0,
            int sexId = 0, int ageId = 0, int areaTypeId = 0)
        {
            ValidateParameters(profileId, areaTypeId, indicatorId, areaCode);    
            InitGrouping(profileId, areaTypeId, indicatorId, sexId, ageId);
            InitMetadataAndTimePeriod(grouping);

            // Get Data
            IList<CoreDataSet> categoryDataList = groupDataReader.GetAllCategoryDataWithinParentArea(areaCode,
                indicatorId, sexId, ageId, timePeriod);

            CalculateSignificances(areaCode, categoryDataList);
            FormatData(categoryDataList);

            return new MostRecentDataForAllCategories
            {
                AreaCode = areaCode,
                IndicatorId = indicatorId,
                AgeId = ageId,
                SexId = sexId,
                CategoryTypes = GetCategoryTypes(categoryDataList),
                Data = categoryDataList
            };
        }

        private void InitGrouping(int profileId, int areaTypeId, int indicatorId, int sexId, int ageId)
        {
            var groupingProvider = new SingleGroupingProvider(groupDataReader, new GroupIdProvider(profileReader));
            grouping = groupingProvider.GetGrouping(profileId, areaTypeId, indicatorId, sexId, ageId);
        }

        private IList<CategoryType> GetCategoryTypes(IList<CoreDataSet> categoryDataList)
        {
            List<int> categoryTypeIds = categoryDataList.Select(x => x.CategoryTypeId).Distinct().ToList();
            IList<CategoryType> categoryTypes = areasReader.GetCategoryTypes(categoryTypeIds);
            return categoryTypes;
        }
    }
}