using System.Collections;
using System.Collections.Generic;
using PholioVisualisation.Parsers;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    /// Definition for handling of indicator metadata special cases.
    /// </summary>
    public class IndicatorMetadataSpecialCase
    {
        private IDictionary<string, string> _parameters;

        public IndicatorMetadataSpecialCase(string specialCaseString)
        {
            _parameters = KeyValuePairListParser.Parse(specialCaseString);
        }

        public bool ShouldUseSpecificAgeId()
        {
            return _parameters.ContainsKey(IndicatorMetadataSpecialCases.InequalityBenchmark_UseAgeId);
        }

        public bool ShouldUseForSpecificCategoryTypeId()
        {
            return _parameters.ContainsKey(IndicatorMetadataSpecialCases.InequalityBenchmark_ForCategoryTypeId);
        }

        public bool ShouldOmitSpecificAgeId()
        {
            return _parameters.ContainsKey(IndicatorMetadataSpecialCases.InequalityData_OmitAgeId);
        }

        public int CategoryTypeId
        {
            get { return int.Parse(_parameters[IndicatorMetadataSpecialCases.InequalityBenchmark_ForCategoryTypeId]); }
        }

        public int BenchmarkAgeId
        {
            get { return int.Parse(_parameters[IndicatorMetadataSpecialCases.InequalityBenchmark_UseAgeId]); }
        }

        public int AgeIdToOmit
        {
            get { return int.Parse(_parameters[IndicatorMetadataSpecialCases.InequalityData_OmitAgeId]); }
        }
    }
}