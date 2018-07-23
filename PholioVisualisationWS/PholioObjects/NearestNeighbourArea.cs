using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class NearestNeighbourArea : IArea
    {
        public const string NearestNeighbourAreaCodePrefix = "nn-";

        public IList<AreaCodeNeighbourMapping> Neighbours { get; set; }

        private int? _sequence;

        private NearestNeighbourArea()
        {
        }

        /// <summary>
        /// WARNING: this should only be called from AreaFactory 
        /// to ensure it is fully initialised.
        /// </summary>
        public NearestNeighbourArea(string areaCode)
        {
            string[] areaCodeCharArray = areaCode.Split('-');
            Code = areaCode;
            NeighbourTypeId = int.Parse(areaCodeCharArray[1]);
            AreaCodeOfAreaWithNeighbours = areaCodeCharArray[2];
        }

        public IList<string> NeighbourAreaCodes
        {
            get { return Neighbours.Select(x => x.NeighbourAreaCode).ToList(); }
        }

        public static bool IsNearestNeighbourAreaCode(string areaCode)
        {
            return areaCode.StartsWith(NearestNeighbourAreaCodePrefix,
                StringComparison.CurrentCultureIgnoreCase);
        }

        public static NearestNeighbourArea New(int neighbourTypeId, string neighbourAreaCode)
        {
            return new NearestNeighbourArea
            {
                Code = CreateAreaCode(neighbourTypeId, neighbourAreaCode),
                NeighbourTypeId = neighbourTypeId,
                AreaCodeOfAreaWithNeighbours = neighbourAreaCode
            };
        }

        public static string CreateAreaCode(int neighbourTypeId, string neighbourAreaCode)
        {
            return NearestNeighbourAreaCodePrefix + neighbourTypeId + "-" + neighbourAreaCode;
        }

        /// <summary>
        /// e.g. returns "E10000014" from "nn-1-E10000014"
        /// </summary>
        public static string GetAreaCodeFromNeighbourAreaCode(string neighbourAreaCode)
        {
            return neighbourAreaCode.Split('-')[2];
        }

        [JsonProperty]
        public virtual int NeighbourTypeId { get; internal set; }

        [JsonProperty]
        public virtual string AreaCodeOfAreaWithNeighbours { get; internal set; }

        [JsonProperty]
        public virtual string Code { get; internal set; }

        [JsonProperty]
        public virtual int? Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }

        /// <summary>
        /// Whether or not Sequence should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeSequence()
        {
            return _sequence.HasValue;
        }

        [JsonProperty(PropertyName = "Short")]
        public string ShortName { get; set; }

        [JsonProperty]
        public virtual string Name { get; set; }

        [JsonProperty]
        public int AreaTypeId
        {
            get
            {
                return NearestNeighbourAreaType.GetAreaTypeIdFromNearestNeighbourTypeId(NeighbourTypeId);
            }
        }

        [JsonIgnore]
        public bool IsCcg
        {
            get { return false; }
        }

        [JsonIgnore]
        public bool IsGpDeprivationDecile
        {
            get { return false; }
        }

        [JsonIgnore]
        public bool IsShape
        {
            get { return false; }
        }

        [JsonIgnore]
        public bool IsCountry
        {
            get { return false; }
        }

        [JsonIgnore]
        public bool IsCountyAndUADeprivationDecile
        {
            get { return false; }
        }

        [JsonIgnore]
        public bool IsGpPractice
        {
            get { return false; }
        }

        public bool IsOnsClusterGroup
        {
            get { return false; }
        }
    }
}
