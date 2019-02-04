using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Fpm.MainUI.ViewModels.ProfilesAndIndicators
{
    public class GroupingSubheadingViewModel
    {
        [JsonProperty(PropertyName = "subheadingId")]
        public int SubheadingId { get; set; }
        [JsonProperty(PropertyName = "groupId")]
        public int GroupId { get; set; }
        [JsonProperty(PropertyName = "areaTypeId")]
        public int AreaTypeId { get; set; }
        [JsonProperty(PropertyName = "subheading")]
        public string Subheading { get; set; }
        [JsonProperty(PropertyName = "sequence")]
        public int Sequence { get; set; }
    }
}