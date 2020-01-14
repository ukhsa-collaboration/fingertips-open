using System.Collections.Generic;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{

    public class QuinaryPopulationDataAction : QuinaryPopulationDataActionBase
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

            builder.BuildPopulationTotalsAndPercentages(areaCode);
            return GetResponseObjects(areaCode, builder);
        }

    }
}