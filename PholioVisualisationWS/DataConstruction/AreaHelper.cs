using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public static class AreaHelper
    {
        /// <summary>
        /// Returns the first CoreDataSet object that matches the specified area code.
        /// </summary>
        public static CoreDataSet GetDataForAreaFromDataList(string areaCode, IEnumerable<CoreDataSet> dataList)
        {
            return dataList.FirstOrDefault(
                x => x.AreaCode.Equals(areaCode, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
