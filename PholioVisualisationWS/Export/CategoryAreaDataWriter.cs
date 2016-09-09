using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class CategoryAreaDataWriter : ParentDataWriter
    {
        private Dictionary<int, IArea> subnationalCategoryIdToCategoryAreaMap = null;
        private int _categoryAreaTypeId;
        private IList<Category> categories;

        public CategoryAreaDataWriter(IAreasReader areasReader, IGroupDataReader groupDataReader, WorksheetInfo worksheet,
            ProfileDataWriter profileDataWriter, CategoryAreaType categoryAreaType)
            : base(areasReader, groupDataReader, worksheet, profileDataWriter)
        {
            _categoryAreaTypeId = categoryAreaType.CategoryTypeId;

            categories = areasReader.GetCategories(_categoryAreaTypeId);

                subnationalCategoryIdToCategoryAreaMap = categories
                    .ToDictionary<Category, int, IArea>(
                    category => category.Id,
                    category => CategoryArea.New(category)
                    );
        }

        public override IList<CategoryIdAndAreaCode> CategoryIdAndAreaCodes
        {
            get
            {
                return categories.OrderBy(x => x.Id)
                    .Select(x => new CategoryIdAndAreaCode { CategoryId = x.Id})
                    .ToList();
            }
        }

        public override IList<CoreDataSet> AddMultipleAreaData(RowLabels rowLabels, Grouping grouping, 
            TimePeriod timePeriod, IndicatorMetadata metadata, Dictionary<string, Area> areaCodeToParentMap)
        {
            var dataList = GroupDataReader.GetCoreDataListForAllCategoryAreasOfCategoryAreaType(
                grouping, timePeriod, _categoryAreaTypeId, AreaCodes.England);

            ProfileDataWriter.AddCategorisedData(Worksheet, rowLabels, dataList, subnationalCategoryIdToCategoryAreaMap);

            return dataList;
        }
    }
}