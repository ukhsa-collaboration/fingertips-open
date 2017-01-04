using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class ProfileFilter
    {
        /// <summary>
        /// Removes system profiles: search, archived indicators and unassigned indicators.
        /// </summary>
        /// <param name="profileIds"></param>
        public static IList<int> RemoveSystemProfileIds(IList<int> profileIds)
        {
            return profileIds.Where(x =>
              x != ProfileIds.Search &&
              x != ProfileIds.Archived &&
              x != ProfileIds.Unassigned)
              .ToList();
        }
    }
}