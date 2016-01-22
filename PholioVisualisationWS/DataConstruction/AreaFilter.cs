using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class AreaFilter : PholioFilter
    {
        private List<IArea> areaList; 

        public AreaFilter(IEnumerable<IArea> areas)
        {
            areaList = areas.ToList();
        }

        /// <summary>
        /// Removes areas that matches the specified area codes.
        /// </summary>
        public  IEnumerable<IArea> RemoveWithAreaCode(IEnumerable<string> areaCodesToRemove)
        {
            if (IsAreaCodeListOk(areaCodesToRemove))
            {
                var lowerCodes = GetCodesInLowerCase(areaCodesToRemove);

                return areaList.Where(x => lowerCodes.Contains(x.Code.ToLower()) == false);
            }

            return areaList;
        }
    }
}
