
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{
    public class RegionToChildAreasHashBuilder
    {
        private IProfileReader profileReader = ReaderFactory.GetProfileReader();
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();

        public Dictionary<ParentArea, IList<IArea>> Build(Profile profile, IEnumerable<string> parentAreaCodes, int childAreaTypeId)
        {
            Dictionary<ParentArea, IList<IArea>> regionalChildAreas = new Dictionary<ParentArea, IList<IArea>>();
            foreach (var parentAreaCode in parentAreaCodes)
            {
                IList<IArea> areas = areasReader.GetChildAreas(parentAreaCode, childAreaTypeId);

                // Remove areas to ignore
                IList<string> codesToIgnore = profileReader.GetAreaCodesToIgnore(profile.Id).AreaCodesIgnoredEverywhere;
                if (codesToIgnore.Count > 0)
                {
                    areas = (from a in areas where !codesToIgnore.Contains(a.Code) select a).ToList();
                }

                regionalChildAreas.Add(new ParentArea(parentAreaCode, childAreaTypeId), areas);
            }

            return regionalChildAreas;
        }
    }
}
