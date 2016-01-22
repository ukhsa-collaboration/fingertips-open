using System.Collections.Generic;
using System.Linq;
using NHibernate.Driver;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class RankDataList
    {
        public static List<CoreDataSet> ValidDataList(IEnumerable<CoreDataSet> dataList, int polarityId)
        {
            List<CoreDataSet> validDataList;
            if (polarityId == PolarityIds.RagHighIsGood)
            {
                validDataList = dataList
                    .Where(x => x.IsValueValid)
                    .OrderByDescending(x => x.Value)
                    .ToList();               
            }
            else
            {
                validDataList = dataList
                    .Where(x => x.IsValueValid)
                    .OrderBy(x => x.Value)
                    .ToList();                
            }

            return validDataList;
        }
    }
}