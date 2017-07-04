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

        /// <summary>
        /// Set formatted numeric value strings and round numeric values for list of CoreDataSet objects.
        /// </summary>
        public override void FormatAndTruncateList(IList<CoreDataSet> dataList)
        {
            foreach (var data in dataList)
            {
                FormatAndTruncate(data);
            }
        }

        /// <summary>
        /// Set formatted numeric value strings and round numeric values
        /// </summary>
        public override void FormatAndTruncate(CoreDataSet data)
        {
            if (data != null)
            {
                formatter.Format(data);
                formatter.FormatConfidenceIntervals(data);
                Truncate(data);
            }
        }

        /// <summary>
        /// Round numeric values
        /// </summary>
        public override void TruncateList(IList<CoreDataSet> dataList)
        {
            foreach (var data in dataList)
            {
                Truncate(data);
            }
        }

        /// <summary>
        /// Round numeric values
        /// </summary>
        public override void Truncate(CoreDataSet data)
        {
            if (data != null)
            {
                TruncateValuesOfValueDataWithCIs(data);
                RoundCount(data);
                data.Denominator2 = Round(data.Denominator2);
            }
        }

        /// <summary>
        /// Fact that Fingertips calculated data with no value is not relevant from now
        /// </summary>
        public void RemoveRedundantValueNotesForDataList(IList<CoreDataSet> dataList)
        {
            foreach (var coreDataSet in dataList)
            {
                RemoveRedundantValueNote(coreDataSet);
            }
        }

        /// <summary>
        /// Fact that Fingertips calculated data with no value is not relevant from now
        /// </summary>
        public void RemoveRedundantValueNote(CoreDataSet coreDataSet)
        {
                if (coreDataSet.ValueNoteId == ValueNoteIds.ValueAggregatedFromAllKnownGeographyValuesByFingertips &&
                    coreDataSet.IsValueValid == false)
                {
                    coreDataSet.ValueNoteId = ValueNoteIds.NoNote;
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
