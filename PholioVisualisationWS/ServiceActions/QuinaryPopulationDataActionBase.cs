using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace PholioVisualisation.ServiceActions
{
    public class QuinaryPopulationDataActionBase
    {
        public Dictionary<string, object> GetSummaryOnly(string areaCode, int areaTypeId, int dataPointOffset)
        {
            if (areaTypeId != AreaTypeIds.GpPractice)
                return null;

            QuinaryPopulationBuilder builder = new QuinaryPopulationBuilder
            {
                DataPointOffset = dataPointOffset,
                AreaCode = areaCode,
                GroupId = GroupIds.PopulationSummary
            };
            Dictionary<string, object> responseObjects = new Dictionary<string, object>();
            builder.BuildPopulationSummary();

            responseObjects.Add("Code", areaCode);

            if (builder.EthnicityText != null)
            {
                responseObjects.Add("Ethnicity", builder.EthnicityText);
            }

            if (builder.DeprivationDecile.HasValue)
            {
                responseObjects.Add("GpDeprivationDecile", builder.DeprivationDecile);
            }

            responseObjects.Add("AdHocValues", builder.AdHocValues);

            return responseObjects;
        }

        protected static Dictionary<string, object> GetResponseObjects(string areaCode, QuinaryPopulationBuilder builder)
        {
            Dictionary<string, object> responseObjects = new Dictionary<string, object>();

            responseObjects.Add("Code", areaCode);

            // Quinary population
            responseObjects.Add("Values", builder.PopulationPercentages);
            responseObjects.Add("PopulationTotals", builder.PopulationTotals);
            responseObjects.Add("Labels", builder.Labels);
            responseObjects.Add("IndicatorName", builder.PopulationIndicatorName);
            responseObjects.Add("Period", builder.Period);
            responseObjects.Add("ChildAreaCount", builder.ChildAreaCount);

            var listSize = builder.ListSize;
            if (listSize != null)
            {
                responseObjects.Add("ListSize", listSize);
            }

            return responseObjects;
        }
    }
}
