using System.Collections.Generic;
using System.Linq;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUI.Helpers
{
    public class AreaSearchHelper
    {
        private IList<int> _searchAreaTypeIds;
        private int _profileId;

        private List<int> excludedAreaTypes = new List<int>
        {
            AreaTypeIds.CcgPostApr2017, AreaTypeIds.CcgSince2018
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
                return _searchAreaTypeIds.Any(x => excludedAreaTypes.Contains(x) == false);
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
            return _searchAreaTypeIds
                .Any(x => x == AreaTypeIds.CcgPreApr2017 || x == AreaTypeIds.CcgPostApr2017);
        }
    }
}