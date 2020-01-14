using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using PholioVisualisation.DataConstruction;

namespace PholioVisualisation.ServiceActions
{
    public class QuinaryPopulationDataCsvAction: QuinaryPopulationDataActionBase
    {
        public Dictionary<string, object> GetPopulationOnly(string areaCode, int areaTypeId, int dataPointOffset)
        {
            QuinaryPopulationBuilder builder = new QuinaryPopulationBuilder
            {
                DataPointOffset = dataPointOffset,
                AreaCode = areaCode,
                GroupId = GroupIds.Population,
                AreaTypeId = areaTypeId
            };

            builder.BuildPopulationTotalsWithoutPercentages(areaCode);
            return GetResponseObjects(areaCode, builder);
        }
    }
}
