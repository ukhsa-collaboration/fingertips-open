using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class ParentAreasOfChildAreasBuilder
    {
        public Dictionary<string, IArea> GetAreaMapping(string parentAreaCode, int childAreaTypeId,
            int parentAreaTypeId)
        {
            var areasReader = ReaderFactory.GetAreasReader();
            var childAreas = new ChildAreaListBuilder(areasReader).GetChildAreas(parentAreaCode, childAreaTypeId);

            Dictionary<string, IArea> map = new Dictionary<string, IArea>();

            IArea parentArea = AreaFactory.NewArea(areasReader, parentAreaCode);
            if (parentArea is Area)
            {
                foreach (var childArea in childAreas)
                {
                    var parentAreaOfSpecificType = areasReader.GetParentAreas(childArea.Code)
                        .FirstOrDefault(x => x.AreaTypeId == parentAreaTypeId);
                    map.Add(childArea.Code, parentAreaOfSpecificType);
                }
            }
            else
            {
                throw new FingertipsException("CategoryAreas not supported");
            }

            return map;
        }
    }
}