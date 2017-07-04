using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace FingertipsDataExtractionTool.AverageCalculator
{
    public interface IParentAreaProvider
    {
        List<IArea> GetParentAreas(int areaTypeId);
    }

    public class ParentAreaProvider : IParentAreaProvider
    {
        private IAreasReader _areasReader;
        private IAreaTypeListProvider _areaTypeListProvider;

        public ParentAreaProvider(IAreasReader areasReader, IAreaTypeListProvider areaTypeListProvider)
        {
            _areasReader = areasReader;
            _areaTypeListProvider = areaTypeListProvider;
        }

        public List<IArea> GetParentAreas(int areaTypeId)
        {
            List<IArea> areas = new List<IArea>();

            // Subnational areas
            var parentAreaTypeIds = _areaTypeListProvider.GetParentAreaTypeIdsUsedForChildAreaType(areaTypeId);
            foreach (var parentAreaTypeId in parentAreaTypeIds)
            {
                areas.AddRange(_areasReader.GetAreasByAreaTypeId(parentAreaTypeId));
            }

            // Category areas
            var categoryTypeIds = _areaTypeListProvider.GetCategoryTypeIdsUsedForChildAreaType(areaTypeId);
            foreach (var categoryTypeId in categoryTypeIds)
            {
                var categoryAreas = _areasReader.GetCategories(categoryTypeId)
                    .Select(CategoryArea.New);
                areas.AddRange(categoryAreas);
            }

            // England
            areas.Add(_areasReader.GetAreaFromCode(AreaCodes.England));

            return areas;
        }
    }
}