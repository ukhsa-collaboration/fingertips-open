using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;

namespace Fpm.MainUI.Helpers
{
    public interface IIndicatorMetadataTextChanger
    {
        void UpdateIndicatorTextValues(int indicatorId, IList<IndicatorMetadataTextItem> textChangesByUser,
            IList<IndicatorMetadataTextProperty> properties, string userName, int profileId,
            bool isOwnerProfileBeingEdited);
    }

    public class IndicatorMetadataTextChanger : IIndicatorMetadataTextChanger
    {
        private readonly ProfileRepository _profileRepository;
        private readonly ProfilesReader _profilesReader;

        /// <summary>
        /// To avoid creating more than one row in database
        /// </summary>
        private bool _hasNewOverridenTextEntryBeenCreated;

        public IndicatorMetadataTextChanger(ProfileRepository profileRepository, ProfilesReader profilesReader)
        {
            _profileRepository = profileRepository;
            _profilesReader = profilesReader;
        }

        public void UpdateIndicatorTextValues(int indicatorId, IList<IndicatorMetadataTextItem> textChangesByUser,
            IList<IndicatorMetadataTextProperty> properties, string userName, int profileId, bool isOwnerProfileBeingEdited)
        {
            foreach (var textChangeByUser in textChangesByUser)
            {
                IndicatorMetadataTextProperty property = properties.First(x => x.PropertyId == textChangeByUser.PropertyId);

                // Get properties + existing values
                IndicatorText indicatorText = _profilesReader.GetIndicatorTextValues(indicatorId,
                        new List<IndicatorMetadataTextProperty> { property }, profileId).First();

                // Figure out whether to update generic or specific property
                var profileIdForProperty = GetProfileIdForProperty(profileId, isOwnerProfileBeingEdited);

                // Text that is being replaced
                var oldText = GetTextBeingReplaced(textChangeByUser, indicatorText);

                // Audit change 
                _profileRepository.LogIndicatorMetadataTextPropertyChange(property.PropertyId, oldText, indicatorId,
                    profileIdForProperty, userName, DateTime.Now);

                var indicatorAlreadyOverridden = _profileRepository.DoesOverriddenIndicatorMetadataRecordAlreadyExist(
                    indicatorId, profileId);

                var userText = textChangeByUser.Text;

                if (isOwnerProfileBeingEdited == false && _hasNewOverridenTextEntryBeenCreated == false &&
                    indicatorAlreadyOverridden == false && textChangeByUser.IsBeingOverriddenForFirstTime)
                {
                    // Create new override
                    _profileRepository.CreateNewOverriddenIndicator(property, userText, indicatorId, profileId);
                    _hasNewOverridenTextEntryBeenCreated = true;
                }
                else
                {
                    // Update existing metadata
                    if (userText == indicatorText.ValueGeneric && indicatorAlreadyOverridden)
                    {
                        // User has cleared the override 
                        userText = null;
                    }
                    _profileRepository.UpdateProperty(property, userText, indicatorId, profileIdForProperty);
                }
            }

            RemoveEmptyOverrides(indicatorId, profileId, properties);
        }

        private static int? GetProfileIdForProperty(int profileId, bool isOwnerProfileBeingEdited)
        {
            return isOwnerProfileBeingEdited
                ? (int?)null // Update generic property
                : profileId; // Update profile specific property
        }

        private string GetTextBeingReplaced(IndicatorMetadataTextItem item, IndicatorText indicatorText)
        {
            string oldText;
            if (item.IsBeingOverriddenForFirstTime)
            {
                // The user is overriding the generic value for the first time
                oldText = null;
            }
            else if (indicatorText.HasSpecificValue())
            {
                // The user is modifying an existing specific value
                oldText = indicatorText.ValueSpecific;
            }
            else
            {
                // User has changed the generic value
                oldText = indicatorText.ValueGeneric;
            }
            return oldText;
        }

        /// <summary>
        /// Clean-up - Remove any overridden Indicator Metadata records where all text property fields are null
        /// </summary>
        private void RemoveEmptyOverrides(int indicatorId, int profileId,
            IList<IndicatorMetadataTextProperty> properties)
        {
            if (!_profilesReader.GetIndicatorTextValues(indicatorId, properties, profileId).ToList().Any(x => x.HasSpecificValue()))
            {
                _profileRepository.DeleteOverriddenMetadataTextValues(indicatorId, profileId);
            }
        }
    }
}