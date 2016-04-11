using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class CoreDataProcessor : DataProcessor<CoreDataSet>
    {
        private NumericFormatter formatter;

        public CoreDataProcessor(NumericFormatter formatter)
        {
            this.formatter = formatter;
        }

        public override void FormatAndTruncateList(IList<CoreDataSet> dataList)
        {
            foreach (var data in dataList)
            {
                FormatAndTruncate(data);
            }
        }

        public override void FormatAndTruncate(CoreDataSet data)
        {
            if (data != null)
            {
                formatter.Format(data);
                formatter.FormatConfidenceIntervals(data);
                Truncate(data);
            }
        }

        public override void TruncateList(IList<CoreDataSet> dataList)
        {
            foreach (var data in dataList)
            {
                Truncate(data);
            }
        }

        public override void Truncate(CoreDataSet data)
        {
            if (data != null)
            {
                TruncateValuesOfValueDataWithCIs(data);
                RoundCount(data);
                data.Denominator2 = Round(data.Denominator2);
            }
        }

        private static void RoundCount(CoreDataSet data)
        {
            if (data.Count.HasValue && 
                data.Count.Value.Equals(ValueWithCIsData.CountMinusOne) == false)
            {
                data.Count = Round(data.Count.Value);
            }
        }
    }
}
