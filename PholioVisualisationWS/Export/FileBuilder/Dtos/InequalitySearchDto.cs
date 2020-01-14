using Newtonsoft.Json;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PholioVisualisation.Export.FileBuilder.Dtos
{
    public class InequalitySearchDto
    {
        [DefaultValue(CategoryTypeIds.Undefined)]
        [JsonProperty("CategoryTypeId", DefaultValueHandling = DefaultValueHandling.Populate)]
        public int CategoryTypeId { get; set; }
        [DefaultValue(CategoryIds.Undefined)]
        [JsonProperty("CategoryId", DefaultValueHandling = DefaultValueHandling.Populate)]
        public int CategoryId { get; set; }
        [JsonProperty("Sex")]
        public int SexId { get; set; }
        [JsonProperty("Age")]
        public int AgeId { get; set; }

        public InequalitySearch MapToInequalitySearch()
        {
            return new InequalitySearch(CategoryTypeId, CategoryId, SexId, AgeId);
        }

        public static Dictionary<int, IList<InequalitySearch>> MapDicInequalitiesSearchDtoToDicInequalitiesSearch(Dictionary<int, IList<InequalitySearchDto>> dicInequalitiesSearchDto)
        {
            var inequalitySearchDic = new Dictionary<int, IList<InequalitySearch>>();

            foreach (var listDto in dicInequalitiesSearchDto)
            {
                var inequalitiesSearchList = listDto.Value.Select(dto => dto.MapToInequalitySearch()).ToList();
                inequalitySearchDic.Add(listDto.Key, inequalitiesSearchList);
            }

            return inequalitySearchDic;
        }
    }
}
