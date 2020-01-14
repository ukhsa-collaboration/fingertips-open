using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.Export.FileBuilder.SupportModels
{
    public class SexAgeInequalitySearch
    {
        public int SexId { get; set; }
        public int AgeId { get; set; }

        public SexAgeInequalitySearch()
        {
        }

        public SexAgeInequalitySearch(int sexId, int ageId)
        {
            SexId = sexId;
            AgeId = ageId;
        }

        public static IList<SexAgeInequalitySearch> CombineSexAndAgeInSexAgeInequalitySearchList(IList<int> sexes, IList<int> ages)
        {
            var sexAgeInequalitySearch = (from sex in sexes from age in ages select new SexAgeInequalitySearch(sex, age)).ToList();
            return sexAgeInequalitySearch;
        }
    }

    public class AreaTypeIdSexAgeInequalitySearch : SexAgeInequalitySearch
    {
        public int AreaTypeId { get; set; }

        public AreaTypeIdSexAgeInequalitySearch()
        {
        }

        public AreaTypeIdSexAgeInequalitySearch(int areaTypeId, int sexId, int ageId): base(sexId, ageId)
        {
            AreaTypeId = areaTypeId;
        }

        public SexAgeInequalitySearch GetSexAgeInequalitySearch()
        {
            return new SexAgeInequalitySearch(SexId, AgeId);
        }
    }
}
