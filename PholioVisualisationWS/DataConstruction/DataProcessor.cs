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
                data.LowerCI95 = RoundNullable(data.LowerCI95);
                data.UpperCI95 = RoundNullable(data.UpperCI95);
                data.LowerCI99_8 = RoundNullable(data.LowerCI99_8);
                data.UpperCI99_8 = RoundNullable(data.UpperCI99_8);
                data.HasBeenTruncated = true;
            }
        }

        public static double Round(double d)
        {
            return Math.Round(d, DecimalPlaceCount, MidpointRounding.AwayFromZero);
        }

        public static double? RoundNullable(double? d)
        {
            if (d.HasValue)
            {
                return Math.Round(d.Value, DecimalPlaceCount, MidpointRounding.AwayFromZero);
            }
            return d;
        }
    }
}
