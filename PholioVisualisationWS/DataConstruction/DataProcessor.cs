using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public abstract class DataProcessor<T>
    {
        public const int DecimalPlaceCount = 4;

        public abstract void FormatAndTruncateList(IList<T> dataList);
        public abstract void FormatAndTruncate(T data);
        public abstract void Truncate(T data);
        public abstract void TruncateList(IList<T> dataList);

        protected static void TruncateValuesOfValueDataWithCIs(ValueWithCIsData data)
        {
            if (data != null && data.HasBeenTruncated == false)
            {
                data.Value = Round(data.Value);
                data.LowerCI = Round(data.LowerCI);
                data.UpperCI = Round(data.UpperCI);
                data.HasBeenTruncated = true;
            }
        }

        public static double Round(double d)
        {
            return Math.Round(d, DecimalPlaceCount, MidpointRounding.AwayFromZero);
        }
    }
}
