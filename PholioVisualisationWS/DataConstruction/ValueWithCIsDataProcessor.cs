using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class ValueWithCIsDataProcessor : DataProcessor<ValueWithCIsData>
    {
        private NumericFormatter formatter;

        public ValueWithCIsDataProcessor(NumericFormatter formatter)
        {
            this.formatter = formatter;
        }

        public override void FormatAndTruncateList(IList<ValueWithCIsData> dataList)
        {
            foreach (var data in dataList)
            {
                FormatAndTruncate(data);
            }
        }

        public override void FormatAndTruncate(ValueWithCIsData data)
        {
            if (data != null)
            {
                formatter.Format(data);
                formatter.FormatConfidenceIntervals(data);
                Truncate(data);
            }
        }

        public override void TruncateList(IList<ValueWithCIsData> dataList)
        {
            foreach (var data in dataList)
            {
                TruncateValuesOfValueDataWithCIs(data);
            }
        }

        public override void Truncate(ValueWithCIsData data)
        {
            TruncateValuesOfValueDataWithCIs(data);
        }
    }
}
