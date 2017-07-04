using System.Collections.Generic;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class QuinaryPopulationDataAction
    {
        /// <summary>
        /// LEGACY: Only user for practice profiles
        /// </summary>
        public Dictionary<string, object> GetPopulationAndSummary(string areaCode, int groupId, int dataPointOffset)
        {
            QuinaryPopulationBuilder builder = new QuinaryPopulationBuilder
            {
                DataPointOffset = dataPointOffset,
                AreaCode = areaCode,
                GroupId = groupId,
                AreOnlyPopulationsRequired = false
            };

            builder.BuildPopulationAndSummary();

            Dictionary<string, object> responseObjects = new Dictionary<string, object>();

            responseObjects.Add("Code", areaCode);

            // Quinary population
            responseObjects.Add("Values", builder.Values);
            responseObjects.Add("Labels", builder.Labels);

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

        public Dictionary<string, object> GetPopulationOnly(string areaCode, int areaTypeId,
            int profileId, int indicatorId, int dataPointOffset)
        {
            QuinaryPopulationBuilder builder = new QuinaryPopulationBuilder
            {
                DataPointOffset = dataPointOffset,
                AreaCode = areaCode
            };

            builder.BuildPopulationOnly(areaCode, areaTypeId, profileId, indicatorId);
            return GetResponseObjects(areaCode, builder);
        }

        public Dictionary<string, object> GetPopulationOnly(string areaCode, int areaTypeId, int dataPointOffset)
        {
            QuinaryPopulationBuilder builder = new QuinaryPopulationBuilder
            {
                DataPointOffset = dataPointOffset,
                AreaCode = areaCode,
                GroupId = GroupIds.Population
            };

            builder.BuildPopulationOnly(areaCode,areaTypeId);
            return GetResponseObjects(areaCode, builder);
        }

        private static Dictionary<string, object> GetResponseObjects(string areaCode, QuinaryPopulationBuilder builder)
        {
            Dictionary<string, object> responseObjects = new Dictionary<string, object>();

            responseObjects.Add("Code", areaCode);

            // Quinary population
            responseObjects.Add("Values", builder.Values);
            responseObjects.Add("Labels", builder.Labels);
            responseObjects.Add("IndicatorName", builder.PopulationIndicatorName);
            responseObjects.Add("Period", builder.Period);

            var listSize = builder.ListSize;
            if (listSize != null)
            {
                responseObjects.Add("ListSize", listSize);
            }

            return responseObjects;
        }

        public Dictionary<string, object> GetSummaryOnly(string areaCode, int areaTypeId, int dataPointOffset)
        {
            if(areaTypeId != AreaTypeIds.GpPractice)
                return null;

            QuinaryPopulationBuilder builder = new QuinaryPopulationBuilder
            {
                DataPointOffset = dataPointOffset,
                AreaCode = areaCode,
                GroupId = GroupIds.PopulationSummary
            };
            Dictionary<string, object> responseObjects = new Dictionary<string, object>();
            builder.BuildSummaryOnly();

            responseObjects.Add("Code", areaCode);

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

            return responseObjects;
        }
    }
}