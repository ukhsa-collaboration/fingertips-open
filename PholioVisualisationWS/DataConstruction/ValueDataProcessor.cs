using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class ValueDataProcessor : DataProcessor<ValueData>
    {
        private NumericFormatter formatter;

        public ValueDataProcessor(NumericFormatter formatter)
        {
            this.formatter = formatter;
        }

        public override void FormatAndTruncateList(IList<ValueData> dataList)
        {
            foreach (var data in dataList)
            {
                FormatAndTruncate(data);
            }
        }

        public override void FormatAndTruncate(ValueData data)
        {
            if (data != null)
            {
                formatter.Format(data);
                Truncate(data);
            }
        }

        public override void TruncateList(IList<ValueData> dataList)
        {
            foreach (var data in dataList)
            {
                Truncate(data);
            }
        }

        public override void Truncate(ValueData data)
        {
            if (data != null)
            {
                data.Value = Round(data.Value);
            }
        }
    }
}
