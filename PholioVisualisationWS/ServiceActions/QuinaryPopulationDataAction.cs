using System.Collections.Generic;
using PholioVisualisation.DataConstruction;

namespace PholioVisualisation.ServiceActions
{
    public class QuinaryPopulationDataAction
    {
        public Dictionary<string, object> GetResponse(string areaCode, int groupId, int dataPointOffset)
        {
            QuinaryPopulationBuilder builder = new QuinaryPopulationBuilder
            {
                DataPointOffset = dataPointOffset,
                AreaCode = areaCode,
                GroupId = groupId,
                AreOnlyPopulationsRequired = false
            };

            builder.Build();

            Dictionary<string, object> responseObjects = new Dictionary<string, object>();

            responseObjects.Add("Code", areaCode);
            responseObjects.Add("Values", builder.Values);

            // Practice specific information
            if (builder.EthnicityText != null)
            {
                responseObjects.Add("Ethnicity", builder.EthnicityText);
            }

            if (builder.DeprivationDecile.HasValue)
            {
                responseObjects.Add("GpDeprivationDecile", builder.DeprivationDecile);
            }

            if (builder.Shape.HasValue)
            {
                responseObjects.Add("Shape", builder.Shape);
            }

            responseObjects.Add("AdHocValues", builder.AdHocValues);

            var listSize = builder.ListSize;
            if (listSize != null)
            {
                responseObjects.Add("ListSize", listSize);
            }

            return responseObjects;
        }
    }
}