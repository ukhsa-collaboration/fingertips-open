using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;
using System.Collections.Generic;

namespace Fpm.MainUI.Helpers
{
    public class IndicatorOwnerChanger : IIndicatorOwnerChanger
    {
        private readonly IProfilesReader _profilesReader;
        private readonly IProfileRepository _profileRepository;

        public IndicatorOwnerChanger(IProfilesReader profilesReader, IProfileRepository profileRepository)
        {
            _profilesReader = profilesReader;
            _profileRepository = profileRepository;
        }

        public void AssignIndicatorToProfile(int indicatorId, int newOwnerProfileId)
        {
            IList<IndicatorMetadataTextProperty> properties = _profilesReader.GetIndicatorMetadataTextProperties();

            //Get any overriden metadatatextvalues for the new owner as these will need to be changed
            IList<IndicatorText> newOwnerIMDTVs = _profilesReader
                .GetOverriddenIndicatorTextValuesForSpecificProfileId(indicatorId, properties, newOwnerProfileId);

            IList<IndicatorText> currentOwnerIMDTVs = null;
            if (newOwnerIMDTVs.Count > 0)
            {
                //Get IndicatorMetadataTextValues where indicatorId = Null (currentOwner)
                currentOwnerIMDTVs = _profilesReader.GetIndicatorTextValues(indicatorId, properties, null);
            }

            _profileRepository.ChangeOwner(indicatorId, newOwnerProfileId, newOwnerIMDTVs, currentOwnerIMDTVs);
        }
    }
}