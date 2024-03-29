﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
  public class CoreDataSet : ValueWithCIsData, ICloneable
    {
        private double? countPerYear;
        public const int NoValueNote = 0;

        public CoreDataSet()
        {
            Significance = new Dictionary<int, int>();
        }

        [JsonProperty]
        public int AgeId { get; set; }

        [JsonProperty]
        public int SexId { get; set; }

        /// <summary>
        ///     Required by NHibernate to map to CoreDataSet table.
        /// </summary>
        [JsonIgnore]
        public int UniqueId { get; set; }

        [JsonProperty]
        public int IndicatorId { get; set; }

        [JsonIgnore]
        public bool IsCountValid
        {
            get { return Count.HasValue && Count != NullValue; }
        }

        [JsonIgnore]
        public bool IsDenominator2Valid
        {
            get { return Denominator2 > 0; }
        }

        [JsonIgnore]
        public double CountPerYear
        {
            get
            {
                // This must only ever be done once in case NHibernate has
                // cached previously modified object
                if (countPerYear.HasValue == false)
                {
                    countPerYear = IsCountValid
                    ? Count.Value / YearRange
                    : NullValue;
                }

                return countPerYear.Value;
            }
        }

        [JsonProperty]
        public string AreaCode { get; set; }

        [JsonProperty(PropertyName = "Denom")]
        public double Denominator { get; set; }

        [JsonIgnore]
        public bool IsDenominatorValid
        {
            get { return Denominator > 0; }
        }

        [JsonProperty(PropertyName = "Denom2")]
        public double Denominator2 { get; set; }

        [JsonProperty]
        public int Year { get; set; }

        [JsonProperty]
        public int YearRange { get; set; }

        [JsonProperty]
        public int Quarter { get; set; }

        [JsonProperty]
        public int Month { get; set; }

        /// <summary>
        ///     Key is comparator ID. Used when there may be multiple benchmarks to compare against.
        /// </summary>
        [JsonProperty(PropertyName = "Sig")]
        public Dictionary<int, int> Significance { get; set; }

        public void AddSignificance(int comparatorId, Significance significance)
        {
            if (Significance.ContainsKey(comparatorId) == false)
            {
                Significance.Add(comparatorId, (int)significance);
            }
        }

        /// <summary>
        ///     Used when then there is only one benchmark to compare against.
        /// </summary>
        [JsonProperty(PropertyName = "Significance")]
        public int? SignificanceAgainstOneBenchmark { get; set; }

        [JsonProperty(PropertyName = "NoteId")]
        public int ValueNoteId { get; set; }

        [JsonProperty]
        public int CategoryTypeId { get; set; }

        [JsonProperty]
        public int CategoryId { get; set; }

        /// <summary>
        ///     Whether or not CategoryId should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeCategoryId()
        {
            return ShouldSerializeCategoryTypeId();
        }

        /// <summary>
        ///     Whether or not CategoryId should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeCategoryTypeId()
        {
            return CategoryTypeId != 0;
        }

        public void CopyValues(Grouping grouping, TimePeriod timePeriod)
        {
            IndicatorId = grouping.IndicatorId;
            AgeId = grouping.AgeId;
            SexId = grouping.SexId;
            Year = timePeriod.Year;
            YearRange = timePeriod.YearRange;
            Quarter = timePeriod.Quarter;
            Month = timePeriod.Month;
        }

        public static CoreDataSet GetNullObject(string areaCode)
        {
            var data = GetNullObject();
            data.AreaCode = areaCode;
            data.CategoryTypeId = -1;
            data.CategoryId = -1;
            data.SexId = -1;
            data.AgeId = -1;
            return data;
        }

        public static CoreDataSet GetNullObject(string areaCode, TimePeriod timePeriod)
        {
            var data = GetNullObject();
            data.AreaCode = areaCode;
            data.Year = timePeriod.Year;
            data.YearRange = timePeriod.YearRange;
            data.Quarter = timePeriod.Quarter;
            data.Month = timePeriod.Month;
            data.CategoryTypeId = -1;
            data.CategoryId = -1;
            data.SexId = -1;
            data.AgeId = -1;
            return data;
        }

        public new static CoreDataSet GetNullObject()
        {
            var data = new CoreDataSet();
            SetDefaults(data);
            return data;
        }

        private static void SetDefaults(CoreDataSet data)
        {
            SetValueWithCIsDataDefaults(data);
            data.Count = NullValue;
            data.Denominator = NullValue;
            data.Denominator2 = NullValue;
            data.CategoryTypeId = -1;
            data.CategoryId = -1;
            data.SexId = -1;
            data.AgeId = -1;
        }

        /// <summary>
        ///     Whether or not Significance should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeSignificance()
        {
            // Do not serialise if SignificanceAgainstOneBenchmark is defined
            return ShouldSerializeSignificanceAgainstOneBenchmark() == false;
        }

        /// <summary>
        ///     Whether or not SignificanceAgainstOneBenchmark should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeSignificanceAgainstOneBenchmark()
        {
            return SignificanceAgainstOneBenchmark.HasValue;
        }

        /// <summary>
        ///     Whether or not ValueNoteId should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeValueNoteId()
        {
            return ValueNoteId != NoValueNote;
        }

        public TimePeriod GetTimePeriod()
        {
            return new TimePeriod
            {
                Year = Year,
                YearRange = YearRange,
                Quarter = Quarter,
                Month = Month
            };
        }
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}