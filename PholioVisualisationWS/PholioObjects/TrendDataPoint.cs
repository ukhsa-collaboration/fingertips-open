
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class TrendDataPoint
    {
        public TrendDataPoint(CoreDataSet coreDataSet)
        {
            CoreDataSet = coreDataSet;
        }

        /// <summary>
        /// Value properties are only copied on demand so they can assigned after they have
        /// been truncated and formatted.
        /// </summary>
        public void CopyValueProperties(CoreDataSet coreDataSet)
        {
            // Copy numeric properties
            Value = coreDataSet.Value;
            Count = coreDataSet.Count;
            Denominator = coreDataSet.Denominator;
            LowerCI = coreDataSet.LowerCI95;
            UpperCI = coreDataSet.UpperCI95;
            LowerCI99_8 = coreDataSet.LowerCI99_8;
            UpperCI99_8 = coreDataSet.UpperCI99_8;

            // Copy formatted properties
            if (coreDataSet.ValueFormatted != null)
            {
                ValueF = coreDataSet.ValueFormatted;
                LowerCIF = coreDataSet.LowerCI95F;
                UpperCIF = coreDataSet.UpperCI95F;
                LowerCIF99_8 = coreDataSet.LowerCI99_8F;
                UpperCIF99_8 = coreDataSet.UpperCI99_8F;
            }

            ValueNoteId = coreDataSet.ValueNoteId;
            CategoryId = coreDataSet.CategoryId;
            CategoryTypeId = coreDataSet.CategoryTypeId;
        }

        /// <summary>
        /// The CoreDataSet object the TrendDataPoint was created from
        /// </summary>
        [JsonIgnore]
        public CoreDataSet CoreDataSet { get; set; }

        [JsonProperty(PropertyName = ("V"))]
        public string ValueF { get; set; }

        [JsonProperty(PropertyName = ("D"))]
        public double Value { get; set; }

        [JsonProperty(PropertyName = ("Denom"))]
        public double Denominator { get; set; }

        [JsonProperty(PropertyName = ("L"))]
        public double? LowerCI { get; set; }

        [JsonProperty(PropertyName = ("LF"))]
        public string LowerCIF { get; set; }

        [JsonProperty(PropertyName = ("U"))]
        public double? UpperCI { get; set; }

        [JsonProperty(PropertyName = ("UF"))]
        public string UpperCIF { get; set; }

        [JsonProperty(PropertyName = ("L99_8"))]
        public double? LowerCI99_8 { get; set; }

        [JsonProperty(PropertyName = ("LF99_8"))]
        public string LowerCIF99_8 { get; set; }

        [JsonProperty(PropertyName = ("U99_8"))]
        public double? UpperCI99_8 { get; set; }

        [JsonProperty(PropertyName = ("UF99_8"))]
        public string UpperCIF99_8 { get; set; }

        [JsonProperty(PropertyName = ("Sig"))]
        public Dictionary<int, Significance> Significance { get; set; }

        [JsonProperty(PropertyName = ("NoteId"))]
        public int ValueNoteId { get; set; }

        [JsonProperty(PropertyName = ("C"))]
        public double? Count { get; set; }

        [JsonProperty(PropertyName = ("IsC"))]
        public bool IsCountValid { get; set; }

        [JsonProperty(PropertyName = ("CategoryId"))]
        public int CategoryId { get; set; }

        [JsonProperty(PropertyName = ("CategoryTypeId"))]
        public int CategoryTypeId { get; set; }

        /// <summary>
        /// Whether or not ValueNoteId should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeValueNoteId()
        {
            return ValueNoteId != CoreDataSet.NoValueNote;
        }
    }
}
