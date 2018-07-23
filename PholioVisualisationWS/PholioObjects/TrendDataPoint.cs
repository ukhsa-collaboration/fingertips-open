
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class TrendDataPoint
    {
        public TrendDataPoint(CoreDataSet coreDataSet)
        {
            Value = coreDataSet.Value;
            ValueF = coreDataSet.ValueFormatted;
            LowerCIF = coreDataSet.LowerCI95F;
            UpperCIF = coreDataSet.UpperCI95F;
            ValueNoteId = coreDataSet.ValueNoteId;
            Count = coreDataSet.Count;
            Significance = new Dictionary<int, Significance>();
        }

        [JsonProperty(PropertyName = ("V"))]
        public string ValueF { get; set; }

        [JsonProperty(PropertyName = ("D"))]
        public double Value { get; set; }

        [JsonProperty(PropertyName = ("L"))]
        public string LowerCIF { get; set; }

        [JsonProperty(PropertyName = ("U"))]
        public string UpperCIF { get; set; }

        [JsonProperty(PropertyName = ("Sig"))]
        public Dictionary<int, Significance> Significance { get; set; }

        [JsonProperty(PropertyName = "NoteId")]
        public int ValueNoteId { get; set; }

        [JsonProperty(PropertyName = "C")]
        public double? Count { get; set; }

        [JsonProperty(PropertyName = "IsC")]
        public bool IsCountValid { get; set; }

        /// <summary>
        /// Whether or not ValueNoteId should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeValueNoteId()
        {
            return ValueNoteId != CoreDataSet.NoValueNote;
        }
    }
}
