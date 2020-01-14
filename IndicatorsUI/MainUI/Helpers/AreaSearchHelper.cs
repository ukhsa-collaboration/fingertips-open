using System.Collections.Generic;
using System.Linq;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUI.Helpers
{
    public class AreaSearchHelper
    {
        private IList<int> _searchAreaTypeIds;
        private int _profileId;

        private static List<int> areaTypesWithPlacenameSearch = new List<int>
        {
            AreaTypeIds.CcgPreApr2017,
            AreaTypeIds.CountyAndUnitaryAuthorityPre2019,
            AreaTypeIds.DistrictAndUAPreApr2019
        };

        private static List<int> ccgs = new List<int>{
            AreaTypeIds.CcgPreApr2017,
            AreaTypeIds.CcgPostApr2017,
            AreaTypeIds.CcgSince2018,
            AreaTypeIds.CcgSinceApr2019
        };
 
        public AreaSearchHelper(int profileId, string searchAreaTypeIds)
        {
            _searchAreaTypeIds = new IntListStringParser(searchAreaTypeIds).IntList;
            _profileId = profileId;
        }

        public bool IsPlaceNameSearchAvailable
        {
            get
            {
                return _searchAreaTypeIds.Any(x => areaTypesWithPlacenameSearch.Contains(x));
            }
        }

        public string GetAreaTypeLabelSingular()
        {
            return IsCcg() ? "CCG" : "local authority";
        }

        public string GetAreaTypeLabelPlural()
        {
            return IsCcg() ? "CCGs" : "local authorities";
        }

        public string GetSearchHeading()
        {
            if (_profileId == ProfileIds.HealthProfiles)
            {
                return "Find your Health Profile";
            }

            if (_profileId == ProfileIds.Diabetes)
            {
                return "Diabetes footcare profile by area";
            }

            return "Find your area";
        }

        private bool IsCcg()
        {
            return _searchAreaTypeIds.Any(x => ccgs.Contains(x));
        }
    }
}