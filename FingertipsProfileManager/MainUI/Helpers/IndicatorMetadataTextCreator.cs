using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.MainUI.Helpers
{
    public class IndicatorMetadataTextCreator : IIndicatorMetadataTextCreator
    {
        private ProfileRepository _profileRepository;

        public IndicatorMetadataTextCreator(ProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public void CreateNewIndicatorTextValues(int profileId, 
            IList<IndicatorMetadataTextItem> indicatorMetadataTextItems,
            IList<IndicatorMetadataTextProperty> properties, 
            int nextIndicatorId, string userName)
        {
            var allPropertiesToAdd = new List<IndicatorMetadataTextProperty>();

            foreach (var item in indicatorMetadataTextItems)
            {
                // Assign text to property
                IndicatorMetadataTextProperty property = properties
                    .First(x => x.PropertyId == item.PropertyId);
                property.Text = item.Text;
                allPropertiesToAdd.Add(property);

                // Save to change log 
                _profileRepository.LogIndicatorMetadataTextPropertyChange(
                    property.PropertyId, null, nextIndicatorId,
                    profileId, userName, DateTime.Now);
            }

            _profileRepository.CreateIndicator(allPropertiesToAdd, nextIndicatorId);
        }

    }
}