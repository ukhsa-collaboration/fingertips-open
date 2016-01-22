using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class PracticeAreaValuesBuilder : AreaValuesBuilder
    {
        public PracticeAreaValuesBuilder(IGroupDataReader groupDataReader)
            : base(groupDataReader)
        {
        }

        /// <summary>
        /// Returns hash to limit JSON footprint to only data required by profile
        /// </summary>
        public IDictionary<string, ValueData> Build(Grouping grouping)
        {
            InitBuild(grouping);

            if (grouping != null)
            {
                var dataHash = new PracticeDataAccess().GetPracticeCodeToValueDataMap(grouping, Period, ParentAreaCode);
                FormatData(dataHash);
                return dataHash;
            }

            return null;
        }

        private void FormatData(Dictionary<string, ValueData> dataHash)
        {
            var dataList = dataHash.Values.ToList();
            new ValueDataProcessor(Formatter).FormatAndTruncateList(dataList);
        }
    }
}
